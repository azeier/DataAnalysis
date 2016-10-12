[<EntryPoint>]
let main argv =
    (ChessBrawl.Read "C:/out.csv", 3) |> ChessBrawl.GenTree |> ChessBrawl.Stringify |> Seq.iter (printfn "%s")
    System.Console.ReadKey() |> ignore
    0
