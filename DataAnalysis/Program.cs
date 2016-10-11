using System;

namespace DataAnalysis
{
	internal class Program
	{
		private static void Main()
		{
			new ChessBrawl("C:/out.csv", "D:/output.txt").CreateTree(3);
			Console.WriteLine("Done");
			Console.ReadKey();
		}
	}
}
