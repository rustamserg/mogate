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
			AddLayer (new MapGridLayer (Game, "map", this, 0));
			AddLayer (new ItemsLayer (Game, "items", this, 1));
			AddLayer (new PlayerLayer (Game, "player", this, 2));
			AddLayer (new MonstersLayer (Game, "monsters", this, 3));
			AddLayer (new EffectsLayer (Game, "effects", this, 4));
			#if !__IOS__
			AddLayer (new FogLayer (Game, "fog", this, 5));
			#endif
			AddLayer (new HUDLayer (Game, "hud", this, 6));
		}

		protected override void OnActivated()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.CountPlaytime = true;
		}

		protected override void OnDeactivated()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.CountPlaytime = false;
		}
	}
}

