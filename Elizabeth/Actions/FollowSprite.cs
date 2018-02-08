using Microsoft.Xna.Framework;

namespace Elizabeth
{
    public class FollowSprite : IAction
	{
		Entity m_entity;
		Entity m_follow;
		float m_duration;
		int m_spent;

		public FollowSprite (Entity entity, Entity follow, float duration)
		{
			m_entity = entity;
			m_follow = follow;
			m_duration = duration;
		}

		public bool Execute(GameTime gameTime)
		{
			m_spent += gameTime.ElapsedGameTime.Milliseconds;

			if (m_spent < m_duration) {
				m_entity.Get<Drawable>().DrawPos = m_follow.Get<Drawable>().DrawPos;
				return false;
			}
			return true;
		}
	}
}

