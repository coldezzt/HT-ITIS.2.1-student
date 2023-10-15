open System
open System.Net.Http

let SendRequest (args : string[]) =
    async {
        let url = $"http://localhost:5000/calculate?value1={args[0]}&operation={args[1]}&value2={args[2]}"
        let client = new HttpClient()
        try 
            let! response = client.GetStringAsync(url) |> Async.AwaitTask
            client.Dispose()
            return response            
        with ex -> return ex.ToString()
    }
    
[<EntryPoint>]
let main _ =
    Console.WriteLine("Первое число > enter > операция > enter > третье число > enter")
    while true do
        let args = [|Console.ReadLine(); Console.ReadLine(); Console.ReadLine()|]
        async {
            let! result = SendRequest args
            return result
        } |> Async.RunSynchronously
        |> Console.WriteLine
    0