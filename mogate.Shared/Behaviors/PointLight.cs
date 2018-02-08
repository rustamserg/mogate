using Elizabeth;
using Microsoft.Xna.Framework;
using System;

namespace mogate
{
    public class PointLight : IBehavior
	{
		public Type Behavior { get { return typeof(PointLight); } }

		public enum DistanceType { Small, Normal, Big };

		public DistanceType Distance { get; set; }
		public Color LightColor { get; set; }

		public PointLight(DistanceType dist, Color light)
		{
			LightColor = light;
			Distance = dist;
		}
	}
}

