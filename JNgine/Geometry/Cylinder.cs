using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace JNgine.Geometry
{
	public class Cylinder : GeometricPrimitive
	{

		static Vector3 GetCircleVector(int i, int tesselation)
		{
			float angle = i * MathHelper.TwoPi / tesselation;

			float dx = (float)Math.Cos(angle);
			float dy = (float)Math.Sin(angle);


			return new Vector3(dx, 0, dy);

		}

		public Cylinder(GraphicsDevice device) : this(device, 32)
		{

		}

		public Cylinder(GraphicsDevice device, int tessellation)
		{
			if (tessellation < 3)
				throw new ArgumentOutOfRangeException("tesselation");

			for (int i = 0; i < tessellation; i++)
			{
				Vector3 normal = GetCircleVector(i, tessellation);

				AddVertex(normal*0.5f  + Vector3.Up * 0.5f, normal);
				AddVertex(normal*0.5f + Vector3.Down * 0.5f, normal);

				AddIndex(i * 2);
				AddIndex(i * 2 + 1);
				AddIndex((i * 2 + 2) % (tessellation * 2));

				AddIndex(i * 2 + 1);
				AddIndex((i * 2 + 3) % (tessellation * 2));
				AddIndex((i * 2 + 2) % (tessellation * 2));
			}

			// Create flat triangle fan caps to seal the top and bottom.
			CreateCap(tessellation, Vector3.Up);
			CreateCap(tessellation, Vector3.Down);

			InitializePrimitive(device);

		}


		void CreateCap(int tessellation, Vector3 normal)
		{
			// Create cap indices.
			for (int i = 0; i < tessellation - 2; i++)
			{
				if (normal.Y > 0)
				{
					AddIndex(CurrentVertex);
					AddIndex(CurrentVertex + (i + 1) % tessellation);
					AddIndex(CurrentVertex + (i + 2) % tessellation);
				}
				else
				{
					AddIndex(CurrentVertex);
					AddIndex(CurrentVertex + (i + 2) % tessellation);
					AddIndex(CurrentVertex + (i + 1) % tessellation);
				}
			}

			// Create cap vertices.
			for (int i = 0; i < tessellation; i++)
			{
				Vector3 position = GetCircleVector(i, tessellation) +
								   normal;

				AddVertex(position*0.5f, normal);
			}
		}
	}
}
