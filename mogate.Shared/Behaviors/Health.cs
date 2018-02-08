using Elizabeth;
using System;

namespace mogate
{
    public class Health : IBehavior
	{
		public Type Behavior { get { return typeof(Health); } }

		public int MaxHP { get; private set; }

		private int m_hp;
		private Action m_onChanged;

		public Health(int hp, int maxhp, Action onChanged)
		{
			m_hp = hp;
			MaxHP = maxhp;
			m_onChanged = onChanged;
		}

		public Health(int hp, Action onChanged = null)
		{
			m_hp = hp;
			MaxHP = hp;
			m_onChanged = onChanged;
		}

		public int HP {
			get { return m_hp; }
			set {
				var isChanged = (m_hp != value);
				m_hp = value;

				if (isChanged && m_onChanged != null)
					m_onChanged ();
			}
		}
	}
}

