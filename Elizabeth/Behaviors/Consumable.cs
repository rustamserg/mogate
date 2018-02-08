using System;
using System.Collections.Generic;

namespace Elizabeth
{
    public enum ConsumeOp { Add, Remove };

	public class Consumable<T> : IBehavior
	{
		public Type Behavior { get { return typeof(Consumable<T>); } }

		private Dictionary<T, int> m_amounts;
		private Action<ConsumeOp, T, int> m_onChanged;

		public Consumable ()
		{
			m_amounts = new Dictionary<T, int> ();
		}

		public Consumable (Action<ConsumeOp, T, int> onChanged)
		{
			m_amounts = new Dictionary<T, int> ();
			m_onChanged = onChanged;
		}

		public void Refill(T type, int amount)
		{
			if (!m_amounts.ContainsKey (type))
				m_amounts.Add (type, 0);

			m_amounts[type] += amount;

			if (m_onChanged != null)
				m_onChanged (ConsumeOp.Add, type, m_amounts[type]);
		}

		public bool TryConsume(T type, int amount)
		{
			if (!m_amounts.ContainsKey(type) || (m_amounts[type] - amount) < 0)
				return false;

			m_amounts[type] -= amount;

			if (m_onChanged != null)
				m_onChanged (ConsumeOp.Remove, type, m_amounts[type]);

			return true;
		}

		public int Amount(T type)
		{
			return m_amounts.ContainsKey(type) ? m_amounts [type] : 0;
		}
	}
}

