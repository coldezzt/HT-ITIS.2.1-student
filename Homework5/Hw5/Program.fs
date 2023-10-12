open System
open Hw5.Parser
open Hw5.Calculator
open Microsoft.FSharp.Core


let args = [|Console.ReadLine(); Console.ReadLine(); Console.ReadLine()|]

let parsed = args |> parseCalcArguments

let resStr = match parsed with
             | Error e -> e.ToString()
             | Ok s ->
                 let arg1, op, arg2 = s 
                 let result = calculate arg1 op arg2 
                 result.ToString()

Console.WriteLine(resStr)