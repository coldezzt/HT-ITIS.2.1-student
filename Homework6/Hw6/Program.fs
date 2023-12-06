module Hw6.App

open Hw6.Calculator
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Giraffe.ViewEngine

let masterPage (pageTitle : string) (content : XmlNode list) =
    html [] [
        head [] [
            meta [ _charset "utf-8"
                   _name "viewport"
                   _content "width=device-width, initial-scale=1" ]
            title [] [ str pageTitle ]
            link [ _href "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"
                   _rel "stylesheet"
                   _integrity "sha384-T3c6CoIi6uLrA9TneNEoa7RxnatzjcDSCmG1MXxSR1GAsXEV/Dwwykc2MPK8M2HN"
                   _crossorigin "anonymous" ]
            script [ _src "https://cdn.jsdelivr.net/npm/@popperjs/core@2.11.8/dist/umd/popper.min.js"
                     _integrity "sha384-I7E8VVD/ismYTF4hNIPjVp/Zjvgyol6VFvRkX/vR+Vc4jQkC+hVqc2pM8ODewa9r"
                     _crossorigin "anonymous" ] []
            script [ _src "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.min.js"
                     _integrity "sha384-BBtl+eGJRgqQAUMxJ7pMwbEyER4l1g+O15P+16Ep7Q9Q+zqX6gSbd85u4mG4QzX+"
                     _crossorigin "anonymous" ] []
        ]
        body [ _class "container" ] [
            h1 [ _class "text-center" ] [ str pageTitle ]
            main [] content
        ]
    ]

let calculatePage =
    [
        h5 [] [ str "Памятка: Используйте ',' для десятичных чисел" ]
        form [ _class "row g-2"; _action "/calculate"; _method "POST"; _id "calculator" ] [
            div [ _class "col-md-5" ] [
                label [ _class "form-label" ] [ str "Первое число:" ]
                input [ _class "form-control"; _type "text"; _name "value1" ]
            ]
            div [ _class "col-md-2" ] [
                label [ _class "form-label" ] [ str "Операция:" ]
                input [ _class "form-control"; _type "text"; _name "operation" ]
            ]
            div [ _class "col-md-5" ] [
                label [ _class "form-label" ] [ str "Второе число:" ]
                input [ _class "form-control"; _type "text"; _name "value2" ]
            ]
            button [ _class "btn btn-success"; _type "submit"] [ str "Посчитать!" ]
        ]   
    ] |> masterPage "Калькулятор"
  
[<CLIMutable>]
type CalculatorArguments =
    {
        Value1 : string
        Operation : string
        Value2 : string
    }
   
let calculatorHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let arg1      = ctx.Request.Query["value1"].ToString()
            let operation = ctx.Request.Query["operation"].ToString()
            let arg2      = ctx.Request.Query["value2"].ToString()
            let parsed = parseCalcArguments([|arg1; operation; arg2|])
            let result =
                match parsed with
                | Ok s ->
                    let arg1, op, arg2 = s
                    Ok ((calculate arg1 op arg2).ToString()) 
                | Error e -> Error (e.ToString())
            
            match result with
            | Ok ok       -> return! (setStatusCode 200 >=> text (ok.ToString())) next ctx
            | Error error -> return! (setStatusCode 400 >=> text error) next ctx
        }
        
let webApp =
    choose [
        GET >=>
            choose [
                route "/"           >=> text "index page"
                route "/calculator" >=> htmlView calculatePage
                route "/calculate"  >=> calculatorHandler
            ]
        setStatusCode 404 >=> text "Not Found" 
    ]
    
type Startup() =
    member _.ConfigureServices (services : IServiceCollection) =
        services
            .AddGiraffe()
            .AddMiniProfiler(fun option -> option.RouteBasePath <- "/profiler")
            |> ignore

    member _.Configure (app : IApplicationBuilder) (_ : IHostEnvironment) (_ : ILoggerFactory) =
        app
            .UseMiniProfiler()
            .UseGiraffe webApp
        
[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun whBuilder -> whBuilder.UseStartup<Startup>() |> ignore)
        .Build()
        .Run()
    0