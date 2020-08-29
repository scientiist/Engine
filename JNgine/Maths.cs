using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNgine
{

	public static class Maths
	{
		public static float Lerp(this float a, float b, float alpha)
		{
			return a + (b - a) * alpha;
		}
	}
}
