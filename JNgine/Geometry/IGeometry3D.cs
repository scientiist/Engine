using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace JNgine
{
	public interface IGeometry3D
	{
		float Opacity { get; set; }
		Color Color { get; set; }
		Texture2D Texture { get; set; }
		Vector3 Size { get; set; }
		Vector3 Position { get; set; }
		Vector3 Rotation { get; set; }
		Quaternion RotationQuaternion { get; }

		Matrix WorldMatrix { get; }

		void Draw(GraphicsDevice g, Camera c);
	}
}
