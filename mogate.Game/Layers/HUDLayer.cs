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
		}

		protected override void OnPostDraw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			var player = Scene.GetLayer("player").GetEntityByTag("player");

			for (int i = 0; i < Math.Ceiling((float)player.Get<Health> ().HP/Globals.HUD_HEALTH_SIZE); i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_life.Texture, drawPos, m_life.GetFrameRect (0), Color.White);
			}
			for (int i = 0; i < Math.Ceiling((float)player.Get<Armor> ().Value/Globals.HUD_ARMOR_SIZE); i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, 300 + i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_armor.Texture, drawPos, m_armor.GetFrameRect (0), Color.White);
			}
			for (int i = 0; i < player.Get<Consumable<ConsumableItems>> ().Amount(ConsumableItems.Trap); i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, 450 + i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_trap.Texture, drawPos, m_trap.GetFrameRect (0), Color.White);
			}
		}
	}
}

