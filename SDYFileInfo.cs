using System;
using System.IO;

namespace lab13
{
	// Информация о файле
	static class SDYFileInfo
	{
		// Полный путь
		public static void FullPath(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException($"Файл {path} не найден");

			Console.WriteLine("Полный путь до файла:\n" + GetFullPath(path));
		}

		// Детальная информация о файле
		public static void Info(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException($"Файл {path} не найден");

			FileInfo file = new FileInfo(path);
			Console.WriteLine(
				$"Имя файла  : {file.Name}" +
				$"\nРазмер     : {file.Length}б" +
				$"\nРасширение : {file.Extension}"
				);
		}

		// Время создания/изменения
		public static void DateInfo(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException($"Файл {path} не найден");

			FileInfo file = new FileInfo(path);
			Console.WriteLine(
				$"Создан  : {file.CreationTime:G}" +
				$"\nИзменён : {file.LastWriteTime:G}"
				);
		}

		// Полный путь
		public static string GetFullPath(string path) => Path.GetFullPath(path);
		// Имя файла
		public static string GetName(string path) => new FileInfo(path).Name;
	}
}
