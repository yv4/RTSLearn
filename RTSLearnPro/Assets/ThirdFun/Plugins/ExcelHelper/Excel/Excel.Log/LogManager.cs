using System.Collections.Generic;

namespace Excel.Log
{
	public static class LogManager
	{
		private static readonly Dictionary<string, ILog> _dictionary = new Dictionary<string, ILog>();

		private static object _sync = new object();

		public static ILog Log<T>(T type)
		{
			string fullName = typeof(T).FullName;
			return Log(fullName);
		}

		public static ILog Log(string objectName)
		{
			ILog log = null;
			if (_dictionary.ContainsKey(objectName))
			{
				log = _dictionary[objectName];
			}
			if (log == null)
			{
				lock (_sync)
				{
					log = Excel.Log.Log.GetLoggerFor(objectName);
					_dictionary.Add(objectName, log);
					return log;
				}
			}
			return log;
		}
	}
}
