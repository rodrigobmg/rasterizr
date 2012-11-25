﻿using Rasterizr.Pipeline.InputAssembler;
using Rasterizr.Pipeline.OutputMerger;
using Rasterizr.Pipeline.Rasterizer;
using Rasterizr.Resources;

namespace Rasterizr
{
	public class DeviceContext
	{
		private readonly InputAssemblerStage _inputAssembler;
		private readonly RasterizerStage _rasterizer;
		private readonly OutputMergerStage _outputMerger;

		public InputAssemblerStage InputAssembler
		{
			get { return _inputAssembler; }
		}

		public RasterizerStage Rasterizer
		{
			get { return _rasterizer; }
		}

		public OutputMergerStage OutputMerger
		{
			get { return _outputMerger; }
		}

		public DeviceContext()
		{
			_inputAssembler = new InputAssemblerStage();
			_rasterizer = new RasterizerStage();
			_outputMerger = new OutputMergerStage();
		}

		public void ClearDepthStencilView(DepthStencilView depthStencilView, DepthStencilClearFlags clearFlags, float depth, byte stencil)
		{
			
		}

		public void ClearRenderTargetView(RenderTargetView renderTargetView, Color4 color)
		{
			renderTargetView.Clear(color);
		}
	}
}