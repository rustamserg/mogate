using System;
using System.Collections.Generic;

namespace mogate
{
	public class Entity
	{
		private Dictionary<Type, IBehavior> m_behaviors = new Dictionary<Type, IBehavior>();

		public void Register(IBehavior behavior)
		{
			m_behaviors[behavior.Behavior] = behavior;
		}

		public T Get<T>()
		{
			return (T)m_behaviors [typeof(T)];
		}
	}
}

