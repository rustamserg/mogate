using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class MainScene: Scene
	{
		public MainScene (Game game, string name) : base(game, name)
		{
		}

		protected override void LoadLayers()
		{
			AddLayer (new MenuLayer (Game, "menu", 0));
		}
	}
}

