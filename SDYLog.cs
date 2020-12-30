using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lab13
{
	// Логирование
	static class SDYLog
	{
		public static string File { get; } = "sdylogfile.txt";

		// Запись простого лога
		public static void Log(string msg, LogType logType = LogType.Info)
		{
			using var sw = new StreamWriter(File, true);
			sw.WriteLineAsync($"{logType}/{DateTime.Now:G}/{msg}");
		}

		// Запись лога с ошибкой
		public static void Log(string msg, Exception exception, LogType logType = LogType.Warn)
		{
			try
			{
				using var sw = new StreamWriter(File, true);
				sw.WriteLineAsync($"{logType}/{DateTime.Now:G}/{msg}");
				if (logType == LogType.Warn)
					sw.WriteLine("\t" + exception.Message);
				else
				{
					sw.WriteLine("\t" + exception.Message);
					sw.WriteLineAsync(exception.StackTrace);
				}
			}
			catch (Exception e)
			{
				Log("Что-то очень плохое", e, LogType.Error);
			}
		}

		// Получение всех записей
		public static List<LogItem> GetLogs()
		{
			using var sr = new StreamReader(File);
			var logs = new List<LogItem>();

			string line;
			bool firstLog = true;

			LogType type = LogType.Unknown;
			DateTime time = DateTime.Now;
			string msg = "", innerMsg = "";
			var stackTrace = new List<string>();

			while ((line = sr.ReadLine()) != null)
			{
				if (line[0] == ' ')
					stackTrace.Add(line.Trim());
				else if (line[0] == '\t')
					innerMsg = line.Trim();
				else
				{
					if (!firstLog)
					{
						logs.Add(new LogItem(type, time, msg, innerMsg, stackTrace));
						type = LogType.Unknown;
						time = DateTime.Now;
						msg = "";
						innerMsg = "";
						stackTrace = new List<string>();
					}
					else
						firstLog = false;
					var items = line.Split('/');
					type = items[0] switch
					{
						"Info" => LogType.Info,
						"Warn" => LogType.Warn,
						"Error" => LogType.Error,
						_ => LogType.Unknown
					};
					time = DateTime.Parse(items[1]);
					for (int i = 2; i < items.Length; i++)
					{
						msg += items[i];
					}
				}
			}
			logs.Add(new LogItem(type, time, msg, innerMsg, stackTrace));
			return logs;
		}

		// Выборка определённых записей
		public static List<LogItem> GetSomeLogs(DateTime day) =>
			GetLogs().Where(item => $"{item.Time:d}" == $"{day:d}").ToList();


		public static List<LogItem> GetSomeLogs(DateTime date1, DateTime date2) =>
			GetLogs().Where(item => item.Time > date1 && item.Time < date2)
			.ToList();

		public static List<LogItem> GetSomeLogs(string keyWord) =>
			GetLogs().Where(item =>
				item.Type.ToString() == keyWord.ToUpper() ||
				item.Msg.ToLower().Contains(keyWord.ToLower()))
			.ToList();

		// Очистка логов
		public static int ClearLogs()
		{
			var logs = GetLogs();
			int counter = 0;
			foreach (var item in logs)
			{
				if (DateTime.Now - item.Time < new TimeSpan(1, 0, 0))
				{
					LogLogItem(item);
					counter++;
				}
			}
			return counter;
		}

		// Запись определённой записи
		private static void LogLogItem(LogItem item)
		{
			using var sw = new StreamWriter(File, true);
			sw.WriteLineAsync($"{item.Type}/{item.Time:G}/{item.Msg}");
			if (item.Type == LogType.Warn)
				sw.WriteLine("\t" + item.InnerMsg);
			else
			{
				sw.WriteLine("\t" + item.InnerMsg);
				foreach (var stackItem in item.StackTrace)
				{
					sw.WriteLineAsync(stackItem);
				}
			}
		}
	}

	// Класс, представляющий одну запись лога
	class LogItem
	{
		public LogType Type { get; }
		public DateTime Time { get; }
		public string Msg { get; }
		public string InnerMsg { get; }
		public List<string> StackTrace { get; }

		public LogItem(LogType type, DateTime time, string msg, string innerMsg, List<string> stackTrace)
		{
			Type = type;
			Time = time;
			Msg = msg;
			InnerMsg = innerMsg;
			StackTrace = stackTrace;
		}

		public void PrintStackTrace()
		{
			foreach (var item in StackTrace)
				Console.WriteLine(item);
		}
	}

	enum LogType
	{
		Info,
		Warn,
		Error,
		Unknown
	}
}
