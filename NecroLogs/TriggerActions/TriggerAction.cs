namespace NecroLogs
{
	public abstract class TriggerAction
	{
		public string TriggerId { get; private set; }

		public TriggerAction(string triggerId)
		{
			TriggerId = triggerId;
		}

		public abstract void Triggered();
	}

}
