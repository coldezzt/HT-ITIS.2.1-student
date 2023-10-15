module Hw6.Calculator

open System

type Message =
    | SuccessfulExecution     = 0
    | WrongArgLength          = 1
    | WrongArgFormat          = 2
    | WrongArgFormatOperation = 3
    | DivideByZero            = 4

type CalculatorOperation =
     | Plus     = 0
     | Minus    = 1
     | Multiply = 2
     | Divide   = 3

type MaybeBuilder() =
    member this.Bind(x, f): Result<'e,'d> =
        match x with
        | Ok s    -> f s
        | Error e -> Error e
        
    member this.Return x: Result<'a,'b> =
        Ok x
    
let maybe = MaybeBuilder()

let isArgLengthSupported (args: string[]): Result<string * string * string, string> =
    match args.Length = 3 with
    | true  -> Ok (args[0], args[1], args[2]) 
    | false -> "Wrong arguments length" |> Error

let inline isOperationSupported (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), string> =
    match operation with
    | "+" | "Plus"     -> Ok (arg1, CalculatorOperation.Plus, arg2)
    | "-" | "Minus"    -> Ok (arg1, CalculatorOperation.Minus, arg2)
    | "*" | "Multiply" -> Ok (arg1, CalculatorOperation.Multiply, arg2)
    | "/" | "Divide"   -> Ok (arg1, CalculatorOperation.Divide, arg2)
    | _   -> $"Could not parse value '{operation}'" |> Error

let parseArgs (arg1: string, operation, arg2: string): Result<('a * CalculatorOperation * 'b), string> =
    match arg1 |> Double.TryParse with
    | false, _   -> $"Could not parse value '{arg1}'" |> Error
    | true, arg1 ->
        match arg2 |> Double.TryParse with 
        | false, _   -> $"Could not parse value '{arg2}'" |> Error
        | true, arg2 -> Ok (arg1, operation, arg2)

let inline isDividingByZero (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), string> =
    match operation with
    | CalculatorOperation.Divide ->
        match int arg2 with
        | 0 -> "DivideByZero" |> Error
        | _ -> Ok (arg1, operation, arg2)
    | _ -> Ok (arg1, operation, arg2)

let parseCalcArguments (args: string[]): Result<('a * CalculatorOperation * 'a), string> =
    maybe {
        let! x      = isArgLengthSupported args
        let! y      = isOperationSupported x
        let! z      = parseArgs y
        let! result = isDividingByZero z
        return result
    }
   
let inline calculate value1 (operation : CalculatorOperation) value2 : ^a =
    match int operation with
    | 0 -> value1 + value2
    | 1 -> value1 - value2
    | 2 -> value1 * value2
    | 3 -> value1 / value2
    | _ -> LanguagePrimitives.GenericZero
    