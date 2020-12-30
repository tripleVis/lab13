using System;
using System.Collections.Generic;
using System.IO;

namespace lab13 {
	class Program {
		static readonly List<Action> Tasks = new List<Action> {//список делегатов
			DiskInfo,
			FileInfo,
			DirInfo,
			FilesAndDirsOfDrive,
			FilesWithExtension,
			Logs,
			NavLogs,
			ClearLogs
		};
		public static void Main() {
			while (true) {
				try {
					Console.Write(
						"1 - диск" +
						"\n2 - файл" +
						"\n3 - папка" +
						"\n4 - файлы и директории диска" +
						"\n5 - файлы с расширением" +
						"\n6 - просмотреть логи" +
						"\n7 - просмотреть логи за день/период/по ключевому слову" +
						"\n8 - почистить логи" +
						"\n0 - выход" +
						"\nВыберите действие: "
						);
					if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > Tasks.Count) {//если не int
						Console.WriteLine("Нет такого действия");
						continue;
					}
					if (choice == 0) {
						Console.WriteLine("Выход...");
						Environment.Exit(0);
					}
					Tasks[choice - 1]();
				}
				catch (FileNotFoundException e) {
					SDYLog.Log("Файл не найден", e);
				}
				catch (DirectoryNotFoundException e) {
					SDYLog.Log("Директорий не найден", e);
				}
				catch (Exception e) {
					SDYLog.Log("Необработанное исключение", e, LogType.Error);
				}
				finally {
					Console.ReadKey();
					Console.Clear();
				}
			}
		}

		//информация о диске
		static void DiskInfo() {
			SDYLog.Log("Просмотрена информация о дисках");
			Console.WriteLine("\n* Место на дисках");
			SDYDiskInfo.FreeSpace();
			Console.WriteLine("\n* Файловые системы дисков");
			SDYDiskInfo.FileSystems();
			Console.WriteLine("\n* Подробная информация");
			SDYDiskInfo.DetailedInfo();
		}
		//информация о файле
		static void FileInfo() {
			Console.Write("Введите путь до файла: ");
			string path = Console.ReadLine();
			if (!File.Exists(path)) {
				Console.WriteLine("Файл не найден");
				return;
			}
			SDYLog.Log($"Просмотрена информация о файле {SDYFileInfo.GetFullPath(path)}");

			Console.WriteLine($"Информация о файле " + SDYFileInfo.GetName(path));
			SDYFileInfo.FullPath(path);
			SDYFileInfo.Info(path);
			SDYFileInfo.DateInfo(path);
		}
		//информация о директории
		static void DirInfo() {
			Console.Write("Введите путь директория: ");
			string path = Console.ReadLine();
			if (!Directory.Exists(path)) {
				Console.WriteLine("Директорий не найден");
				return;
			}
			SDYLog.Log($"Просмотрена информация о директории {path}");

			SDYDirInfo.FilesAmt(path);
			SDYDirInfo.CreationDate(path);
			SDYDirInfo.SubDirsAmt(path);
			SDYDirInfo.ParentDirs(path);
		}
		//информация о файлах и директориях диска
		static void FilesAndDirsOfDrive() {
			Console.Write("Введите букву диска: ");
			string drive = Console.ReadLine();
			if (SDYFileManager.GetDrive(drive) == null) {
				Console.WriteLine("Диск не найден");
				return;
			}
			SDYLog.Log("Считаны и записаны файлы и папки диска " + drive.ToUpper());
			SDYFileManager.FilesAndDirs(drive);
			Console.WriteLine("Всё считано и записано");
		}
		//файлы с заданным расширением в заданном директории
		static void FilesWithExtension() {
			Console.Write("Введите путь директория: ");
			string path = Console.ReadLine();
			if (!Directory.Exists(path)) {
				Console.WriteLine("Директорий не найден");
				return;
			}
			Console.Write("Введите расширение файлов: ");
			string ext = Console.ReadLine();
			SDYLog.Log($"Поиск файлов в директории {path} с расширением .{ext}");

			SDYFileManager.FilesWithExtension(path, ext);
			Console.WriteLine("Всё считано и записано");
		}
		///логи
		static void Logs() {
			var logs = SDYLog.GetLogs();
			if (logs.Count == 0) {
				Console.WriteLine("Записей в логах не обнаружено");
				return;
			}
			Console.WriteLine($"\n{logs.Count} записей с {logs[0].Time:G}\n");
			foreach (var item in logs) {
				Console.WriteLine($"\n{item.Type,-7} | {item.Time:G} | {item.Msg}");
				if (item.InnerMsg != "")
					Console.WriteLine("\t" + item.InnerMsg);
				if (item.StackTrace.Count != 0)
					foreach (var stackItem in item.StackTrace)
						Console.WriteLine("\t" + stackItem);
			}
		}
		//навигация по логам
		static void NavLogs() {
			Console.Write(
				"Выбрать определённые логи за" +
				"\nа) определённый день - \"дд.мм.гггг\"" +
				"\nб) промежуток времени - \"дд.мм.гггг чч:мм дд.мм.гггг чч:мм\"" +
				"\nв) по ключевому слову" +
				"\nВведите строку для поиска: "
				);
			string str = Console.ReadLine();
			if (str == null) return;
			string[] splitted = str.Split(' ');
			List<LogItem> logs;
			logs = SDYLog.GetSomeLogs(str);
			switch (splitted.Length) {
				case 1:
					if (DateTime.TryParse(str, out DateTime res))//преобразование к дате
						logs = SDYLog.GetSomeLogs(res);
					break;
				case 4:
					string strDate1 = $"{splitted[0]} {splitted[1]}";
					string strDate2 = $"{splitted[2]} {splitted[3]}";
					if (DateTime.TryParse(strDate1, out DateTime date1) &&
					    DateTime.TryParse(strDate2, out DateTime date2))
						logs = SDYLog.GetSomeLogs(date1, date2);
					break;
				default:
					break;
			}
			if (logs.Count == 0)
				Console.WriteLine("Записей не найдено");
			else
				foreach (var item in logs) {
					Console.WriteLine($"\n{item.Type,-7} | {item.Time:G} | {item.Msg}");
					if (item.InnerMsg != "")
						Console.WriteLine("\t" + item.InnerMsg);
					if (item.StackTrace.Count != 0)
						foreach (var stackItem in item.StackTrace)
							Console.WriteLine("\t" + stackItem);
				}
		}
		//очистка логов
		static void ClearLogs() {
			int res = SDYLog.ClearLogs();
			Console.WriteLine($"Удалено {res} записей");
		}
	}
}
