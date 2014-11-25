using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class InterScene : Scene
	{
		public InterScene (Game game, string name) : base(game, name)
		{
		}

		protected override void LoadLayers()
		{
			AddLayer (new InterLayer (Game, "inter", this, 0));
		}
	}
}

