using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NecroLogs;

namespace LogReader
{
	public class LevelTrigger : LogTrigger
	{

		public LevelTrigger(int id)
			:base(id)
		{

		}

		public override Regex Condition
		{
			get
			{
				return new Regex("NEW ITEM");
			}
		}

		public int ItemId { get; set; }
		public CoOrd Position { get; set; }
		public string LevelId { get; set; }
	}
}
