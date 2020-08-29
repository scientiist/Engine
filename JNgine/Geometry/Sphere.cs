using JNgine.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace JNgine.Primitives
{
	public class Sphere: GeometricPrimitive
	{

		public Sphere(GraphicsDevice graphics): this(graphics, 16) { }

		public Sphere(GraphicsDevice graphics, int tessellation)
		{
			if (tessellation < 3)
				throw new ArgumentOutOfRangeException("tesselation");


			int verticalSegments = tessellation;
			int horizontalSegments = tessellation * 2;


			float radius = 0.5f;


			AddVertex(Vector3.Down * radius, Vector3.Down);

			for (int i = 0; i < verticalSegments - 1; i++)
			{
				float latitude = ((i + 1) * MathHelper.Pi / verticalSegments) - MathHelper.PiOver2;

				float dy = (float)Math.Sin(latitude);
				float dxz = (float)Math.Cos(latitude);

				for (int j = 0; j < horizontalSegments; j++)
				{
					float longitude = j * MathHelper.TwoPi / horizontalSegments;

					float dx = (float)Math.Cos(longitude) * dxz;
					float dz = (float)Math.Sin(longitude) * dxz;

					Vector3 normal = new Vector3(dx, dy, dz);


					AddVertex(normal * radius, normal);
				}
			}

			AddVertex(Vector3.Up * radius, Vector3.Up);

			for (int i = 0; i < horizontalSegments; i++)
			{
				AddIndex(0);
				AddIndex(1 + (i + 1) % horizontalSegments);
				AddIndex(1 + i);
			}
			// Fill the sphere body with triangles joining each pair of latitude rings.
			for (int i = 0; i < verticalSegments - 2; i++)
			{
				for (int j = 0; j < horizontalSegments; j++)
				{
					int nextI = i + 1;
					int nextJ = (j + 1) % horizontalSegments;

					AddIndex(1 + i * horizontalSegments + j);
					AddIndex(1 + i * horizontalSegments + nextJ);
					AddIndex(1 + nextI * horizontalSegments + j);

					AddIndex(1 + i * horizontalSegments + nextJ);
					AddIndex(1 + nextI * horizontalSegments + nextJ);
					AddIndex(1 + nextI * horizontalSegments + j);
				}
			}

			// Create a fan connecting the top vertex to the top latitude ring.
			for (int i = 0; i < horizontalSegments; i++)
			{
				AddIndex(CurrentVertex - 1);
				AddIndex(CurrentVertex - 2 - (i + 1) % horizontalSegments);
				AddIndex(CurrentVertex - 2 - i);
			}

			InitializePrimitive(graphics);
		}
	}
}
