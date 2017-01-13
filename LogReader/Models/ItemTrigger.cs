using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NecroLogs;

namespace LogReader
{
	public class ItemTrigger : LogTrigger
	{
		public ItemTrigger(string id) : base(id)
		{
		}

		public override Regex Condition
		{
			get
			{
				return new Regex("NEW ITEM");
			}
		}
	}
}
