module Hw5.Parser

open System
open Hw5
open Hw5.Calculator
open Hw5.MaybeBuilder
open Microsoft.FSharp.Core

let isArgLengthSupported (args:string[]): Result<'a,'b> =
    match args.Length = 3 with
    | true -> Ok args
    | false -> Message.WrongArgLength |> Error

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isOperationSupported (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    match operation with
    | "+" -> Ok (arg1, CalculatorOperation.Plus, arg2)
    | "-" -> Ok (arg1, CalculatorOperation.Minus, arg2)
    | "*" -> Ok (arg1, CalculatorOperation.Multiply, arg2)
    | "/" -> Ok (arg1, CalculatorOperation.Divide, arg2)
    | _ -> Message.WrongArgFormatOperation |> Error

let parseArgs (args: string[]): Result<('a * CalculatorOperation * 'b), Message> =
    match args[0].Replace(".", ",") |> Double.TryParse with
    | false, _ -> Message.WrongArgFormat |> Error
    | true, arg1 ->
        match args[2].Replace(".", ",") |> Double.TryParse  with 
        | false, _ -> Message.WrongArgFormat |> Error
        | true, arg2 -> isOperationSupported (arg1, args[1], arg2)

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isDividingByZero (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    match operation with
    | CalculatorOperation.Divide ->
        match int arg2 with
        | 0 -> Message.DivideByZero |> Error
        | _ -> Ok (arg1, operation, arg2)
    | _ -> Ok (arg1, operation, arg2)
    
let parseCalcArguments (args: string[]): Result<'a, 'b> =
    match isArgLengthSupported args with
    | Error e -> e |> Error
    | Ok args ->
        let parsed = args |> parseArgs
        match parsed with
        | Ok s -> s |> isDividingByZero
        | Error e -> e |> Error
