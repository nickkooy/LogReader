namespace NecroLogs
{
	public abstract class ActionClip : TriggerAction
	{
		public const int DefaultClipSize = 1;
		public ActionClip(string triggerId) : this(triggerId, DefaultClipSize)
		{
		}

		public ActionClip(string triggerId, int clipsize)
			 : base(triggerId)
		{
		}

		public int Rounds { get; set; }

		public int RoundsFired { get; set; }

		public void Reload()
		{
			RoundsFired = 0;
		}

		public sealed override void Triggered()
		{
			if (RoundsFired < Rounds)
			{
				++RoundsFired;
				Fire();
			}
		}

		protected abstract void Fire();
	}
}
