using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroLogs
{
	public class LogLine
	{
		public string Text { get; set; }
		public DateTime Timestamp { get; set; }

        public LogLine(string text)
        {
            Text = text;
            Timestamp = DateTime.Now;
        }
	}
}
