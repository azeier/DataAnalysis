using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataAnalysis
{
	public class Node
	{
		public string Name { get; set; }
		public int Turn { get; }
		public List<Node> Children = new List<Node>();
		public int Value { get; set; }
		public int Wins { get; set; }
		public int Losses { get; set; }

		public Node(string name, int turn)
		{
			Name = name;
			Turn = turn;
		}

		public Node GetChild(string name, int turn)
		{
			var child = Children.FirstOrDefault(x => x.Name == name && x.Turn == turn);
			if(child == null)
			{
				child = new Node(name, turn);
				Children.Add(child);
			}
			return child;
		}
	}

	internal class ChessBrawl
	{
		public int TotalGames { get; set; }
		public int WhiteWins { get; set; }
		public int[] CoinWins = new int[100];
		public int[] CoinLosses = new int[100];
		private List<Game> _games;
		private readonly string _inputFile;
		private readonly string _outputFile;

		public ChessBrawl(string inputFile, string outputFile)
		{
			_inputFile = inputFile;
			_outputFile = outputFile;
		}

		public void CreateTree(int depth)
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
			using(var sw = new StreamWriter(_outputFile))
				foreach(var line in PrintNode(root, depth))
					sw.WriteLine(line);
		}

		private IEnumerable<string> PrintNode(Node node, int depthThreshold)
		{
			yield return $"T{node.Turn} {node.Name} {node.Value} {Math.Round(100.0 * node.Wins / (node.Wins + node.Losses), 2)}%";
			foreach(var child in node.Children)
			{
				if(child.Turn > depthThreshold) continue;
				foreach(var val in PrintNode(child, depthThreshold))
					yield return val;
			}
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
			for(int turn = 2; turn < 100; turn += 2)
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
					if(game.Read(line))
					{
						if(game.P1.Hero.StartsWith("KAR_"))
							yield return game;
						else invalidGames++;
						game = new Game();
					}
				}
			}
			Console.WriteLine("Invalid Games: " + invalidGames);
		}
	}

	internal class Game
	{
		private int _index;
		public string Id { get; set; }
		public int FirstPlayer;
		public int FriendlyPlayer;
		public int Turns;
		public Player P1 { get; set; }
		public Player P2 { get; set; }

		public bool Read(string line)
		{
			var values = line.Split(',');
			if(_index == 0)
			{
				Id = values[0];
				int.TryParse(values[2], out FirstPlayer);
				int.TryParse(values[3], out FriendlyPlayer);
				int.TryParse(values[4], out Turns);
			}
			else if(_index == 1)
				P1 = Player.Read(line);
			else if(_index == 2)
			{
				P2 = Player.Read(line);
				return true;
			}
			_index++;
			return false;
		}
	}

	public class Player
	{
		public string Hero { get; set; }
		public string Result { get; set; }
		public int Fatigue;
		public int HeroPowerUses;
		public IEnumerable<Action> Draws { get; set; }
		public IEnumerable<Action> Plays { get; set; }

		public static Player Read(string line)
		{
			var values = line.Split(',');
			var player = new Player
			{
				Hero = values[2],
				Result = values[3],
				Draws = GetActions(values[6]),
				Plays = GetActions(values[7]),
			};
			int.TryParse(values[4], out player.Fatigue);
			int.TryParse(values[5], out player.HeroPowerUses);
			return player;
		}

		private static IEnumerable<Action> GetActions(string line) => line.Split('|').Select(Action.Parse);
	}

	public class Action
	{
		public string CardId;
		public int Turn;

		public static Action Parse(string actionString)
		{
			var action = new Action();
			var parts = actionString.Split(':');
			action.CardId = parts[0];
			if(parts.Length > 1)
				int.TryParse(parts[1], out action.Turn);
			return action;
		}
	}
}
