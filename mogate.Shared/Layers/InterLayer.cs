using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Elizabeth;

namespace mogate
{
	public class InterLayer : Layer
	{
		public InterLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var font = sprites.GetFont ("SpriteFont1");

			if (gameState.GameProgress == GameProgressState.InGame) {
				var ent = CreateEntity ();
				ent.Register (new Text (font, "Next level"));
				ent.Register (new Drawable (new Vector2 (450, 200)));
				ent.Register (new Clickable (new Rectangle (450, 200, 400, 40)));
				ent.Get<Clickable> ().OnTouched += OnAction;
			} else {
				var ent = CreateEntity ();
				ent.Register (new Text (font, "Back to menu"));
				ent.Register (new Drawable (new Vector2 (440, 550)));
				ent.Register (new Clickable (new Rectangle (440, 550, 400, 40)));
				ent.Get<Clickable> ().OnTouched += OnAction;

				int idx = 0;

				foreach (var hof in gameState.HallOfFame) {
					var hofsprite = CreateEntity ();
					hofsprite.Register (new Sprite (sprites.GetSprite (string.Format("player_{0:D2}", hof.PlayerSpriteID))));
					hofsprite.Register (new Drawable (new Vector2 (380, 320 + idx * 36)));

					var hofname = CreateEntity();
					hofname.Register (new Text (font, hof.PlayerName));
					hofname.Register (new Drawable (new Vector2 (380 + 40, 320 + idx * 36)));

					var ts = TimeSpan.FromTicks (hof.TotalPlaytime);
					var time = string.Format ("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);

					var hoftime = CreateEntity();
					hoftime.Register (new Text (font, time));
					hoftime.Register (new Drawable (new Vector2 (380 + 140, 320 + idx * 36)));

					++idx;
				}

				ent = CreateEntity ();
				ent.Register (new Text (font));

				if (gameState.GameProgress == GameProgressState.Death) {
					ent.Register (new Drawable (new Vector2 (380, 250)));
					ent.Get<Text> ().Message = string.Format ("Rest in peace {0}", gameState.PlayerName);
				} else {
					ent.Register (new Drawable (new Vector2 (380, 250)));
					ent.Get<Text> ().Message = string.Format ("{0} won the game", gameState.PlayerName);
				}
			}
		}

		private void OnAction(Point _, Entity __)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			if (gameState.GameProgress == GameProgressState.InGame) {
				director.ActivateScene ("game");
			} else {
				gameState.NewGame ();
				director.ActivateScene ("main");
			}
		}
	}
}

