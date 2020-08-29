using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace JNgine
{
	public struct VertexPositionNormal: IVertexType
	{
		public Vector3 Position;
		public Vector3 Normal;



		public VertexPositionNormal(Vector3 pos, Vector3 norm)
		{
			Position = pos;
			Normal = norm;
		}

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
		(
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
		);

		VertexDeclaration IVertexType.VertexDeclaration
		{
			get { return VertexPositionNormal.VertexDeclaration; }
		}
	}
}
