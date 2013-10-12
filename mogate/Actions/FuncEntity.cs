using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class FuncEntity : IAction
	{
		Entity m_entity;
		Func<Entity, bool> m_func;

		public FuncEntity (Entity entity, Func<Entity, bool> func)
		{
			m_entity = entity;
			m_func = func;
		}

		public bool Execute(GameTime gameTime)
		{
			return m_func(m_entity);
		}
	}
}

