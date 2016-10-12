module ChessBrawl

let GetInt (s:string[], i:int) = 
    if s.Length <= i then None 
    else match System.Int32.TryParse(s.[i]) with 
            | (true, int) -> Some(int) 
            | _ -> None

type Action (str:string) =
    let parts = str.Split[|':'|]
    member this.CardId = parts.[0]
    member this.Turn = (parts, 1) |> GetInt 

type Player (str:string) =
    let parts = str.Split[|','|]
    let GetActions (s:string) = s.Split[|'|'|] |> Seq.map(Action)
    member this.Hero = parts.[2]
    member this.Result = parts.[3]
    member this.Draws = GetActions parts.[6]
    member this.Plays = GetActions parts.[7]

type Game (str:string list) =
    let meta = str.[2].Split[|','|]
    member this.Id = meta.[0]
    member this.FirstPlayer = (meta, 2) |> GetInt
    member this.FriendlyPlayer = (meta, 3) |> GetInt
    member this.Turns = (meta, 4) |> GetInt
    member this.P1 = Player(str.[1])
    member this.P2 = Player(str.[0])

type Node (name:string, turn:int) =
    member this.Name = name
    member this.Turn = turn
    member val Value = 0 with get, set
    member val Wins = 0 with get, set
    member val Losses = 0 with get, set
    member val Children = List.empty<Node> with get, set
    member this.GetChild (n:string, t:int) = 
        let mutable child = this.Children |> List.tryFind(fun x -> x.Name = n && x.Turn = t)
        if child = None then child <- Node(n, t) |> Some; this.Children <- child.Value :: this.Children
        child.Value.Value <- child.Value.Value + 1 
        (child.Value)

let Read (inputFile:string) = 
    seq {
        let mutable lines = List.empty
        use sr = new System.IO.StreamReader(inputFile)
        while not sr.EndOfStream do
            lines <- sr.ReadLine() :: lines
            if lines.Length = 3 then 
                let game = Game lines
                if game.P1.Hero.StartsWith "KAR_" then yield game
                lines <- List.empty
    }

let GenTree (games:Game seq, depth:int) =
    let mutable root = Node("Root", 0)
    for game in games do
        let mutable node = root
        for turn in [1..min game.Turns.Value depth] do
            let player = if turn%2 = 0 then game.P2 else game.P1
            let plays = player.Plays |> Seq.filter(fun x -> x.Turn = Some(turn))
            let name = if Seq.isEmpty(plays) then "None" else plays |> Seq.map(fun x -> x.CardId) |> String.concat ","
            node <- node.GetChild(name, turn)
            if player.Result = "WON" then node.Wins <- node.Wins+1 else node.Losses <- node.Losses + 1
    (root)

let CardWinrateByTurn (games:Game seq, cardIds:string[]) =
    let queenStats = Array.replicate 35 (0, 0)
    let getQueenAction (game:Game, result:string) = 
        match result with 
        | r when r = game.P1.Result -> game.P1.Plays
        | _ -> game.P2.Plays
        |> Seq.tryFind(fun x -> cardIds |> Array.contains x.CardId), result = "WON"        
    let updateStats (action:Action option, won:bool) =
        if action.IsSome then
            let wins, losses = queenStats.[action.Value.Turn.Value]
            let w, l = if won then (1, 0) else (0, 1)
            queenStats.[action.Value.Turn.Value] <- (wins + w, losses + l)
    for game in games do
        (game, "WON") |> getQueenAction |> updateStats |> ignore
        (game, "LOST") |> getQueenAction  |> updateStats |> ignore
    queenStats |> Array.map(fun (wins, losses) -> (float wins / float (wins + losses), wins + losses)) |> Array.indexed |> printfn "%A"
    0

let rec Stringify (n:Node) = 
    seq {
        if n.Wins > 0 || n.Losses > 0 then 
            let winrate = System.Math.Round(float n.Wins / float (n.Wins + n.Losses), 2)
            yield [string n.Turn; n.Name; string n.Value; string winrate] |> String.concat " "
        for child in n.Children do
            for node in Stringify child do yield node
    }
