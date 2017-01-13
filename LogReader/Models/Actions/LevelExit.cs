using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NecroLogs;

namespace LogReader
{
	public class LevelExit : TriggerAction
	{
		public LevelExit(string id)
			:base (id)
		{ }

		public override void Triggered()
		{
			throw new NotImplementedException();
		}
	}
}
