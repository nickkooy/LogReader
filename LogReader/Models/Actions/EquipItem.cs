using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NecroLogs;

namespace LogReader
{
	public class EquipItem : TriggerAction
	{
		public EquipItem(string triggerId)
			: base(triggerId)
		{
		}

		public override void Triggered()
		{
			throw new NotImplementedException();
		}
	}
}
