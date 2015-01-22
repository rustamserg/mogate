using System;
using Microsoft.Xna.Framework;
using Elizabeth;

namespace mogate
{
	public class AttackEntity : IAction
	{
		Entity m_attacker;
		Entity m_defender;

		public AttackEntity (Entity attacker, Entity defender)
		{
			m_attacker = attacker;
			m_defender = defender;
		}

		public bool Execute(GameTime gameTime)
		{
			if (m_defender.Has<Attackable> ())
				m_defender.Get<Attackable> ().Attack (m_attacker, m_defender);

			return true;
		}
	}
}

