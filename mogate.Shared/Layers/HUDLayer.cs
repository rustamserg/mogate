using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class HUDLayer : Layer
	{
		Sprite2D m_life;

		public HUDLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			m_life = sprites.GetSprite ("health_01");

			var infoEnt = CreateEntity ();
			infoEnt.Register (new Execute ());
			infoEnt.Register (new Text (sprites.GetFont ("SpriteFont1")));
			infoEnt.Register (new Drawable (new Vector2 (Globals.CELL_WIDTH,
				Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT), Color.Green));

			var infoMsg = new Loop (new ActionEntity(infoEnt, (_) => {
				UpdateInfoMessage (infoEnt);
			}));
			infoEnt.Get<Execute> ().Add (infoMsg, "info_loop");

			var feedbackEnt = CreateEntity ("hud_feedback");
			feedbackEnt.Register (new Execute ());
			feedbackEnt.Register (new Text (sprites.GetFont ("SpriteFont1")));
			feedbackEnt.Register (new Drawable (new Vector2 (25 * Globals.CELL_WIDTH,
				Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT), Color.Red));
		}

		public void FeedbackMessage(string message, int duration = 1000)
		{
			var feedbackEnt = GetEntityByTag ("hud_feedback");
			var seq = new Sequence ();
			seq.Add (new Timeline(new ActionEntity (feedbackEnt, (_) => {
				feedbackEnt.Get<Text> ().Message = message;
			}), duration));
			seq.Add (new ActionEntity (feedbackEnt, (_) => {
				feedbackEnt.Get<Text> ().Message = string.Empty;
			}));
			feedbackEnt.Get<Execute> ().AddNew (seq);
		}

		private void UpdateInfoMessage(Entity infoEnt)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var player = Scene.GetLayer("player").GetEntityByTag("player");

			infoEnt.Get<Text>().Message = string.Format ("Player: {0} Stage: {1} HP: {2} Money: {3} Time: {4:D2}:{5:D2}:{6:D2}",
				gameState.PlayerName, gameState.Level + 1, player.Get<Health>().HP, player.Get<Consumable<ConsumableTypes>>().Amount(ConsumableTypes.Money),
				gameState.Playtime.Hours, gameState.Playtime.Minutes, gameState.Playtime.Seconds);
		}

		protected override void OnPostDraw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var player = Scene.GetLayer("player").GetEntityByTag("player");

			for (int i = 0; i < Math.Ceiling((float)player.Get<Health> ().HP/Globals.HEALTH_PACK); i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_life.Texture, drawPos, m_life.GetFrameRect (0), Color.White);
			}

			if (player.Has<Armor> ()) {
				var armorDrawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, 10 * Globals.CELL_HEIGHT);
				int armorId = player.Get<Armor> ().ArchetypeID;
				var armorSpriteName = string.Format ("armor_{0:D2}", Archetypes.Armors [armorId] ["sprite_index"]);
				var armorSprite = sprites.GetSprite (armorSpriteName);
				spriteBatch.Draw (armorSprite.Texture, armorDrawPos, armorSprite.GetFrameRect (0), Color.White);
			}

			var weaponDrawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, 11 * Globals.CELL_HEIGHT);
			int weaponId = player.Get<Attack> ().ArchetypeID;
			string weaponSpriteName = string.Format ("weapon_{0:D2}", Archetypes.Weapons [weaponId] ["sprite_index"]);
			var weaponSprite = sprites.GetSprite (weaponSpriteName);
			spriteBatch.Draw(weaponSprite.Texture, weaponDrawPos, weaponSprite.GetFrameRect (0), Color.White);
		}
	}
}

