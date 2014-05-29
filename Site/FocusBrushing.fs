(*
    Translation of http://bl.ocks.org/mbostock/1667367
*)

[<ReflectedDefinition>]
module Site.FocusBrushing

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.D3

type Margin =
    {
        Top : float
        Right : float
        Bottom : float
        Left : float
    }

type DataRow =
    {
        Date : EcmaScript.Date
        Price : float
    }

let private Render (canvas: Dom.Element) =
    let margin  = { Top = 10.; Right = 10.; Bottom = 100.; Left = 40. }
    let margin2 = { Top = 430.; Right = 10.; Bottom = 20.; Left = 40. }
    let width   = 960. / 2. - margin.Left - margin.Right
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
        D3.Select(canvas).Append("svg")
            .Attr("class", "FocusBrushing")
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
            .Call(fun s -> brush.Apply(s))
            .SelectAll("rect")
                .Attr("y", -6)
                .Attr("height", height2 + 7.)
                |> ignore)

let Sample =
    Samples.Build(__SOURCE_FILE__)
        .Title("FocusBrushing")
        .Render(Render)
        .Create()
