using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Elizabeth;

namespace mogate
{
	public class EffectsLayer : Layer
	{
		public EffectsLayer(Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public void SpawnEffect(Point pos, string name, float duration)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var ent = CreateEntity ();
			ent.Register (new Execute ());
			ent.Register (new Sprite (sprites.GetSprite (name)));
			ent.Register (new Drawable (new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));

			var seq = new Sequence ();
			seq.Add (new AnimSprite(ent, duration));
			seq.Add (new ActionEntity (ent, (_) => {
				RemoveEntityByTag(ent.Tag);
			}));

			ent.Get<Execute> ().Add (seq);
		}

		public void AttachEffect(Entity entity, string name, float duration)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var pos = entity.Get<Position> ().MapPos;

			var ent = CreateEntity ();
			ent.Register (new Execute ());
			ent.Register (new Sprite (sprites.GetSprite (name)));
			ent.Register (new Drawable (new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));

			var seq = new Sequence ();
			var spawn = new Spawn ();

			spawn.Add (new AnimSprite(ent, duration));
			spawn.Add (new FollowSprite (ent, entity, duration));

			seq.Add (spawn);
			seq.Add (new ActionEntity (ent, (_) => {
				RemoveEntityByTag(ent.Tag);
			}));

			ent.Get<Execute> ().Add (seq);
		}

	}
}

