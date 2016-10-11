namespace DataAnalysis.ChessBrawl.Objects
{
	public class Game
	{
		private int _parsed;
		public string Id { get; set; }
		public int FirstPlayer;
		public int FriendlyPlayer;
		public int Turns;
		public Player P1 { get; set; }
		public Player P2 { get; set; }

		public bool Parse(string line)
		{
			var values = line.Split(',');
			switch(_parsed)
			{
				case 0:
					Id = values[0];
					int.TryParse(values[2], out FirstPlayer);
					int.TryParse(values[3], out FriendlyPlayer);
					int.TryParse(values[4], out Turns);
					break;
				case 1:
					P1 = Player.Parse(line);
					break;
				case 2:
					P2 = Player.Parse(line);
					return true;
			}
			_parsed++;
			return false;
		}
	}
}
