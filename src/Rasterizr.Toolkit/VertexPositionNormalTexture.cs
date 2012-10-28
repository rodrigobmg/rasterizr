using System.Runtime.InteropServices;
using Nexus;
using Rasterizr.InputAssembler;
using Rasterizr.ShaderCore;

namespace Rasterizr.Toolkit
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionNormalTexture
	{
		public Point3D Position;
		public Vector3D Normal;
		public Point2D TextureCoordinate;

		public VertexPositionNormalTexture(Point3D position, Vector3D normal, Point2D textureCoordinate)
		{
			Position = position;
			Normal = normal;
			TextureCoordinate = textureCoordinate;
		}

		public static InputElementDescription[] InputElements
		{
			get
			{
				return new[]
				{
					new InputElementDescription(Semantics.Position, 0),
					new InputElementDescription(Semantics.Normal, 0),
					new InputElementDescription(Semantics.TexCoord, 0)
				};
			}
		}
	}
}