using System;
using System.Collections.Generic;

namespace mogate
{
	public class Entity
	{
		private Dictionary<string, IAspect> m_aspects = new Dictionary<string, IAspect>();

		public void RegisterAspect(IAspect aspect)
		{
			m_aspects.Add (aspect.Name, aspect);
		}

		public IAspect GetAspect(string name)
		{
			return m_aspects [name];
		}
	}
}

