namespace Excel.Log
{
	public interface ILog
	{
		void InitializeFor(string loggerName);

		void Debug(string message, params object[] formatting);

		void Info(string message, params object[] formatting);

		void Warn(string message, params object[] formatting);

		void Error(string message, params object[] formatting);

		void Fatal(string message, params object[] formatting);
	}
	public interface ILog<T> where T : new()
	{
	}
}
