using System;
using Microsoft.Xna.Framework;
using Elizabeth;

namespace mogate
{
	public class TriggerEntity : IAction
	{
		Entity m_from;
		Entity m_to;

		public TriggerEntity (Entity from, Entity to)
		{
			m_from = from;
			m_to = to;
		}

		public bool Execute(GameTime gameTime)
		{
			if (m_to.Has<Triggerable> ()) {
				int dist = Utils.Dist (m_from.Get<Position> ().MapPos, m_to.Get<Position> ().MapPos);
				if (dist <= m_to.Get<Triggerable> ().Distance)
					m_to.Get<Triggerable> ().Trigger (m_from, m_to);
			}
			return true;
		}
	}
}

