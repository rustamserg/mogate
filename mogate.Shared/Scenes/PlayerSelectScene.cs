using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class PlayerSelectScene : Scene
	{
		public PlayerSelectScene (Game game, string name) : base(game, name)
		{
		}

		protected override void LoadLayers()
		{
			AddLayer (new MenuBackgroundLayer (Game, "back", this, 0));
			AddLayer (new PlayerSelectLayer (Game, "menu", this, 1));
		}
	}
}

