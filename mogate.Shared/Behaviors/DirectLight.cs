using Elizabeth;
using Microsoft.Xna.Framework;
using System;

namespace mogate
{
    public class DirectLight : IBehavior
	{
		public Type Behavior { get { return typeof(DirectLight); } }

		public Color LightColor { get; set; }

		public DirectLight (Color light)
		{
			LightColor = light;
		}
	}
}