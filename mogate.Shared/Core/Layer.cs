using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mogate
{
	public class Layer : GameComponent
	{
		private Dictionary<string, Entity> m_entitiesByTag = new Dictionary<string, Entity>();
		private int m_id;
		private bool m_isActivated;

		public string Name { get; private set; }
		public int ZOrder { get; set; }
		public Scene Scene { get; private set; } 

		public Layer(Game game, string name, Scene scene, int z) : base(game)
		{
			Name = name;
			ZOrder = z;
			Scene = scene;
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

		public void RemoveEntityByTag(string tag)
		{
			m_entitiesByTag.Remove (tag);
		}

		public override void Update(GameTime gameTime)
		{
			var iter = new List<Entity> (m_entitiesByTag.Values);
			var mouse = Mouse.GetState ();

			foreach (var ent in iter) {
				if (!m_isActivated)
					continue;

				if (ent.Has<Execute> ()) {
					ent.Get<Execute> ().Update (gameTime);
				}
				if (ent.Has<Clickable> ()) {
					ent.Get<Clickable> ().HandleMouseInput (mouse);
				}
			}

			if (m_isActivated)
				OnPostUpdate (gameTime);

			base.Update (gameTime);
		}

		public void Draw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			OnPreDraw (spriteBatch, gameTime);

			foreach (var ent in m_entitiesByTag.Values) {

				if (!ent.Has<Drawable> ())
					continue;

				var drawAspect = ent.Get<Drawable> ();
				var drawColor = (drawAspect.DrawAlpha < 1.0f)
					? drawAspect.DrawColor * drawAspect.DrawAlpha : drawAspect.DrawColor;

				if (ent.Has<Sprite> ()) {
					var sprite = ent.Get<Sprite> ();
					spriteBatch.Draw (sprite.Image.Texture,
						drawAspect.DrawPos,
						sprite.DrawRect,
						drawColor);
				}
				if (ent.Has<Text> ()) {
					var text = ent.Get<Text> ();
					spriteBatch.DrawString (text.Font,
						text.Message,
						drawAspect.DrawPos,
						drawColor);
				}
			}

			OnPostDraw (spriteBatch, gameTime);
		}

		public void Activate()
		{
			m_isActivated = true;
			m_entitiesByTag.Clear();
			OnActivated ();
		}

		public void Deactivate()
		{
			OnDeactivated ();
			m_entitiesByTag.Clear();
			m_isActivated = false;
		}

		protected virtual void OnPostUpdate(GameTime gameTime)
		{
		}

		protected virtual void OnPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
		{
		}

		protected virtual void OnPreDraw(SpriteBatch spriteBatch, GameTime gameTime)
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

