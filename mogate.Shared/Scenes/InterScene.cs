using System;
using Microsoft.Xna.Framework;
using Elizabeth;

namespace mogate
{
	public class InterScene : Scene
	{
		public InterScene (Game game, string name) : base(game, name, Globals.VIEWPORT_WIDTH, Globals.VIEWPORT_HEIGHT)
		{
		}

		protected override void LoadLayers()
		{
			AddLayer (new MenuBackgroundLayer (Game, "back", this, 0));
			AddLayer (new InterLayer (Game, "inter", this, 1));
		}
	}
}

