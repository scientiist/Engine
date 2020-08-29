using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace JNgine
{
	public class Scene
	{

		public Color SkyColor { get; set; }

		public Camera Camera { get; set; }

		public List<IGeometry3D> Geometry { get; set; }



		public Scene(Game game)
		{
			Geometry = new List<IGeometry3D>();
			Camera = new Camera(game, new Vector3(10, 1, 5), Vector3.Zero, 15f);
		}

		public void AddGeometry(IGeometry3D geom)
		{
			Geometry.Add(geom);
		}


		

		public void Draw(GraphicsDevice device)
		{

			foreach(IGeometry3D geom in Geometry)
			{
				geom.Draw(device, Camera);
			}
		}


	}
}
