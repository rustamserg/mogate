using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

namespace mogate
{
	public class PlayerSelectLayer : Layer
	{
		private SpriteFont m_font;

		public PlayerSelectLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_font = sprites.GetFont ("SpriteFont1");

			var ent = CreateEntity ();
			ent.Register (new Text (m_font, "Choose a hero"));
			ent.Register (new Drawable (new Vector2 (410, 200)));

			for (int idx = 0; idx < Archetypes.Players.Length; ++idx) {
				var arch = Archetypes.Players [idx];

				ent = CreateEntity ();
				ent.Register (new Tag (idx));
				ent.Register (new Sprite (sprites.GetSprite (string.Format ("player_{0:D2}", arch ["sprite_index"]))));
				ent.Register (new Drawable (new Vector2 (400 + 50 * idx, 250)));
				ent.Register (new Clickable (new Rectangle (400 + 50 * idx, 250, 40, 40), ent));
				ent.Get<Clickable> ().OnLeftButtonPressed += OnPlayerSelect;
				ent.Get<Clickable> ().OnTouched += OnPlayerSelect;
			}

			ent = CreateEntity ("player_name");
			ent.Register (new Text (m_font));
			ent.Register (new Drawable (new Vector2 (420, 320)));

			ent = CreateEntity ("player_health");
			ent.Register (new Text (m_font));
			ent.Register (new Drawable (new Vector2 (420, 350)));

			ent = CreateEntity ("player_move_speed");
			ent.Register (new Text (m_font));
			ent.Register (new Drawable (new Vector2 (420, 380)));

			ent = CreateEntity ("player_skill");
			ent.Register (new Text (m_font));
			ent.Register (new Drawable (new Vector2 (420, 410)));

			ent = CreateEntity ();
			ent.Register (new Text (m_font, "Go to dangeon"));
			ent.Register (new Drawable (new Vector2 (410, 500)));
			ent.Register (new Clickable (new Rectangle (410, 500, 400, 40)));
			ent.Get<Clickable> ().OnLeftButtonPressed += OnAction;
			ent.Get<Clickable> ().OnTouched += OnAction;

			UpdateStats ();
		}

		private void OnPlayerSelect(Point _, Entity player)
		{
			int archIdx = player.Get<Tag> ().ID;
			var arch = Archetypes.Players [archIdx];

			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.ApplyArchetype (arch);

			UpdateStats ();
		}

		private void UpdateStats()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var ent = GetEntityByTag ("player_name");
			ent.Get<Text> ().Message = string.Format ("Name: {0}", gameState.PlayerName);

			ent = GetEntityByTag ("player_health");
			ent.Get<Text> ().Message = string.Format ("Health: {0}", gameState.PlayerHealth);

			ent = GetEntityByTag ("player_move_speed");
			ent.Get<Text> ().Message = string.Format ("Speed: {0}", GetMoveSpeedLabel (gameState.PlayerMoveSpeed));

			ent = GetEntityByTag ("player_skill");
			if (gameState.PlayerMoneyMultiplier > 1) {
				ent.Get<Text> ().Message = string.Format ("Money: x{0}", gameState.PlayerMoneyMultiplier);
			} else if (gameState.PlayerAttackMultiplier > 1) {
				ent.Get<Text> ().Message = string.Format ("Attack: x{0}", gameState.PlayerAttackMultiplier);
			} else if (gameState.PlayerPoisonMultiplier == 0) {
				ent.Get<Text> ().Message = "Immune to poison";
			} else {
				ent.Get<Text> ().Message = string.Format ("Range: x{0}", gameState.PlayerAttackDistance);
			}
		}

		private string GetMoveSpeedLabel(int moveSpeed)
		{
			// 300, 400, 200
			if (moveSpeed <= 200)
				return "fast";
			else if (moveSpeed <= 300)
				return "normal";
			else if (moveSpeed <= 400)
				return "slower";
			else
				return "slowest";
		}

		private void OnAction(Point _, Entity __)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			director.ActivateScene ("game");
		}
	}
}

