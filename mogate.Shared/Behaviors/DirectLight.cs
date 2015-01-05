﻿using System;
using Microsoft.Xna.Framework;

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