using System.Collections.Generic;
using Nexus;
using Rasterizr.Core.InputAssembler;
using Rasterizr.Core.OutputMerger;
using Rasterizr.Core.Rasterizer;
using Rasterizr.Core.ShaderCore;
using Rasterizr.Core.ShaderCore.GeometryShader;
using Rasterizr.Core.ShaderCore.PixelShader;
using Rasterizr.Core.ShaderCore.VertexShader;

namespace Rasterizr.Core
{
	public class RasterizrDevice
	{
		#region Properties

		public InputAssemblerStage InputAssembler { get; private set; }
		public VertexShaderStage VertexShader { get; private set; }
		public GeometryShaderStage GeometryShader { get; private set; }
		public RasterizerStage Rasterizer { get; private set; }
		public PixelShaderStage PixelShader { get; private set; }
		public OutputMergerStage OutputMerger { get; private set; }

		#endregion

		#region Constructor

		public RasterizrDevice()
		{
			InputAssembler = new InputAssemblerStage();
			VertexShader = new VertexShaderStage(InputAssembler);
			GeometryShader = new GeometryShaderStage();

			PixelShader = new PixelShaderStage();
			OutputMerger = new OutputMergerStage();

			Rasterizer = new RasterizerStage(VertexShader, PixelShader, OutputMerger);
		}

		#endregion

		public void ClearDepthBuffer(float depth)
		{
			OutputMerger.DepthBuffer.Clear(depth);
		}

		public void ClearRenderTarget(ColorF color)
		{
			OutputMerger.RenderTarget.Clear(color);
		}

		public void Draw(int vertexCount, int startVertexLocation)
		{
			BeginDraw();
			var inputAssemblerOutputs = InputAssembler.Run(vertexCount, startVertexLocation);
			DrawInternal(inputAssemblerOutputs);
		}

		public void DrawIndexed(int indexCount, int startIndexLocation, int baseVertexLocation)
		{
			BeginDraw();
			var inputAssemblerOutputs = InputAssembler.RunIndexed(indexCount, startIndexLocation, baseVertexLocation);
			DrawInternal(inputAssemblerOutputs);
		}

		private void BeginDraw()
		{
			new ShaderValidator().CheckCompatibility(VertexShader.VertexShader, PixelShader.PixelShader);
		}

		private void DrawInternal(IEnumerable<InputAssemblerOutput> inputAssemblerOutputs)
		{
			var vertexShaderOutputs = VertexShader.Run(inputAssemblerOutputs);
			var geometryShaderOutputs = GeometryShader.Run(vertexShaderOutputs);
			var rasterizerOutputs = Rasterizer.Run(geometryShaderOutputs);
			var pixelShaderOutputs = PixelShader.Run(rasterizerOutputs);
			OutputMerger.Run(pixelShaderOutputs);
		}
	}
}