namespace Site

open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.D3

/// This is a translation of an example by mbostock to WebSharper.D3:
/// http://bl.ocks.org/mbostock/4183330

[<AutoOpen>]
[<JavaScript>]
module D3Extras =

    let Defer (job: (obj * 'T -> unit) -> unit) : Async<'T> =
        Async.FromContinuations(fun (ok, no, _) ->
            job (fun (err, data) -> if As err then no (exn ("Defer: failed")) else ok data))

    type D3 with

        static member Json(url: string) =
            Defer(fun c -> D3.Json(url, c))

        static member Tsv(url: string) =
            Defer(fun c -> D3.Tsv(url, c))

[<JavaScript>]
module WorldTour =

    [<Name "">]
    type ITopoJson =
        abstract feature : topology: obj * geoObject: obj -> obj
        abstract mesh : topology: obj * geoObject: obj * filter: (obj * obj -> bool) -> obj

    let topojson : ITopoJson =
        JS.Global?topojson

    [<AbstractClass>]
    type Country =
        inherit D3.Feature

    let Render (ctx: Dom.Element) =
        let width = 960.
        let height = 500.
        let projection =
            D3.Geo.Orthographic()
                .Scale(248.)
                .ClipAngle(90.)
        let title =
            D3.Select(ctx).Append("h1")
        let canvas =
            D3.Select(ctx)
                .Append("canvas")
                .Attr("width", width)
                .Attr("height", height)
        let c = (canvas.Node() |> As<CanvasElement>).GetContext("2d")
        let path = D3.Geo.Path().Projection(projection).Context(c)
        async {
            let globe = New ["type" => "sphere"]
            let! world = D3.Json("WorldTour/world-110m.json")
            let! names = D3.Tsv("WorldTour/world-country-names.tsv")
            let landFeature = topojson.feature(world, world?objects?``land``)
            let countries : obj [] = topojson.feature(world, world?objects?countries)?features
            let borders = topojson.mesh(world, world?objects?countries, fun (a, b) -> a !==. b)
            let countries =
                countries
                |> Array.filter (fun d ->
                    names
                    |> Array.exists (fun n ->
                        if d?id ==. n?id then
                            d?name <- n?name
                            true
                        else false))
                |> Array.sortBy (fun c -> c?name)
                |> As<Country[]>
            let rec transition i : unit =
                D3.Transition()
                    .Duration(1250)
                    .Each("start", fun (_, _) -> title.Text(countries.[i]?name : string) |> ignore)
                    .Tween("rotate", fun _ ->
                        let (p0, p1) = D3.Geo.Centroid(countries.[i])
                        let r = D3.Interpolate(projection.Rotate(), (-p0, -p1, 0.))
                        Action<_>(fun t ->
                            let (x, y, z) = r.Invoke(t)
                            projection.Rotate(x, y, z) |> ignore
                            c.ClearRect(0., 0., width, height)
                            c.FillStyle <- "#bbb"; c.BeginPath(); path.Call(landFeature) |> ignore; c.Fill();
                            c.FillStyle <- "#f00"; c.BeginPath(); path.Call(countries.[i]) |> ignore; c.Fill();
                            c.StrokeStyle <- "#fff"; c.LineWidth <- 0.5; c.BeginPath(); path.Call(borders) |> ignore; c.Stroke()
                            c.StrokeStyle <- "#000"; c.LineWidth <- 2.0; c.BeginPath(); path.Call(globe) |> ignore; c.Stroke()))
                    .Transition()
                        .Each("end", fun _ -> transition (i + 1))
                |> ignore
            return transition 0
        }
        |> Async.Start

    let Sample =
        Samples.Build()
            .Id("WorldTour")
            .FileName(__SOURCE_FILE__)
            .Render(Render)
            .Create()
