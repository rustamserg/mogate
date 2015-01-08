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
			ent.Register (new Drawable (new Vector2 (450, 200)));
			ent.Register (new Clickable (new Rectangle (450, 200, 400, 40)));
			ent.Get<Clickable> ().OnLeftButtonPressed += OnAction;
			ent.Get<Clickable> ().OnTouched += OnAction;

			if (gameState.IsGameEnd) {
				ent.Get<Text> ().Message = "New game";

				int idx = 0;

				foreach (var hof in gameState.HallOfFame) {
					var hofsprite = CreateEntity ();
					hofsprite.Register (new Sprite (sprites.GetSprite (string.Format("player_{0:D2}", hof.PlayerSpriteID))));
					hofsprite.Register (new Drawable (new Vector2 (380, 320 + idx * 36)));

					var hofname = CreateEntity();
					hofname.Register (new Text (m_font, hof.PlayerName));
					hofname.Register (new Drawable (new Vector2 (380 + 40, 320 + idx * 36)));

					var ts = TimeSpan.FromTicks (hof.TotalPlaytime);
					var time = string.Format ("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);

					var hoftime = CreateEntity();
					hoftime.Register (new Text (m_font, time));
					hoftime.Register (new Drawable (new Vector2 (380 + 140, 320 + idx * 36)));

					++idx;
				}
			} else {
				ent.Get<Text> ().Message = "Next level";
			}
		}

		private void OnAction(Point _, Entity __)
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

