using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace JNgine.Geometry
{
	public abstract class GeometricPrimitive: IGeometry3D, IDisposable
	{
		#region Properties
		public float Opacity { get; set; }
		public Color Color { get; set; }
		public Texture2D Texture { get; set; }
		public Vector3 Size { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }

		public Quaternion RotationQuaternion
		{
			get { return Quaternion.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z); }
		}
		public Matrix WorldMatrix
		{
			get { return Matrix.CreateFromQuaternion(RotationQuaternion) * Matrix.CreateTranslation(Position); }
		}
		#endregion

		List<VertexPositionNormal> vertices = new List<VertexPositionNormal>();
		List<ushort> indices = new List<ushort>();


		public GeometricPrimitive()
		{
			Position = new Vector3(0, 0, 0);
			Size = new Vector3(1, 1, 1);
			Color = new Color(1.0f, 1.0f, 1.0f);
			Opacity = 1;
		}


		VertexBuffer vertexBuffer;
		IndexBuffer indexBuffer;
		BasicEffect basicEffect;


		protected void AddVertex(Vector3 position, Vector3 normal)
		{
			vertices.Add(new VertexPositionNormal(position, normal));
		}

		protected void AddIndex(int index)
		{
			if (index > ushort.MaxValue)
				throw new ArgumentOutOfRangeException("index");

			indices.Add((ushort)index);

		}


		protected int CurrentVertex
		{
			get { return vertices.Count; }
		}

		protected void InitializePrimitive(GraphicsDevice graphicsDevice)
		{
			vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormal), vertices.Count, BufferUsage.None);

			vertexBuffer.SetData(vertices.ToArray());

			indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), indices.Count, BufferUsage.None);

			indexBuffer.SetData(indices.ToArray());
			basicEffect = new BasicEffect(graphicsDevice);
			basicEffect.EnableDefaultLighting();
		}


		~GeometricPrimitive()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (vertexBuffer != null)
					vertexBuffer.Dispose();

				if (indexBuffer != null)
					indexBuffer.Dispose();

				if (basicEffect != null)
					basicEffect.Dispose();
			}
		}

		public void Draw(GraphicsDevice graphics, Effect effect) {

			// Set our vertex declaration, vertex buffer, and index buffer.
			graphics.SetVertexBuffer(vertexBuffer);

			graphics.Indices = indexBuffer;


			foreach (EffectPass effectPass in effect.CurrentTechnique.Passes)
			{
				effectPass.Apply();

				int primitiveCount = indices.Count / 3;

				graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);

				// obsolete
				// graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,vertices.Count, 0, primitiveCount);

			}
		}


		public void Draw(GraphicsDevice device, Camera camera)//Matrix view, Matrix projection, Color color)
		{
			// Set BasicEffect parameters.
			basicEffect.LightingEnabled = true;
			if (Texture != null) {
                                basicEffect.TextureEnabled = true;
                                basicEffect.Texture = Texture;
                            }
			basicEffect.EnableDefaultLighting();
			basicEffect.World = Matrix.CreateScale(Size) * WorldMatrix;
			basicEffect.View = camera.View;
			basicEffect.Projection = camera.Projection;
			basicEffect.DiffuseColor = Color.ToVector3();
			basicEffect.Alpha = Opacity;
			basicEffect.AmbientLightColor = camera.Ambient.ToVector3();
			basicEffect.FogColor = camera.FogColor.ToVector3();
			basicEffect.FogStart = camera.FogStart;
			basicEffect.FogEnd = camera.FogEnd;

			device.DepthStencilState = DepthStencilState.Default;

		//	if (Alpha < 1)
		//	{
				// Set renderstates for alpha blended rendering.
				device.BlendState = BlendState.AlphaBlend;
		//	}
			//else
			//{
				// Set renderstates for opaque rendering.
			//	device.BlendState = BlendState.Opaque;
			//}

			// Draw the model, using BasicEffect.
			Draw(device, basicEffect);
		}
	}
}
