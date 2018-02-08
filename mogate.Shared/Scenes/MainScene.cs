using Elizabeth;
using Microsoft.Xna.Framework;

namespace mogate
{
    public class MainScene: Scene
	{
		public MainScene (Game game, string name) : base(game, name, Globals.VIEWPORT_WIDTH, Globals.VIEWPORT_HEIGHT)
		{
		}

		protected override void LoadLayers()
		{
			AddLayer (new MenuBackgroundLayer (Game, "back", this, 0));
			AddLayer (new MenuLayer (Game, "menu", this, 1));
		}
	}
}

