using Elizabeth;
using Microsoft.Xna.Framework;
using System.Linq;

namespace mogate
{
    public class MenuLayer : Layer
	{
		public MenuLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var controller = CreateEntity ();
			controller.Register (new Execute ());
			controller.Get<Execute> ().Add (new Loop (new ActionEntity (controller, (_) => {
				Update (controller);
			})));
		}

		private void Update(Entity controller)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			if (gameState.IsLoaded) {
				if (!GetAllEntities().Any(ent => ent.Tag == "new_game_btn")) {
					var res = (IGameResources)Game.Services.GetService (typeof(IGameResources));

					var newGameBtn = CreateEntity ("new_game_btn");
					newGameBtn.Register (new Text (res.GetFont ("SpriteFont1"), "New game"));
					newGameBtn.Register (new Drawable (new Vector2 (440, 270)));
					newGameBtn.Register (new Clickable (new Rectangle (440, 270, 400, 40)));
					newGameBtn.Get<Clickable> ().OnTouched += StartNewGame;

					if (gameState.Level > 0) {
						var cntGameBtn = CreateEntity ();
						cntGameBtn.Register (new Text (res.GetFont ("SpriteFont1"), "Continue"));
						cntGameBtn.Register (new Drawable (new Vector2 (450, 320)));
						cntGameBtn.Register (new Clickable (new Rectangle (450, 320, 400, 40)));
						cntGameBtn.Get<Clickable> ().OnTouched += ContinueGame;
					}
				}
			}
		}

		void StartNewGame(Point _, Entity __)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.NewGame ();

			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			director.ActivateScene ("player_select");
		}

		void ContinueGame(Point _, Entity __)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.ContinueGame ();

			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			director.ActivateScene ("game");
		}
	}
}

