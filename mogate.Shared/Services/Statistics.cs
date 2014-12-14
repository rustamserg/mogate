using System;

namespace mogate
{
	public interface IStatistics
	{
		void EntityAdded(string entity);
		void EntityRemoved(string entity);

		void BehaviorRegistered(string entity, string behavior);
		void BehaviorFetched (string entity, string behavior);
	}

	public class Statistics : IStatistics
	{
		public void EntityAdded(string entity)
		{
		}

		public void EntityRemoved(string entity)
		{
		}

		public void BehaviorRegistered(string entity, string behavior)
		{
		}

		public void BehaviorFetched (string entity, string behavior)
		{
		}
	}
}

