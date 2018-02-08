using Elizabeth;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mogate
{
    public class IFFSystem : IBehavior
	{
		public Type Behavior { get { return typeof(IFFSystem); } }

		public int Priority { get; private set; }
		public int ID { get; private set; }

		public IFFSystem (int id, int priority = 0)
		{
			ID = id;
			Priority = priority;
		}

		public bool IsFoe(Entity target)
		{
			if (target.Has<IFFSystem> ()) {
				return ID != target.Get<IFFSystem> ().ID;
			}
			return false;
		}

		public IEnumerable<Entity> GetFoes(IEnumerable<Entity> targets)
		{
			return from e in targets
					where IsFoe(e)
					orderby e.Get<IFFSystem>().Priority
					select e;
		}
	}
}

