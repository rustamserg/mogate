using Microsoft.Xna.Framework;
using System;

namespace Elizabeth
{
    public class ActionEntity : IAction
	{
		Action<Entity> m_action;
		Entity m_entity;

		public ActionEntity (Entity entity, Action<Entity> action)
		{
			m_entity = entity;
			m_action = action;
		}

		public bool Execute(GameTime gameTime)
		{
			m_action (m_entity);
			return true;
		}
	}
}

