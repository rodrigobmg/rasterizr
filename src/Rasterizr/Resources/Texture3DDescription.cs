﻿namespace Rasterizr.Resources
{
	public struct Texture3DDescription
	{
		public int Width;
		public int Height;
		public int Depth;
		public int MipLevels;
		public int ArraySize;
		public Format Format;
		public BindFlags BindFlags;
	}
}