using System;
using System.Collections.Generic;

namespace mogate
{
	public class Entity
	{
		private Dictionary<Type, IAspect> m_aspects = new Dictionary<Type, IAspect>();

		public void Register(IAspect aspect)
		{
			m_aspects[aspect.Behavior] = aspect;
		}

		public T Get<T>()
		{
			return (T)m_aspects [typeof(T)];
		}
	}
}

