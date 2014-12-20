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

			for (int i = 0; i < Globals.HALL_OF_FAME_SIZE; ++i) {
				var hofname = CreateEntity(string.Format("hof_name_{0}", i));
				hofname.Register (new Text (m_font));
				hofname.Register (new Drawable (new Vector2 (420, 320 + i * 25)));

				var hoftime = CreateEntity(string.Format("hof_time_{0}", i));
				hoftime.Register (new Text (m_font));
				hoftime.Register (new Drawable (new Vector2 (420 + 100, 320 + i * 25)));
			}

			if (gameState.IsGameEnd) {
				ent.Get<Text> ().Message = "New game";

				int idx = 0;
				foreach (var hof in gameState.HallOfFame) {
					var hofname = GetEntityByTag(string.Format("hof_name_{0}", idx));
					hofname.Get<Text> ().Message = hof.PlayerName;

					var hoftime = GetEntityByTag(string.Format("hof_time_{0}", idx++));
					var ts = TimeSpan.FromTicks (hof.TotalPlaytime);
					hoftime.Get<Text> ().Message = string.Format ("{0:D2}:{1:D2}:{2:D2}",
						ts.Hours, ts.Minutes, ts.Seconds);
				}
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

