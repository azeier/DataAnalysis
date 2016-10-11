using System.Collections.Generic;
using System.Linq;

namespace DataAnalysis.ChessBrawl
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
}
