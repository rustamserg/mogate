using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class ItemsLayer : Layer
	{
		Random m_rand = new Random(DateTime.UtcNow.Millisecond);

		public ItemsLayer(Game game, string name, int z) : base(game, name, z)
		{
		}

		public override void OnActivated()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var mapGrid = world.GetLevel(gameState.Level);
			RemoveAllEntities ();

			foreach (var room in mapGrid.GetRooms()) {
				var pos = new Point (room.Pos.X + m_rand.Next (room.Width), room.Pos.Y + m_rand.Next (room.Height));

				var ent = CreateEntity ();
				ent.Register (new Drawable (sprites.GetSprite ("items_barrel"),
					new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
			}
		}
	}
}

