using System.Collections.Generic;
using System.Linq;

namespace DataAnalysis.ChessBrawl.Objects
{
	public class Player
	{
		public string Hero { get; set; }
		public string Result { get; set; }
		public int Fatigue;
		public int HeroPowerUses;
		public IEnumerable<Action> Draws { get; set; }
		public IEnumerable<Action> Plays { get; set; }

		public static Player Parse(string line)
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
}
