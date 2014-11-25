using System;
using System.Collections.Generic;

namespace mogate
{
	public class Entity
	{
		private Dictionary<Type, IBehavior> m_behaviors = new Dictionary<Type, IBehavior>();
		public readonly string Tag;	

		public Entity(string tag = "")
		{
			Tag = tag;
		}

		public void Register(IBehavior behavior)
		{
			m_behaviors[behavior.Behavior] = behavior;
		}

		public T Get<T>()
		{
			return (T)m_behaviors [typeof(T)];
		}

		public bool Has<T>()
		{
			return m_behaviors.ContainsKey (typeof(T));
		}
	}
}

