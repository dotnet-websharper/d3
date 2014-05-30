namespace Site

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.D3

/// Draws three circles.
[<JavaScript>]
module Circles =

    let private data = [| 10.0; 20.0; 30.0 |]

    let private Render (data: double[]) (canvas: Dom.Element) =
        D3.Select(canvas)
            .Append("svg")
            .SelectAll("circle")
            .Data<double>(data)
            .Enter()
                .Append("circle")
                .Attr("cx", fun x -> x * 10.)
                .Attr("cy", fun x -> x * 10.)
                .Attr("r", 15.)
        |> ignore

    let Sample =
        Samples.Build(__SOURCE_FILE__)
            .Title("Circles")
            .Keywords(["basic"])
            .Render(Render data)
            .Create()
