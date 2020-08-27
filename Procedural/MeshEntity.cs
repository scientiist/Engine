using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Procedural
{

    public static class Utils
	{
        public static Vector3 VectorFromColor(Color c)
		{
            return new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
        }
	}

    public class MeshEntity
    {
        Model model;


        public Color Color { get; set; }
        public Color SpecularColor { get; set; }
        public float Alpha { get; set; }

        public Texture2D Texture { get; set; }

        public Vector3 Size;

        public MeshEntity(Model model)
        {
            this.model = model;
            Position = new Vector3(0,0,0);
            Rotation = new Vector3();
            Size = new Vector3(1, 1, 1);
            Color = new Color(1.0f, 1.0f, 1.0f);
        }

        public Vector3 Position;

        public Vector3 Rotation;

        public Quaternion RotationQuaternion
		{
            get { return Quaternion.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z); }
		}

        public Matrix Matrix
        {
            get { return Matrix.CreateFromQuaternion(RotationQuaternion) * Matrix.CreateTranslation(Position);  }
             
        }

        public void Draw(GraphicsDevice graphics, Camera camera)//, Lighting lighting)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    var effect = part.Effect;
                    if (part.PrimitiveCount > 0)
                    {
                        BasicEffect basicEffect = effect as BasicEffect;

                        if (basicEffect != null)
                        {
                            basicEffect.View = camera.View;
                            basicEffect.Projection = camera.ProjectionMatrix;
                            basicEffect.World = Matrix.CreateScale(Size) * Matrix;
                            if (Texture != null) {
                                basicEffect.TextureEnabled = true;
                                basicEffect.Texture = Texture;
                            }
                            basicEffect.SpecularColor = Utils.VectorFromColor(SpecularColor);
                            basicEffect.SpecularPower = 0.5f;
                            basicEffect.LightingEnabled = true;
                            basicEffect.EnableDefaultLighting();
                            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f)*Position;
                            basicEffect.DirectionalLight0.Direction = new Vector3(1, 0, 0);
                            basicEffect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);
                            //basicEffect.AmbientLightColor = Utils.VectorFromColor(lighting.Ambient);
                           // basicEffect.FogColor          = Utils.VectorFromColor(lighting.FogColor);
                           // basicEffect.FogStart          = lighting.FogStart;
                           // basicEffect.FogEnd            = lighting.FogEnd;
                           // lighting.ApplyLightSources(this, basicEffect);
                        } else {
                            effect.Parameters["WorldViewProjection"].SetValue(Matrix.Identity * camera.View * camera.ProjectionMatrix);
                        }
                        graphics.SetVertexBuffer(part.VertexBuffer);
                        graphics.Indices = part.IndexBuffer;

                        for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                        {
                            effect.CurrentTechnique.Passes[i].Apply();
                            graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                        }
                    }
                }
            }
        }
    }
}
