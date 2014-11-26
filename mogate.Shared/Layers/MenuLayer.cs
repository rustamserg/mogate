using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mogate
{
	public class MenuLayer : Layer
	{
		private SpriteFont m_font;

		public MenuLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_font = sprites.GetFont ("SpriteFont1");
		}

		protected override void OnPostUpdate(GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			if (gameState.IsLoaded) {
				if (!GetAllEntities().Any(ent => ent.Tag == "new_game_btn")) {
					var newGameBtn = CreateEntity ("new_game_btn");
					newGameBtn.Register (new Text (m_font, "Click to start new game"));
					newGameBtn.Register (new Drawable (new Vector2 (420, 270)));
					newGameBtn.Register (new Clickable (new Rectangle (420, 270, 100, 22)));
					newGameBtn.Get<Clickable> ().LeftButtonPressed += StartNewGame;
					if (gameState.Level > 0) {
						var cntGameBtn = CreateEntity ();
						cntGameBtn.Register (new Text (m_font, "Click to continue last game"));
						cntGameBtn.Register (new Drawable (new Vector2 (420, 300)));
						cntGameBtn.Register (new Clickable (new Rectangle (420, 300, 100, 22)));
						cntGameBtn.Get<Clickable> ().LeftButtonPressed += ContinueGame;
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

