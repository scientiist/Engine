using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNgine
{
	public class Floor: IGeometry3D
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

		BasicEffect basicEffect;

		private int floorWidth;
		private int floorHeight;
		private VertexBuffer floorBuffer;
		private GraphicsDevice device;
		private Color[] floorColors = new Color[2] { Color.White, Color.Black };

		public Floor(GraphicsDevice device, int width, int height) {
			Opacity = 1;
			Size = new Vector3(1, 1, 1);
			Color = new Color(1.0f, 1.0f, 1.0f);
			this.device = device;
			this.floorWidth = width;
			this.floorHeight = height;
			BuildFloorBuffer();
			basicEffect = new BasicEffect(device);
			basicEffect.EnableDefaultLighting();
		}

		private void BuildFloorBuffer() {
			List<VertexPositionColor> vertexList = new List<VertexPositionColor>();

			int counter = 0;

			for(int x = 0; x< floorWidth; x++) 
			{
				counter++;
				for (int z = 0; z < floorHeight; z++) {
					counter++;

					foreach(VertexPositionColor vertex in FloorTile(x, z, floorColors[counter%2])) {
						vertexList.Add(vertex);
					}
				}
			}
			floorBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertexList.Count, BufferUsage.None);
			floorBuffer.SetData<VertexPositionColor>(vertexList.ToArray());
		}

		public void Draw(GraphicsDevice device, Camera camera) {
			basicEffect.VertexColorEnabled = true;
			basicEffect.View = camera.View;
			basicEffect.Projection = camera.Projection;
			basicEffect.World = WorldMatrix;
			basicEffect.Alpha = Opacity;
			basicEffect.DiffuseColor = Color.ToVector3();
			basicEffect.AmbientLightColor = Color.ToVector3();
			basicEffect.EmissiveColor = Color.ToVector3();
			basicEffect.SpecularColor = Color.ToVector3();

			foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				device.SetVertexBuffer(floorBuffer);
				device.DrawPrimitives(PrimitiveType.TriangleList, 0, floorBuffer.VertexCount / 3);
			}
		}


		private List<VertexPositionColor> FloorTile(int xOffset, int zOffset, Color tileColor) {
			List<VertexPositionColor> vList = new List<VertexPositionColor>();

			Random r = new Random();

			vList.Add(new VertexPositionColor(new Vector3(0 + xOffset, 0, 0 + zOffset)*2, tileColor));
			vList.Add(new VertexPositionColor(new Vector3(1 + xOffset, 0, 0 + zOffset)*2, tileColor));
			vList.Add(new VertexPositionColor(new Vector3(0 + xOffset, 0, 1 + zOffset)*2, tileColor));
			vList.Add(new VertexPositionColor(new Vector3(1 + xOffset, 0, 0 + zOffset)*2, tileColor));
			vList.Add(new VertexPositionColor(new Vector3(1 + xOffset, 0, 1 + zOffset)*2, tileColor));
			vList.Add(new VertexPositionColor(new Vector3(0 + xOffset, 0, 1 + zOffset)*2, tileColor));
			return vList;
		}
	}
}
