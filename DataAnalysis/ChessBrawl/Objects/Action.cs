namespace DataAnalysis.ChessBrawl.Objects
{
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
