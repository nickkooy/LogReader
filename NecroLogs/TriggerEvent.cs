using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroLogs
{
	public abstract class TriggerEvent
	{
		public int TriggerId { get; private set; }

		public TriggerEvent(int triggerId)
		{
			TriggerId = triggerId;
		}

		public abstract void Triggered();
	}
}
