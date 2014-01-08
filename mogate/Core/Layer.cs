using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class Layer : GameComponent
	{
		private Dictionary<int, Entity> m_entitiesByTag = new Dictionary<int, Entity>();
		private int m_id;

		public string Name { get; private set; }
		public int ZOrder { get; set; }

		public Layer(Game game, string name, int z) : base(game)
		{
			Name = name;
			ZOrder = z;
		}

		public Entity CreateEntity(int tag = 0)
		{
			var ent = new Entity(tag == 0 ? ++m_id : tag);
			m_entitiesByTag.Add (ent.Tag, ent);
			return ent;
		}

		public Entity GetEntityByTag(int tag)
		{
			return m_entitiesByTag [tag];
		}

		public IEnumerable<Entity> GetAllEntities()
		{
			return new List<Entity>(m_entitiesByTag.Values);
		}

		public void RemoveAllEntities()
		{
			m_entitiesByTag.Clear();
		}

		public void RemoveEntityByTag(int tag)
		{
			m_entitiesByTag.Remove (tag);
		}

		public override void Update(GameTime gameTime)
		{
			var iter = new List<Entity> (m_entitiesByTag.Values);
			foreach (var ent in iter) {
				if (ent.Has<Execute> ()) {
					ent.Get<Execute> ().Update (gameTime);
				}
			}

			OnPostUpdate (gameTime);
			base.Update (gameTime);
		}

		public void Draw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			foreach (var ent in m_entitiesByTag.Values) {
				if (!ent.Has<Drawable> ())
					continue;

				spriteBatch.Draw (ent.Get<Drawable>().Sprite.Texture,
					ent.Get<Drawable>().DrawPos,
					ent.Get<Drawable>().DrawRect,
					ent.Get<Drawable>().DrawColor);
			}

			OnPostDraw (spriteBatch, gameTime);
		}

		protected virtual void OnPostUpdate(GameTime gameTime)
		{
		}

		protected virtual void OnPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
		{
		}

		public virtual void OnActivated()
		{
		}

		public virtual void OnDeactivated()
		{
		}
	}
}

