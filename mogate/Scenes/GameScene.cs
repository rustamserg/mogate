using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class GameScene : Scene
	{
		public GameScene (Game game, string name) : base(game, name)
		{
		}

		protected override void OnPostComposeScene()
		{
			AddLayer (new HeroLayer (Game, "hero", 0));
		}
	}
}

