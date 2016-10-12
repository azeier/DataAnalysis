[<EntryPoint>]
let main argv =
    //(ChessBrawl.Read "C:/out.csv", 3) |> ChessBrawl.GenTree |> ChessBrawl.Stringify |> Seq.iter (printfn "%s")
    (ChessBrawl.Read "C:/out.csv", [|"KAR_A10_09"; "KAR_A10_10"|]) |> ChessBrawl.CardWinrateByTurn |> ignore
    System.Console.ReadKey() |> ignore
    0
