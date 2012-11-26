﻿using System;
using System.Collections.Generic;
using System.Linq;
using Buffer = Rasterizr.Resources.Buffer;

namespace Rasterizr.Pipeline.InputAssembler
{
	public class InputAssemblerStage
	{
		public const int VertexInputResourceSlotCount = 32;

		private readonly VertexBufferBinding[] _vertexBufferBindings;
		private IndexBufferBinding _indexBufferBinding;

		public InputLayout InputLayout { get; set; }
		public PrimitiveTopology PrimitiveTopology { get; set; }

		public InputAssemblerStage()
		{
			_vertexBufferBindings = new VertexBufferBinding[VertexInputResourceSlotCount];
		}

		public void SetVertexBuffers(int startSlot, params VertexBufferBinding[] vertexBufferBindings)
		{
			for (int i = 0; i < vertexBufferBindings.Length; i++)
				_vertexBufferBindings[i + startSlot] = vertexBufferBindings[i];
		}

		public void SetIndexBuffer(Buffer indexBuffer, Format format, int offset)
		{
			if (format != Format.R16_UInt && format != Format.R32_UInt)
				throw new ArgumentOutOfRangeException("format");
			_indexBufferBinding = new IndexBufferBinding(indexBuffer, format, offset);
		}

		public void GetVertexBuffers(int startSlot, int numBuffers, VertexBufferBinding[] vertexBufferBindings)
		{
			for (int i = 0; i < numBuffers; i++)
				vertexBufferBindings[i] = _vertexBufferBindings[i + startSlot];
		}

		public void GetIndexBuffer(out Buffer indexBuffer, out Format format, out int offset)
		{
			indexBuffer = _indexBufferBinding.Buffer;
			format = _indexBufferBinding.Format;
			offset = _indexBufferBinding.Offset;
		}

		internal IEnumerable<InputAssemblerVertexOutput> GetVertexStream(
			int vertexCount, 
			int startVertexLocation)
		{
			var vertexBufferIndices = _vertexBufferBindings
				.Select(x => new VertexBufferIndex(x, startVertexLocation))
				.ToArray();

			return GetVertexStreamInternal(vertexCount, 0, startVertexLocation, vertexBufferIndices);
		}

		internal IEnumerable<InputAssemblerVertexOutput> GetVertexStreamIndexed(
			int indexCount, 
			int startIndexLocation, 
			int baseVertexLocation)
		{
			var vertexBufferIndices = _vertexBufferBindings
				.Select(x => new IndexedVertexBufferIndex(_indexBufferBinding, startIndexLocation, x, baseVertexLocation))
				.ToArray();

			return GetVertexStreamInternal(indexCount, 0, startIndexLocation, vertexBufferIndices);
		}

		internal IEnumerable<InputAssemblerVertexOutput> GetVertexStreamInstanced(
			int vertexCountPerInstance, 
			int instanceCount, 
			int startVertexLocation, 
			int startInstanceLocation)
		{
			// Setup per-instance data (applies across draw call).
			// TODO: This isn't quite right - we need make vertexBufferIndices a dictionary, keyed on slot.
			var vertexBufferIndices = InputLayout.Slots
				.Select(x => (x.InputSlotClass == InputClassification.PerInstanceData) 
					? new InstancedVertexBufferIndex(x.InstanceDataStepRate, _vertexBufferBindings[x.InputSlot], startInstanceLocation) 
					: new VertexBufferIndex(_vertexBufferBindings[x.InputSlot], startVertexLocation))
				.ToArray();
			var perInstanceBufferIndices = vertexBufferIndices
				.Where(x => x.InputDataClass == InputClassification.PerInstanceData)
				.ToArray();
			var perVertexBufferIndices = vertexBufferIndices
				.Where(x => x.InputDataClass == InputClassification.PerVertexData)
				.ToArray();

			for (int i = 0; i < instanceCount; i++)
			{
				// Reset per-vertex data (applies to each instance).
				foreach (var perVertexBufferIndex in perVertexBufferIndices)
					perVertexBufferIndex.Reset();

				foreach (var result in GetVertexStreamInternal(vertexCountPerInstance, i, startVertexLocation, vertexBufferIndices))
					yield return result;

				foreach (var vertexBufferIndex in perInstanceBufferIndices)
					vertexBufferIndex.Increment(InputClassification.PerInstanceData);
			}
		}

		internal IEnumerable<InputAssemblerVertexOutput> GetVertexStreamIndexedInstanced(
			int indexCountPerInstance, 
			int instanceCount, 
			int startIndexLocation, 
			int baseVertexLocation, 
			int startInstanceLocation)
		{
			throw new NotImplementedException();
		}

		private IEnumerable<InputAssemblerVertexOutput> GetVertexStreamInternal(
			int vertexCount, 
			int instanceID,
			int vertexID, 
			VertexBufferIndex[] vertexBufferIndices)
		{
			for (int i = 0; i < vertexCount; i++)
			{
				var output = new InputAssemblerVertexOutput();
				output.VertexID = vertexID++;
				output.InstanceID = instanceID;
				output.Data = new byte[InputLayout.DataLength];

				foreach (var inputElement in InputLayout.Elements)
					vertexBufferIndices[inputElement.InputSlot].GetData(
						output.Data, inputElement.RegisterIndex * sizeof (float) * 4,
						inputElement.AlignedByteOffset,
						FormatHelper.SizeOfInBytes(inputElement.Format));

				yield return output;

				foreach (var vertexBufferIndex in vertexBufferIndices)
					vertexBufferIndex.Increment(InputClassification.PerVertexData);
			}
		}

	}
}