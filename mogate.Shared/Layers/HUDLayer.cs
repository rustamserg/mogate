using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class HUDLayer : Layer
	{
		Sprite2D m_life;
		Sprite2D m_armor;
		Sprite2D m_trap;

		public HUDLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			m_life = sprites.GetSprite ("items_life");
			m_armor = sprites.GetSprite ("items_shield");
			m_trap = sprites.GetSprite ("effects_fire");

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
			feedbackEnt.Register (new Drawable (new Vector2 (20 * Globals.CELL_WIDTH,
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

			infoEnt.Get<Text>().Message = string.Format ("Stage: {0} HP: {1} Armor: {2} Time: {3:D2}:{4:D2}:{5:D2}",
				gameState.Level + 1, player.Get<Health>().HP, player.Get<Armor>().Value,
				gameState.Playtime.Hours, gameState.Playtime.Minutes, gameState.Playtime.Seconds);
		}

		/*protected override void OnPostDraw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			var player = Scene.GetLayer("player").GetEntityByTag("player");

			for (int i = 0; i < Math.Ceiling((float)player.Get<Health> ().HP/Globals.HEALTH_PACK); i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_life.Texture, drawPos, m_life.GetFrameRect (0), Color.White);
			}
			for (int i = 0; i < Math.Ceiling((float)player.Get<Armor> ().Value/Globals.ARMOR_PACK); i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, 300 + i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_armor.Texture, drawPos, m_armor.GetFrameRect (0), Color.White);
			}
			for (int i = 0; i < player.Get<Consumable<ConsumableItems>> ().Amount(ConsumableItems.Trap); i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, 450 + i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_trap.Texture, drawPos, m_trap.GetFrameRect (0), Color.White);
			}
		}*/
	}
}

