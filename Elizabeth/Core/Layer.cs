using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Linq;

namespace Elizabeth
{
    public class Layer
	{
		private Dictionary<string, Entity> m_entitiesByTag = new Dictionary<string, Entity>();
		private List<Entity> m_orderedEntity = new List<Entity> ();

		private int m_id;
		private bool m_isActivated;
		private Vector2 m_screenToWorld;

		public string Name { get; private set; }
		public int ZOrder { get; set; }
		public Scene Scene { get; private set; }
		protected Game Game { get; private set; }

		public Layer(Game game, string name, Scene scene, int z)
		{
			Name = name;
			ZOrder = z;
			Scene = scene;
			Game = game;

			float horScaling = (float)scene.ViewportWidth / Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
			float verScaling = (float)scene.ViewportHeight / Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
			m_screenToWorld = new Vector2 (horScaling, verScaling);
		}

		public Entity CreateEntity(string tag = "")
		{
			var gameStats = (IStatistics)Game.Services.GetService (typeof(IStatistics));

			if (string.IsNullOrEmpty (tag)) {
				tag = string.Format ("{0}:{1}", Name, (++m_id).ToString ());
			}
			var ent = new Entity(tag, gameStats);

			m_entitiesByTag.Add (ent.Tag, ent);
			m_orderedEntity = new List<Entity> (m_entitiesByTag.Values);
			m_orderedEntity.OrderBy (la => la.Has<Drawable>() ? la.Get<Drawable>().ZOrder : int.MaxValue);
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
			m_entitiesByTag [tag].OnRemoved ();

			m_entitiesByTag.Remove (tag);
			m_orderedEntity.RemoveAll (ent => ent.Tag == tag);
		}

		public void Update(GameTime gameTime, MouseState mouse, TouchCollection touches, KeyboardState keyboard)
		{
			var iter = new List<Entity> (m_entitiesByTag.Values);

			foreach (var ent in iter) {
				if (!m_isActivated)
					continue;

				if (ent.Has<Execute> ()) {
					ent.Get<Execute> ().Update (gameTime);
				}
				if (ent.Has<Clickable> ()) {
					ent.Get<Clickable> ().HandleMouseInput (mouse, m_screenToWorld);
					ent.Get<Clickable> ().HandleTouchInput (touches);
                    ent.Get<Clickable>().HandleKeyboardInput(keyboard);
				}
			}
		}

		public void Draw (SpriteBatch spriteBatch, GameTime gameTime)
		{		
			foreach (var ent in m_orderedEntity) {

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
			m_orderedEntity.Clear ();
			OnActivated ();
		}

		public void Deactivate()
		{
			OnDeactivated ();
			m_entitiesByTag.Clear();
			m_orderedEntity.Clear ();
			m_isActivated = false;
		}			

		public virtual void OnActivated()
		{
		}

		public virtual void OnDeactivated()
		{
		}

		protected virtual void OnPostDraw (SpriteBatch spriteBatch, GameTime gameTime)
		{
		}
	}
}

