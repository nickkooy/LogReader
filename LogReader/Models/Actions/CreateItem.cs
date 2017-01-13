using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NecroLogs;

namespace LogReader.Models.Actions
{
	public class CreateItem : TriggerAction
	{
		public CreateItem(string triggerId) : base(triggerId)
		{
		}

		public override void Triggered()
		{
			throw new NotImplementedException();
		}
	}
}
