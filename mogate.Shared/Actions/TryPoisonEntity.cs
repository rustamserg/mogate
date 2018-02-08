using Elizabeth;
using Microsoft.Xna.Framework;

namespace mogate
{
    public class TryPoisonEntity : IAction
	{
		Entity m_attacker;
		Entity m_defender;

		public TryPoisonEntity (Entity attacker, Entity defender)
		{
			m_attacker = attacker;
			m_defender = defender;
		}

		public bool Execute(GameTime gameTime)
		{
			if (m_defender.Has<Poisonable> ())
				m_defender.Get<Poisonable> ().TryPoison (m_attacker, m_defender);

			return true;
		}
	}
}

