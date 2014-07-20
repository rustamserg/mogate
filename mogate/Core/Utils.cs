using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace mogate
{
	public static class Utils
	{
		static Random m_rand = new Random(DateTime.UtcNow.Millisecond);

		public static T RandomEnumValue<T> ()
		{
			return Enum.GetValues(typeof (T)).Cast<T>().OrderBy(x => m_rand.Next()).FirstOrDefault();
		}

		public static double Dist (Point from, Point to)
		{
			return Math.Sqrt(Math.Pow(from.X  - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
		}

		public static void Shuffle<T>(this IList<T> list)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = m_rand.Next(n + 1);
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}

		public static Rectangle RectangleFromString(string rect)
		{
			// convert {{x, y}, {w, h}} to rect
			string parsed = rect.Replace ("{", "").Replace ("}", "");
			string[] dt = parsed.Split (',');
			return new Rectangle (int.Parse (dt [0]), int.Parse (dt [1]), int.Parse (dt [2]), int.Parse (dt [3]));
		}
	}
}
