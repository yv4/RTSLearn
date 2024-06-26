namespace Excel.Log.Logger
{
	public class NullLog : ILog, ILog<NullLog>
	{
		public void InitializeFor(string loggerName)
		{
		}

		public void Debug(string message, params object[] formatting)
		{
		}

		public void Info(string message, params object[] formatting)
		{
		}

		public void Warn(string message, params object[] formatting)
		{
		}

		public void Error(string message, params object[] formatting)
		{
		}

		public void Fatal(string message, params object[] formatting)
		{
		}
	}
}
