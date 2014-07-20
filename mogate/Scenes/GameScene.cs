using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class GameScene : Scene
	{
		public GameScene (Game game, string name) : base(game, name)
		{
		}

		protected override void LoadLayers()
		{
			AddLayer (new MapGridLayer (Game, "map", 0));
			AddLayer (new ItemsLayer (Game, "items", 1));
			AddLayer (new HeroLayer (Game, "hero", 2));
			AddLayer (new MonstersLayer (Game, "monsters", 3));
			AddLayer (new EffectsLayer (Game, "effects", 4));
		}
	}
}

