using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class InterLayer : Layer
	{
		private SpriteFont m_font;

		public InterLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_font = sprites.GetFont ("SpriteFont1");

			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.NextLevel ();

			var ent = CreateEntity ();
			ent.Register (new Text (m_font));
			ent.Register (new Drawable (new Vector2 (420, 270)));
			ent.Register (new Clickable (new Rectangle (420, 270, 200, 22)));
			ent.Get<Clickable> ().LeftButtonPressed += OnAction;
			ent.Get<Clickable> ().TouchPressed += OnAction;

			if (gameState.IsGameEnd) {
				ent.Get<Text> ().Message = "New game";
			} else {
				ent.Get<Text> ().Message = "Next level";
			}
		}

		private void OnAction(Point _)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			if (gameState.IsGameEnd)
				director.ActivateScene ("main");
			else
				director.ActivateScene ("game");
		}
	}
}

