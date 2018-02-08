using System;
using System.Collections.Generic;

namespace Elizabeth
{
    public class Entity
	{
		private readonly IStatistics m_gameStats;
		private Dictionary<Type, IBehavior> m_behaviors = new Dictionary<Type, IBehavior>();
		public readonly string Tag;	

		public Entity(string tag, IStatistics gameStats)
		{
			m_gameStats = gameStats;
			Tag = tag;

			m_gameStats.EntityAdded (Tag);
		}

		public void OnRemoved()
		{
			if (Has<Execute> ())
				Get<Execute> ().Cancel ();

			m_gameStats.EntityRemoved (Tag);
		}

		public void Register(IBehavior behavior)
		{
			m_behaviors[behavior.Behavior] = behavior;
			m_gameStats.BehaviorRegistered (Tag, behavior.Behavior.ToString ());
		}

		public T Get<T>()
		{
			var behavior = m_behaviors [typeof(T)];
			m_gameStats.BehaviorFetched (Tag, behavior.Behavior.ToString ());

			return (T)behavior;
		}

		public bool Has<T>()
		{
			return m_behaviors.ContainsKey (typeof(T));
		}
	}
}

