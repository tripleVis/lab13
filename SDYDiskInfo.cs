using System;
using System.IO;

namespace lab13
{
	// Информация о диске
	static class SDYDiskInfo
	{
		// Свободное пространство на каждом диске
		public static void FreeSpace()
		{
			long available = 0;
			foreach (var drive in DriveInfo.GetDrives())
			{
				available += drive.AvailableFreeSpace;
				Console.WriteLine($"Диск {drive.Name}: {drive.AvailableFreeSpace.ToGb(),-5:F2}гб");
			}
			Console.WriteLine($"\nСвободно места: {available.ToGb(),-5:F2}гб");
		}

		// Файловая система каждого диска
		public static void FileSystems()
		{
			foreach (var drive in DriveInfo.GetDrives())
				Console.WriteLine($"Диск {drive.Name}: {drive.DriveFormat}");
		}

		// Детальная информация о каждом диске
		public static void DetailedInfo()
		{
			foreach (var drive in DriveInfo.GetDrives())
			{
				Console.WriteLine(
					$"\n  Диск {drive.Name}" +
					$"\nВсего    : {drive.TotalSize.ToGb(),-5:F2}гб" +
					$"\nСвободно : {drive.AvailableFreeSpace.ToGb(),-5:F2}гб" +
					$"\nМетка    : {drive.VolumeLabel}"
					);
			}
		}
	}

	// Перевод из б в гб
	static class LongExtension
	{
		public static double ToGb(this long bytes) => bytes / Math.Pow(2, 30);
	}
}
