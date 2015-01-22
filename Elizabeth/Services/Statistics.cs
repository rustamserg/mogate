using System;
using System.Collections.Generic;
using System.IO;

namespace Elizabeth
{
	public interface IStatistics
	{
		void EntityAdded(string entity);
		void EntityRemoved(string entity);

		void BehaviorRegistered(string entity, string behavior);
		void BehaviorFetched (string entity, string behavior);

		void Dump();
	}

	#if DEBUG
	public class Statistics : IStatistics
	{
		private object m_lock = new object();

		private Dictionary<string, int> m_behaviorRegistered = new Dictionary<string, int>();
		private Dictionary<string, int> m_behaviorFetched = new Dictionary<string, int>();

		public void EntityAdded(string entity)
		{
		}

		public void EntityRemoved(string entity)
		{
		}

		public void BehaviorRegistered(string entity, string behavior)
		{
			lock (m_lock) {
				if (!m_behaviorRegistered.ContainsKey(behavior)) {
					m_behaviorRegistered.Add (behavior, 0);
				}
				m_behaviorRegistered [behavior] = m_behaviorRegistered [behavior] + 1;
			}
		}

		public void BehaviorFetched (string entity, string behavior)
		{
			lock (m_lock) {
				if (!m_behaviorFetched.ContainsKey(behavior)) {
					m_behaviorFetched.Add (behavior, 0);
				}
				m_behaviorFetched [behavior] = m_behaviorFetched [behavior] + 1;
			}
		}

		public void Dump()
		{
			lock (m_lock) {
				using (StreamWriter sw = File.CreateText("/Users/rustam/Documents/behavior_registered.txt")) 
				{
					foreach (var beh in m_behaviorRegistered.Keys) {
						sw.WriteLine ("{0}, {1}", beh, m_behaviorRegistered [beh]);
					}
				}
				using (StreamWriter sw = File.CreateText("/Users/rustam/Documents/behavior_fetched.txt")) 
				{
					foreach (var beh in m_behaviorFetched.Keys) {
						sw.WriteLine ("{0}, {1}", beh, m_behaviorFetched [beh]);
					}
				}
			}
		}
	}
	#else
	public class Statistics : IStatistics
	{
		public void EntityAdded(string entity) {}
		public void EntityRemoved(string entity) {}
		public void BehaviorRegistered(string entity, string behavior) {}
		public void BehaviorFetched (string entity, string behavior) {}
		public void Dump() {}
	}
	#endif
}

