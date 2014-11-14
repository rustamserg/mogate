using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;


namespace mogate
{
	public class ItemsLayer : Layer
	{
		Random m_rand = new Random(DateTime.UtcNow.Millisecond);

		public ItemsLayer(Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var mapGrid = world.GetLevel(gameState.Level);

			foreach (var room in mapGrid.GetRooms()) {
				var pos = new Point (room.Pos.X + m_rand.Next (room.Width), room.Pos.Y + m_rand.Next (room.Height));

				var ent = CreateEntity ();
				ent.Register (new Drawable (sprites.GetSprite ("items_barrel"),
					new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Position (pos.X, pos.Y));
				ent.Register (new Health (10));
				ent.Register (new Attackable ((attacker) => OnAttacked(ent, attacker)));
				ent.Register (new PointLight (4));
			}

			if (gameState.Level == Globals.MAX_LEVELS - 1) {
				var tree = CreateEntity ();
				var treeRoom = mapGrid.GetRooms ().First ();
				var pos = new Point (treeRoom.Pos.X + m_rand.Next (treeRoom.Width), treeRoom.Pos.Y + m_rand.Next (treeRoom.Height));

				tree.Register (new Position (pos.X, pos.Y));
				tree.Register (new Health (10));
				tree.Register (new PointLight (5));
				tree.Register (new Attackable ((attacker) => OnAttacked(tree, attacker)));
				tree.Register (new Drawable (sprites.GetSprite("items_artefact"), new Vector2 (pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
			}
		}

		void OnAttacked (Entity item, Entity attacker)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			effects.AttachEffect (item, "effects_damage", 400);

			if (item.Get<Health> ().HP == 0) {
				RemoveEntityByTag (item.Tag);
		
				var mp = item.Get<Position> ().MapPos;
				var ent = CreateEntity ();
				ent.Register (new Drawable (sprites.GetSprite ("items_life"),
					new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Position (mp.X, mp.Y));
				ent.Register (new Triggerable (1, (from) => OnTriggered(ent, from)));
			}
		}

		void OnTriggered (Entity item, Entity from)
		{
			if (from.Has<Health> ()) {
				from.Get<Health> ().HP = Math.Min (from.Get<Health> ().MaxHP,
					from.Get<Health> ().HP + 20);
			}
			RemoveEntityByTag (item.Tag);
		}
	}
}

