using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace mogate
{
	public static class Utils
	{
		public static Random Rand = new Random(DateTime.UtcNow.Millisecond);
		public enum Direction { Up, Down, Left, Right };

		public static T RandomEnumValue<T> ()
		{
			return Enum.GetValues(typeof (T)).Cast<T>().OrderBy(x => Rand.Next()).FirstOrDefault();
		}

		public static double Dist (Point from, Point to)
		{
			return Math.Sqrt(Math.Pow(from.X  - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
		}

		public static double DirectionDist (Point from, Point to, Direction fromDir)
		{
			if (fromDir == Direction.Up && (from.X != to.X || from.Y < to.Y))
				return double.MaxValue;
			if (fromDir == Direction.Down && (from.X != to.X || from.Y > to.Y))
				return double.MaxValue;
			if (fromDir == Direction.Left && (from.X < to.X || from.Y != to.Y))
				return double.MaxValue;
			if (fromDir == Direction.Right && (from.X > to.X || from.Y != to.Y))
				return double.MaxValue;
			return Dist (from, to);
		}

		public static bool FindDirection(Point from, Point to, out Direction direction)
		{
			if (from.X == to.X && from.Y < to.Y) {
				direction = Direction.Down;
				return true;
			} else if (from.X == to.X && from.Y > to.Y) {
				direction = Direction.Up;
				return true;
			}  else if (from.X < to.X && from.Y == to.Y) {
				direction = Direction.Right;
				return true;
			} else if (from.X > to.X && from.Y == to.Y) {
				direction = Direction.Left;
				return true;
			}
			direction = Direction.Up;
			return false;
		}

		public static void Shuffle<T>(this IList<T> list)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = Rand.Next(n + 1);
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
