using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataAnalysis.ChessBrawl.Objects;

namespace DataAnalysis.ChessBrawl
{
	public class Analysis
	{
		public int TotalGames { get; set; }
		public int WhiteWins { get; set; }
		public int[] CoinWins = new int[100];
		public int[] CoinLosses = new int[100];
		private List<Game> _games;
		private readonly string _inputFile;

		public Analysis(string inputFile)
		{
			_inputFile = inputFile;
		}

		public void CreateActionTree(int depth, string outputFile)
		{
			_games = ReadGames(_inputFile).ToList();
			var root = new Node("Root", 0);
			foreach(var game in _games)
			{
				var currNode = root;
				for(var turn = 1; turn <= game.Turns; turn++)
				{
					var player = turn%2 == 0 ? game.P2 : game.P1;
					var plays = player.Plays.Where(x => x.Turn == turn).ToList();
					var name = plays.Count == 0 ? "PASS" : string.Join(",", plays.Select(x => x.CardId));
					currNode = currNode.GetChild(name, turn);
					currNode.Value++;
					if(player.Result == "WON")
						currNode.Wins++;
					else currNode.Losses++;
				}
			}
			using(var sw = new StreamWriter(outputFile))
				foreach(var line in GetNodeStrings(root, depth))
					sw.WriteLine(line);
		}

		public void Foo()
		{
			_games = ReadGames(_inputFile).ToList();
			foreach(var game in _games)
			{
				TotalGames++;
				var whiteWon = false;
				if(game.P1.Result == "WON")
				{
					WhiteWins++;
					whiteWon = true;
				}
				var coinTurn = game.P2.Plays.FirstOrDefault(x => x.CardId == "GAME_005")?.Turn ?? 99;
				(whiteWon ? CoinLosses : CoinWins)[coinTurn]++;

			}
			Console.WriteLine("Total Games: " + TotalGames);
			Console.WriteLine($"White Wins: {WhiteWins} ({100.0*WhiteWins/TotalGames}%)");
			for(var turn = 2; turn < 100; turn += 2)
			{
				var wins = CoinWins[turn];
				var losses = CoinLosses[turn];
				if(wins + losses == 0)
					break;
				Console.WriteLine($"Turn {turn/2} Coin winrate: {Math.Round(100.0*wins/(wins + losses), 2)}% ({wins + losses} games)");
			}
			var noCoinWins = CoinWins[99];
			var noCoinLosses = CoinLosses[99];
			Console.WriteLine($"No Coin played winrate: {Math.Round(100.0*noCoinWins/(noCoinWins + noCoinLosses), 2)}% ({noCoinWins + noCoinLosses} games)");
			Console.WriteLine(string.Join(", ", CoinWins.Take(10)));
			Console.WriteLine(string.Join(", ", CoinLosses.Take(10)));
		}

		private IEnumerable<Game> ReadGames(string inputFile)
		{
			var invalidGames = 0;
			using(var reader = new StreamReader(inputFile))
			{
				var game = new Game();
				while(!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					if(!game.Parse(line))
						continue;
					if(game.P1.Hero.StartsWith("KAR_"))
						yield return game;
					else invalidGames++;
					game = new Game();
				}
			}
			Console.WriteLine("Invalid Games: " + invalidGames);
		}

		private IEnumerable<string> GetNodeStrings(Node node, int depthThreshold)
		{
			yield return $"{node.Turn} {node.Name} {node.Value} {Math.Round(100.0 * node.Wins / (node.Wins + node.Losses), 2)}%";
			foreach(var child in node.Children)
			{
				if(child.Turn > depthThreshold) continue;
				foreach(var val in GetNodeStrings(child, depthThreshold))
					yield return val;
			}
		}
	}
}
