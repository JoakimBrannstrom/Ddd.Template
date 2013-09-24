namespace Ddd.Template.Server.Bootstrap
{
	using System;
	using log4net;

	public sealed class Log4NetLogger : NEventStore.Logging.ILog
	{
		private readonly ILog _log;

		public Log4NetLogger(Type typeToLog)
		{
			_log = LogManager.GetLogger(typeToLog);
		}

		public void Verbose(string message, params object[] values)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat(message, values);
		}

		public void Debug(string message, params object[] values)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat(message, values);
		}

		public void Info(string message, params object[] values)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat(message, values);
		}

		public void Warn(string message, params object[] values)
		{
			if (_log.IsWarnEnabled)
				_log.WarnFormat(message, values);
		}

		public void Error(string message, params object[] values)
		{
			if (_log.IsErrorEnabled)
				_log.ErrorFormat(message, values);
		}

		public void Fatal(string message, params object[] values)
		{
			if (_log.IsFatalEnabled)
				_log.FatalFormat(message, values);
		}
	}
}
