using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class Layer : GameComponent
	{
		private Dictionary<string, Entity> m_entitiesByTag = new Dictionary<string, Entity>();
		private int m_id;

		public string Name { get; private set; }
		public int ZOrder { get; set; }

		public Layer(Game game, string name, int z) : base(game)
		{
			Name = name;
			ZOrder = z;
		}

		public Entity CreateEntity(string tag = "")
		{
			var ent = new Entity(string.IsNullOrEmpty(tag) ? (++m_id).ToString() : tag);
			m_entitiesByTag.Add (ent.Tag, ent);
			return ent;
		}

		public Entity GetEntityByTag(string tag)
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

		public void RemoveEntityByTag(string tag)
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

				var drawAspect = ent.Get<Drawable> ();
				var drawColor = (drawAspect.DrawAlpha < 1.0f)
					? drawAspect.DrawColor * drawAspect.DrawAlpha : drawAspect.DrawColor;

				spriteBatch.Draw (drawAspect.Sprite.Texture,
					drawAspect.DrawPos,
					drawAspect.DrawRect,
					drawColor);
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

