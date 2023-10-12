module Hw5.MaybeBuilder

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
type MaybeBuilder() =
    member this.Bind(x, f): Result<'e,'d> =
        match x with
        | Ok s -> f s
        | Error e -> Error e
        
    member this.Return x: Result<'a,'b> =
        Ok x
        
let maybe = MaybeBuilder()