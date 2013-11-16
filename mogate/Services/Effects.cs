using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public enum EffectState
	{
		Started,
		Finished
	};

	public interface IEffects
	{
		IEnumerable<Entity> GetEffects();
		void SpawnEffect (Point pos, string name, float duration);
		void SpawnEffect (Entity entity, string name, float duration);
	};

	public class Effects : GameComponent, IEffects
	{
		List<Entity> m_effects = new List<Entity>();

		public Effects (Game game) : base(game)
		{
		}

		public override void Update(GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			if (gameState.State == EState.GameStarted) {
				UpdateEffects (gameTime);
			}

			base.Update (gameTime);
		}


		public IEnumerable<Entity> GetEffects()
		{
			return new List<Entity> (m_effects);
		}

		public void SpawnEffect(Point pos, string name, float duration)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var ent = new Entity ();
			ent.Register (new Execute ());
			ent.Register (new State<EffectState> (EffectState.Started));
			ent.Register (new Drawable (sprites.GetSprite ("effects"), name,
				new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));

			var seq = new Sequence ();
			seq.Add (new AnimSprite(ent, name, duration));
			seq.Add (new ActionEntity (ent, (_) => {
				ent.Get<State<EffectState>> ().EState = EffectState.Finished;
			}));

			ent.Get<Execute> ().Add (seq);
			m_effects.Add (ent);
		}

		public void SpawnEffect(Entity entity, string name, float duration)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var pos = entity.Get<Position> ().MapPos;

			var ent = new Entity ();
			ent.Register (new Execute ());
			ent.Register (new State<EffectState> (EffectState.Started));
			ent.Register (new Drawable (sprites.GetSprite ("effects"), name,
				new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));

			var seq = new Sequence ();
			var spawn = new Spawn ();

			spawn.Add (new AnimSprite(ent, name, duration));
			spawn.Add (new FollowSprite (ent, entity, duration));

			seq.Add (spawn);
			seq.Add (new ActionEntity (ent, (_) => {
				ent.Get<State<EffectState>> ().EState = EffectState.Finished;
			}));

			ent.Get<Execute> ().Add (seq);
			m_effects.Add (ent);
		}

		private void UpdateEffects(GameTime gameTime)
		{
			foreach (var sfx in m_effects)
				sfx.Get<Execute> ().Update (gameTime);

			m_effects.RemoveAll (e => e.Get<State<EffectState>> ().EState == EffectState.Finished);
		}
	}
}

