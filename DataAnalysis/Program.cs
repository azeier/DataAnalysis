using System;

namespace DataAnalysis
{
	internal class Program
	{
		private static void Main()
		{
			new ChessBrawl.Analysis("C:/out.csv").CreateActionTree(100, "D:/output.txt");
			Console.WriteLine("Done");
			Console.ReadKey();
		}
	}
}
