using System;
using System.IO;

namespace lab13
{
	// Информация о директории
	static class SDYDirInfo
	{
		// Кол-во файлов в директории
		public static void FilesAmt(string path)
		{
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException($"Директория {path} не найдена");

			Console.WriteLine($"Кол-во файлов: {new DirectoryInfo(path).GetFiles().Length}");
		}

		// Дата создания директория
		public static void CreationDate(string path)
		{
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException($"Директория {path} не найдена");

			Console.WriteLine($"Дата создания: {new DirectoryInfo(path).CreationTime:G}");
		}

		// Кол-во поддиректориев
		public static void SubDirsAmt(string path)
		{
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException($"Директория {path} не найдена");

			Console.WriteLine($"Кол-во поддиректориев: {new DirectoryInfo(path).GetDirectories().Length}");
		}

		// Вывод директориев-родителей
		public static void ParentDirs(string path)
		{
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException($"Директория {path} не найдена");

			Console.WriteLine("Список родительских директориев");
			Print(new DirectoryInfo(path).Parent);

			// Рекурсивный вывод предка
			static void Print(DirectoryInfo directory)
			{
				if (directory == null)
					return;
				Console.WriteLine("  " + directory);
				Print(directory.Parent);
			}
		}
	}
}
