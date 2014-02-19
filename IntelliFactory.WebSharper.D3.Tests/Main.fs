namespace IntelliFactory.WebSharper.D3.Tests

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets
open IntelliFactory.WebSharper.D3

//module D3Helpers =
//    type Area [<Inline "d3.svg.area()">] () =
//        member this.X
//            with [<Inline "$this.x()">] get () = X<obj -> float>
//            and  [<Inline "$this.x($x)">] set (x: obj -> float) = ()
//
//        [<Inline "$this.x($x)">]
//        member this.SetX (x: float) = ()

module FocusBrushing =
    open IntelliFactory.WebSharper.Html

    type Margin =
        {
            Top    : float
            Right  : float
            Bottom : float
            Left   : float
        }

    type DataRow =
        {
            Date  : EcmaScript.Date
            Price : float
        }

    type Control() =
        inherit Web.Control()

        [<JavaScript>]
        override this.Body =
            let margin  = { Top = 10.; Right = 10.; Bottom = 100.; Left = 40. }
            let margin2 = { Top = 430.; Right = 10.; Bottom = 20.; Left = 40. }
            let width   = 960. - margin.Left - margin.Right
            let height  = 500. - margin.Top - margin.Bottom
            let height2 = 500. - margin2.Top - margin2.Bottom

            let x  = D3.Time.Scale().Range([|0.; width|])
            let x2 = D3.Time.Scale().Range([|0.; width|])
            let y  = D3.Scale.Linear().Range([|height ; 0.|])
            let y2 = D3.Scale.Linear().Range([|height2; 0.|])

            let xAxis  = D3.Svg.Axis().Scale(x) .Orient(Orientation.Bottom)
            let xAxis2 = D3.Svg.Axis().Scale(x2).Orient(Orientation.Bottom)
            let yAxis  = D3.Svg.Axis().Scale(y) .Orient(Orientation.Left  )

            let brush =
                D3.Svg.Brush()
                    .X(x2)

            let area =
                D3.Svg.Area()
                    .Interpolate(Interpolation.Monotone)
                    .X(fun d -> x.Apply(d.Date))
                    .Y0(float height)
                    .Y1(fun d -> y.Apply(d.Price))

            let area2 =
                D3.Svg.Area()
                    .Interpolate(Interpolation.Monotone)
                    .X(fun d -> x2.Apply(d.Date))
                    .Y0(float height2)
                    .Y1(fun d -> y2.Apply(d.Price))

            let svg =
                D3.Select("body").Append("svg")
                    .Attr("width", width + margin.Left + margin.Right)
                    .Attr("height", height + margin.Top + margin.Bottom);

            svg.Append("defs").Append("clipPath")
                .Attr("id", "clip")
                .Append("rect")
                .Attr("width", width)
                .Attr("height", height)
                |> ignore

            let focus =
                svg.Append("g")
                    .Attr("transform", SvgTransform.Translate(margin.Left, margin.Top))

            let context =
                svg.Append("g")
                    .Attr("transform", SvgTransform.Translate(margin2.Left, margin2.Top))

            let brushed() =
                x.Domain(if brush.Empty() then x2.Domain() else As <| brush.Extent()) |> ignore
                focus.Select("path").Attr("d", area) |> ignore
                focus.Select(".x.axis") |> xAxis.Apply

            brush.On(BrushEvent.Brush, brushed) |> ignore

            D3.Csv("sp500.csv", fun data ->

                let parseDate = D3.Time.Format("%b %Y").Parse

                let parsedData =
                    data |> Array.map (fun d ->
                        {
                            Date  = parseDate(d?date)
                            Price = +d?price
                        }
                    )

                x.Domain(As <| D3.Extent(parsedData, fun d -> d.Date)) |> ignore
                y.Domain([|0.; D3.Max(parsedData, fun d -> d.Price)|]) |> ignore
                x2.Domain(x.Domain()) |> ignore
                y2.Domain(y.Domain()) |> ignore

                focus.Append("path")
                    .Datum(parsedData)
                    .Attr("clip-path", "url(#clip)")
                    .Attr("d", area)
                    |> ignore

                focus.Append("g")
                    .Attr("class", "x axis")
                    .Attr("transform", SvgTransform.Translate(0., height))
                    |> xAxis.Apply

                focus.Append("g")
                    .Attr("class", "y axis")
                    |> yAxis.Apply

                context.Append("path")
                    .Datum(parsedData)
                    .Attr("d", area2)
                    |> ignore

                context.Append("g")
                    .Attr("class", "x axis")
                    .Attr("transform", SvgTransform.Translate(0., height2))
                    |> xAxis2.Apply

                context.Append("g")
                    .Attr("class", "x brush")
                    .Call(brush.Apply)
                  .SelectAll("rect")
                    .Attr("y", -6)
                    .Attr("height", height2 + 7.)
                    |> ignore
            )

            upcast Div []


module AreaChart =
    open IntelliFactory.WebSharper.Html

    type DataRow =
        {
            Date  : EcmaScript.Date
            Close : float
        }

    type Control() =
        inherit Web.Control()

        [<JavaScript>]
        override this.Body =
            let top, right, bottom, left = 20., 20., 30., 50.
            let width = 960. - left - right
            let height = 500. - top - bottom

            let x = D3.Time.Scale().Range([| 0.; width |])
            let y = D3.Scale.Linear().Range([| height; 0. |])

            let xAxis = D3.Svg.Axis().Scale(x).Orient(Orientation.Bottom)
            let yAxis = D3.Svg.Axis().Scale(y).Orient(Orientation.Left)

            let area =
                D3.Svg.Area()
                    .X(fun d -> x.Apply(d.Date))
                    .Y0(float height)
                    .Y1(fun d -> y.Apply(d.Close))

            let svg =
                D3.Select("body").Append("svg")
                    .Attr("width", width + left + right)
                    .Attr("height", height + top + bottom)
                  .Append("g")
                    .Attr("transform", SvgTransform.Translate(left, top))

            D3.Tsv("data.tsv", fun data ->
                let parseDate = D3.Time.Format("%d-%b-%y").Parse

                let parsedData =
                    Array.map (fun d ->
                        {
                            Date  = parseDate(d?date)
                            Close = float d?close
                        }
                    ) data

                x.Domain(As <| D3.Extent(parsedData, fun d -> d.Date))   |> ignore
                y.Domain([| 0.; D3.Max(parsedData, fun d -> d.Close) |]) |> ignore

                svg.Append("path")
                    .Datum(parsedData)
                    .Attr("class", "area")
                    .Attr("d", area)
                    |> ignore

                svg.Append("g")
                    .Attr("class", "x axis")
                    .Attr("transform", SvgTransform.Translate(0., height))
                    .Call(xAxis.Apply)
                    |> ignore

                svg.Append("g")
                    .Attr("class", "y axis")
                    .Call(yAxis.Apply)
                  .Append("text")
                    .Attr("transform", SvgTransform.Rotate(-90.))
                    .Attr("y", 6)
                    .Attr("dy", ".71em")
                    .Style("text-anchor", "end")
                    .Text("Price ($)")
                    |> ignore
            )

            upcast Div []

module Snake =
    open IntelliFactory.WebSharper.Html
    open System.Collections.Generic
    open IntelliFactory.WebSharper.JQuery

    type Control() =
        inherit Web.Control()

        [<JavaScript>]
        override this.Body =
            let snake = [| 4, 4 ; 3, 4 ; 2, 4 |]
            let food = [| 4, 4 |]
            let direction = 1, 0
            //let nextMoves = [| |]
             //let iterval_id = JavaScript.SetInterval(tick, 100)
            let gridsize = 40
                        
            let scale =
                D3.Scale.Ordinal()
                    .Domain(D3.Range(gridsize))
                    .RangeRoundBands((0., float <| JQuery.Of("svg").Height()), 0.)

            let update_snake() =
                let svg = D3.Select("svg")
                let cells = 
                    svg.SelectAll("rect.snake")
                        .Data(As<obj[]> snake, fun (d, _) -> Json.Stringify(d))

                cells.Enter()
                   .Append("rect")
                   .Attr("class", "snake")
                   .Attr("width", scale.RangeBand())
                   .Attr("height", scale.RangeBand())
                   .Attr("x", fun (x, _) -> scale.Apply(As<int> x))
                   .Attr("y", fun (_, y) -> scale.Apply(As<int> y))
                   |> ignore

                cells.Exit().Remove()

            upcast Div []

type Action =
    | Home

module Skin =
    open System.Web

    let MainTemplate =
        Content.Template<Content.HtmlElement list>("~/Main.html")
            .With("body", id)

    let WithTemplate body : Content<Action> =
        Content.WithTemplate MainTemplate body

module Site =
    open IntelliFactory.Html

    let HomePage =
        Skin.WithTemplate <| fun ctx ->
            [
                Div [ new FocusBrushing.Control() ]
            ]

    let Main =
        Sitelet.Sum [
            Sitelet.Content "/" Home HomePage
        ]

[<Sealed>]
type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Main
        member this.Actions = [ Home ]

[<assembly: Website(typeof<Website>)>]
do ()
