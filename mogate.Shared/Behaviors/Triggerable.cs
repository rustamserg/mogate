using System;
using Elizabeth;

namespace mogate
{
	public class Triggerable : IBehavior
	{
		public Type Behavior { get { return typeof(Triggerable); } }

		private Action<Entity> m_onTrigger;
		public int Distance { get; private set; }

		public Triggerable(Action<Entity> onTrigger, int distance = 0)
		{
			m_onTrigger = onTrigger;
			Distance = distance;
		}

		public void Trigger(Entity from, Entity to)
		{
			if (m_onTrigger != null)
				m_onTrigger (from);
		}
	}
}

