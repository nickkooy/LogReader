namespace NecroLogs
{
	public abstract class ToggleAction : TriggerAction
	{
		public ToggleAction(string triggerId) : base(triggerId)
		{
		}

		public bool State
		{
			get;
			set;
		}

		public sealed override void Triggered()
		{
			State = !State;
			Toggled();
		}

		protected abstract void Toggled();
	}
}
