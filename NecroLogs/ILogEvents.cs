using System;

namespace NecroLogs
{
	public interface ILogEvents
	{
		event EventHandler<OnLogEventArgs> OnLogEvent;
	}


}
