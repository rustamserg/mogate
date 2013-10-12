using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class AnimSprite : IAction
	{
		Entity m_entity;
		float m_duration;
		string m_frameName;
		int m_spent;

		public AnimSprite (Entity entity, string frameName, float duration)
		{
			m_entity = entity;
			m_duration = duration;
			m_frameName = frameName;
		}

		public bool Execute(GameTime gameTime)
		{
			m_spent += gameTime.ElapsedGameTime.Milliseconds;

			if (m_spent < m_duration) {
				float t = m_spent / m_duration;
				m_entity.Get<Drawable> ().FrameName = m_frameName;
				m_entity.Get<Drawable> ().FrameId = (int)(t * (m_entity.Get<Drawable> ().FrameCount - 1));
				return false;
			} else {
				m_spent = 0;
				return true;
			}
		}
	}
}

