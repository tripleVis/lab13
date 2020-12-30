using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace lab13
{
	static class SDYFileManager
	{
		// Получение файлов из каталога
		public static FileInfo[] GetFiles(string strDrive)
		{
			var drive = GetDrive(strDrive);
			if (drive == null)
			{
				SDYLog.Log($"Диск {drive} не найден", LogType.Warn);
				return null;
			}
			return new DirectoryInfo(drive.Name).GetFiles();
		}

		// Получение каталогов из корня диска
		public static DirectoryInfo[] GetDirs(string strDrive)
		{
			var drive = GetDrive(strDrive);
			if (drive == null)
			{
				SDYLog.Log($"Диск {drive} не найден", LogType.Warn);
				return null;
			}
			return new DirectoryInfo(drive.Name).GetDirectories();
		}

		// Получения информации о диске
		public static DriveInfo GetDrive(string drive)
		{
			foreach (var item in DriveInfo.GetDrives())
				if (item.Name.ToLower()[0] == drive.ToLower()[0])
					return item;
			return null;
		}

		public static void FilesAndDirs(string drive)
		{
			if (GetDrive(drive) == null)
			{
				SDYLog.Log($"Диск {drive} не найден", LogType.Warn);
				return;
			}

			// Запись файлов и директориев диска
			var files = GetFiles(drive);
			var dirs = GetDirs(drive);
			Directory.CreateDirectory("SDYInspect");
			using (var sw = new StreamWriter("SDYInspect/SDYdirinfo.txt"))
			{
				sw.WriteLine($"[Диск {drive.ToUpper()}]");
				sw.WriteLine("[Список файлов]");
				foreach (var item in files)
					sw.WriteLine(item.Name);
				sw.WriteLine("[Список директориев]");
				foreach (var item in dirs)
					sw.WriteLine(item.Name);
			}

			// Копирование
			File.Copy("SDYInspect/SDYdirinfo.txt", "SDYInspect/SDYdirinfo_copy.txt", true);
			int failCounter = 0;

			// Удаление
			File.Delete("SDYInspect/SDYdirinfo.txt");
		}

		public static void FilesWithExtension(string path, string ext)
		{
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException($"Директория {path} не найдена");

			// Создание нового чистого директория
			Directory.CreateDirectory("SDYFiles");
			foreach (var item in new DirectoryInfo("SDYFiles").GetFiles())
				File.Delete(item.FullName);

			// Копирование в него файлов
			var files = new DirectoryInfo(path).GetFiles();
			foreach (var item in files)
				if (item.Extension == "." + ext || item.Extension == ext)
					File.Copy(item.FullName, "SDYFiles/" + item.Name);

			Directory.CreateDirectory("SDYInspect");
			Directory.CreateDirectory("Files");
			foreach (var item in new DirectoryInfo("Files").GetFiles())
				File.Delete(item.FullName);

			Directory.Delete("SDYInspect/SDYFiles", true);
			// Перемещение директория
			Directory.Move("SDYFiles", "SDYInspect/SDYFiles");

			// Удаление старого архива
			File.Delete("SDYInspect/archive.zip");
			// Упаковка архива
			ZipFile.CreateFromDirectory("SDYInspect/SDYFiles", "SDYInspect/archive.zip");
			// Распаковка архива
			ZipFile.ExtractToDirectory("SDYInspect/archive.zip", "Files");
		}
	}
}
