open System
open Hw4.Calculator
open Hw4.Parser

let arg1 = Console.ReadLine();
let operation = Console.ReadLine();
let arg2 = Console.ReadLine();

let arguments = [|arg1; operation; arg2|]
let parsedArgs = parseCalcArguments(arguments)
let result = calculate parsedArgs.arg1 parsedArgs.operation parsedArgs.arg2 
Console.WriteLine(result)