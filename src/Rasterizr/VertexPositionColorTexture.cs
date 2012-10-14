using System.Runtime.InteropServices;
using Nexus;
using Nexus.Graphics.Colors;
using Rasterizr.Core.InputAssembler;
using Rasterizr.Core.ShaderCore;

namespace Rasterizr
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionColorTexture
	{
		public Point3D Position;
		public ColorF Color;
		public Point2D TextureCoordinate;

		public VertexPositionColorTexture(Point3D position, ColorF color, Point2D textureCoordinate)
		{
			Position = position;
			Color = color;
			TextureCoordinate = textureCoordinate;
		}

		public static InputElementDescription[] InputElements
		{
			get
			{
				return new[]
				{
					new InputElementDescription(Semantics.Position, 0),
					new InputElementDescription(Semantics.Color, 0),
					new InputElementDescription(Semantics.TexCoord, 0)
				};
			}
		}
	}
}