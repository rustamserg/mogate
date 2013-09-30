using System;

namespace mogate
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

		public bool Execute()
		{
			m_action (m_entity);
			return true;
		}
	}
}

