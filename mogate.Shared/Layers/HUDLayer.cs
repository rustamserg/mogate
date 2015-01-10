using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class HUDLayer : Layer
	{
		Sprite2D m_life;
		Sprite2D m_antitod;

		public HUDLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			m_life = sprites.GetSprite ("health_01");
			m_antitod = sprites.GetSprite ("antitod_potion_01");

			var infoEnt = CreateEntity ();
			infoEnt.Register (new Execute ());
			var infoMsg = new Loop (new ActionEntity(infoEnt, (_) => {
				UpdateInfoMessage (infoEnt);
			}));
			infoEnt.Get<Execute> ().Add (infoMsg, "info_loop");

			var feedbackEnt = CreateEntity ("hud_feedback");
			feedbackEnt.Register (new Execute ());
			feedbackEnt.Register (new Text (sprites.GetFont ("SpriteFont1")));
			feedbackEnt.Register (new Drawable (new Vector2 (570, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT), Color.Yellow));

			var hudIcon = CreateEntity ();
			hudIcon.Register (new Sprite (sprites.GetSprite("health_01")));
			hudIcon.Register (new Drawable (new Vector2 (0, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT)));

			hudIcon = CreateEntity ();
			hudIcon.Register (new Sprite (sprites.GetSprite("money_01")));
			hudIcon.Register (new Drawable (new Vector2 (100, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT)));

			hudIcon = CreateEntity ();
			hudIcon.Register (new Sprite (sprites.GetSprite("weapon_01")));
			hudIcon.Register (new Drawable (new Vector2 (200, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT)));

			hudIcon = CreateEntity ();
			hudIcon.Register (new Sprite (sprites.GetSprite("armor_01")));
			hudIcon.Register (new Drawable (new Vector2 (300, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT)));

			var textMsg = CreateEntity ("hud_health");
			textMsg.Register (new Text(sprites.GetFont ("SpriteFont1")));
			textMsg.Register (new Drawable (new Vector2 (40, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT), Color.Green));

			textMsg = CreateEntity ("hud_weapon");
			textMsg.Register (new Text(sprites.GetFont ("SpriteFont1")));
			textMsg.Register (new Drawable (new Vector2 (240, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT), Color.Green));

			textMsg = CreateEntity ("hud_money");
			textMsg.Register (new Text(sprites.GetFont ("SpriteFont1")));
			textMsg.Register (new Drawable (new Vector2 (140, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT), Color.Green));

			textMsg = CreateEntity ("hud_armor");
			textMsg.Register (new Text(sprites.GetFont ("SpriteFont1")));
			textMsg.Register (new Drawable (new Vector2 (340, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT), Color.Green));

			textMsg = CreateEntity ("hud_time");
			textMsg.Register (new Text(sprites.GetFont ("SpriteFont1")));
			textMsg.Register (new Drawable (new Vector2 (440, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT), Color.Green));
		}

		public void FeedbackMessage(string message, Color color, int duration = 1000)
		{
			var feedbackEnt = GetEntityByTag ("hud_feedback");
			var seq = new Sequence ();
			seq.Add (new ActionEntity (feedbackEnt, (_) => {
				feedbackEnt.Get<Drawable> ().DrawColor = color;
			}));
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

			var textMsg = GetEntityByTag ("hud_health");
			textMsg.Get<Text> ().Message = player.Get<Health> ().HP.ToString();

			textMsg = GetEntityByTag ("hud_money");
			textMsg.Get<Text> ().Message = player.Get<Consumable<ConsumableTypes>> ().Amount (ConsumableTypes.Money).ToString ();

			textMsg = GetEntityByTag ("hud_weapon");
			textMsg.Get<Text> ().Message = player.Get<Attack> ().Damage.ToString();

			textMsg = GetEntityByTag ("hud_armor");
			if (player.Has<Armor> ()) {
				textMsg.Get<Text> ().Message = player.Get<Armor> ().Defence.ToString ();
			} else {
				textMsg.Get<Text> ().Message = "none";
			}

			textMsg = GetEntityByTag ("hud_time");
			textMsg.Get<Text> ().Message = string.Format ("{0:D2}:{1:D2}:{2:D2}", gameState.Playtime.Hours, gameState.Playtime.Minutes, gameState.Playtime.Seconds);
		}

		protected override void OnPostDraw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var player = Scene.GetLayer("player").GetEntityByTag("player");

			for (int i = 0; i < Math.Ceiling((float)player.Get<Health> ().HP/Globals.HEALTH_PACK); i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_life.Texture, drawPos, m_life.GetFrameRect (0), Color.White);
			}
			for (int i = 0; i < player.Get<Consumable<ConsumableTypes>>().Amount(ConsumableTypes.Antitod); i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, (i + 15)  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_antitod.Texture, drawPos, m_antitod.GetFrameRect (0), Color.White);
			}

			if (player.Has<Armor> ()) {
				var armorDrawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, 20 * Globals.CELL_HEIGHT);
				int armorId = player.Get<Armor> ().ArchetypeID;
				var armorSpriteName = string.Format ("armor_{0:D2}", Archetypes.Armors [armorId] ["sprite_index"]);
				var armorSprite = sprites.GetSprite (armorSpriteName);
				spriteBatch.Draw (armorSprite.Texture, armorDrawPos, armorSprite.GetFrameRect (0), Color.White);
			}

			var weaponDrawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, 21 * Globals.CELL_HEIGHT);
			int weaponId = player.Get<Attack> ().ArchetypeID;
			string weaponSpriteName = string.Format ("weapon_{0:D2}", Archetypes.Weapons [weaponId] ["sprite_index"]);
			var weaponSprite = sprites.GetSprite (weaponSpriteName);
			spriteBatch.Draw(weaponSprite.Texture, weaponDrawPos, weaponSprite.GetFrameRect (0), Color.White);
		}
	}
}

