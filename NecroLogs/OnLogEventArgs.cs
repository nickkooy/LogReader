using System;

namespace NecroLogs
{
	public class OnLogEventArgs : EventArgs
	{
		public LogLine Line { get; private set; }
		public OnLogEventArgs(LogLine line)
		{
			Line = line;
		}
	}
}
