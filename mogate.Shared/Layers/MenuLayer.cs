using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

			if (gameState.DataState == SaveDataState.Ready) {
				if (!GetAllEntities().Any(ent => ent.Tag == "new_game_btn")) {
					var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

					var newGameBtn = CreateEntity ("new_game_btn");
					newGameBtn.Register (new Text (sprites.GetFont ("SpriteFont1"), "Start new game"));
					newGameBtn.Register (new Drawable (new Vector2 (420, 270)));
					newGameBtn.Register (new Clickable (new Rectangle (420, 270, 400, 40)));
					newGameBtn.Get<Clickable> ().OnLeftButtonPressed += StartNewGame;
					newGameBtn.Get<Clickable> ().OnTouched += StartNewGame;

					if (gameState.Level > 0) {
						var cntGameBtn = CreateEntity ();
						cntGameBtn.Register (new Text (sprites.GetFont ("SpriteFont1"), "Continue game"));
						cntGameBtn.Register (new Drawable (new Vector2 (420, 320)));
						cntGameBtn.Register (new Clickable (new Rectangle (420, 320, 400, 40)));
						cntGameBtn.Get<Clickable> ().OnLeftButtonPressed += ContinueGame;
						cntGameBtn.Get<Clickable> ().OnTouched += ContinueGame;
					}
				}
			}
		}

		void StartNewGame(Point _)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.NewGame ();

			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			director.ActivateScene ("game");
		}

		void ContinueGame(Point _)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			director.ActivateScene ("game");
		}
	}
}

