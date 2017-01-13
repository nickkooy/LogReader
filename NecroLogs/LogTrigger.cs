using System.Text.RegularExpressions;

namespace NecroLogs
{
	public abstract class LogTrigger
	{
		public LogTrigger(string id)
		{
			Id = id;
		}
		public abstract Regex Condition
		{
			get;
		}

		public string Id { get; private set; }

		public bool Check(LogLine line)
		{
			if (line == null)
				return false;

			return Check(line.Text);
		}

		public virtual bool Check(string line)
		{
			if (Condition == null || line == null)
				return false;

			return Condition.IsMatch(line);
		}
	}
}
