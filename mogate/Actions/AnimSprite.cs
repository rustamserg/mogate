using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class AnimSprite : IAction
	{
		Entity m_entity;
		float m_duration;
		int m_spent;
		Sprite2D m_sprite;

		public AnimSprite (Entity entity, Sprite2D sprite, float duration)
		{
			m_entity = entity;
			m_duration = duration;
			m_sprite = sprite;
		}

		public AnimSprite (Entity entity, float duration)
		{
			m_entity = entity;
			m_duration = duration;
			m_sprite = m_entity.Get<Drawable> ().Sprite;
		}

		public bool Execute(GameTime gameTime)
		{
			m_spent += gameTime.ElapsedGameTime.Milliseconds;

			if (m_spent < m_duration) {
				float t = m_spent / m_duration;
				m_entity.Get<Drawable> ().Sprite = m_sprite;
				m_entity.Get<Drawable> ().FrameId = (int)(t * (m_entity.Get<Drawable> ().Sprite.Frames - 1));
				return false;
			} else {
				m_spent = 0;
				return true;
			}
		}
	}
}

