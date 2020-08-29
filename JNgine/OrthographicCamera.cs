using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace JNgine
{
	class OrthographicCamera: Camera
	{

		public OrthographicCamera(Game game, Vector3 position, Vector3 rotation, float speed) : base(game, position, rotation, speed)
		{

		}

		protected override void UpdateProjectionMatrix()
		{
			Projection = Matrix.CreateOrthographic(100, 100, 0.05f, 1000.0f);
		}
	}
}
