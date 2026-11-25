// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2026 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
namespace Site

open WebSharper
open WebSharper.JavaScript
open WebSharper.D3

/// A tour of the d3-shape surface: the `pie`/`arc` generators, `symbol` glyphs,
/// a `stack`ed area chart, and a `line` generator over point/linear/sequential
/// and diverging scales.
[<JavaScript>]
module Shapes =

    let Width = 900.
    let Height = 520.

    let Palette = [| "#6366f1"; "#22d3ee"; "#f59e0b"; "#10b981"; "#ef4444"; "#a855f7" |]

    let private palette i = Palette.[i % Palette.Length]

    // Selections typed as `Selection<obj>` hand their datum back as `obj`, so
    // the attribute callbacks below recover the concrete datum shape.
    let private asArc (d: obj) : PieArcDatum<float> = As d
    let private asFloat (d: obj) : float = As d

    let PieAndArc (svg: Selection<obj>) =
        let pieData = [| 3.; 5.; 2.; 4.; 6. |]
        let arcs = D3.Pie<float>().Apply(pieData)
        let arc =
            D3.Arc<PieArcDatum<float>>()
                .InnerRadius(46.)
                .OuterRadius(104.)
                .CornerRadius(4.)
                .PadAngle(0.02)

        let donut = svg.Append("g").Attr("transform", "translate(140,150)")

        donut.SelectAll("path")
            .Data(arcs)
            .Enter()
            .Append("path")
            .AttrFn("d", fun d -> arc.Apply(asArc d))
            .AttrFn("fill", fun d -> palette (asArc d).Index)
            .Attr("stroke", "#ffffff")
            .Attr("stroke-width", 2)
        |> ignore

        donut.SelectAll("text")
            .Data(arcs)
            .Enter()
            .Append("text")
            .Attr("text-anchor", "middle")
            .Attr("dy", "0.35em")
            .Attr("fill", "#ffffff")
            .Attr("font-size", 13)
            .AttrFn("x", fun d ->
                let (x, _) = arc.Centroid(asArc d)
                x)
            .AttrFn("y", fun d ->
                let (_, y) = arc.Centroid(asArc d)
                y)
            .Text(fun (d: obj) (_: int) -> string (int (asArc d).Value))
        |> ignore

        donut.Append("text")
            .Attr("text-anchor", "middle")
            .Attr("y", 140)
            .Attr("fill", "currentColor")
            .Attr("font-size", 12)
            .Text(sprintf "pie of %d slices" pieData.Length)
        |> ignore

    let Symbols (svg: Selection<obj>) =
        let group = svg.Append("g").Attr("transform", "translate(300,20)")
        let symbol = D3.Symbol<obj>().Size(520.)
        D3.Symbols
        |> Array.iteri (fun i (st: SymbolType) ->
            group.Append("path")
                .Attr("transform", sprintf "translate(%d,50)" (i * 58))
                .Attr("d", symbol.Type(st).Apply(null))
                .Attr("fill", palette i)
            |> ignore)

    let StackedAreas (svg: Selection<obj>) =
        let rows : obj[] =
            [|
                New [ "apples" => 10; "oranges" => 6; "bananas" => 3 ]
                New [ "apples" => 8; "oranges" => 9; "bananas" => 5 ]
                New [ "apples" => 5; "oranges" => 4; "bananas" => 9 ]
                New [ "apples" => 11; "oranges" => 3; "bananas" => 6 ]
                New [ "apples" => 7; "oranges" => 8; "bananas" => 4 ]
            |]
        let keys = [| "apples"; "oranges"; "bananas" |]
        let series =
            D3.Stack<obj>()
                .Keys(keys)
                .Offset(D3.StackOffsetNone)
                .Order(D3.StackOrderNone)
                .Apply(rows)

        let x =
            D3.ScaleBand<int>()
                .Domain([| 0 .. rows.Length - 1 |])
                .Range([| 0.; 336. |])
                .PaddingInner(0.2)

        let area =
            D3.Area<SeriesPoint>()
                .X(fun (_: SeriesPoint) i _ -> x.Apply(i) + x.Bandwidth() / 2.)
                .Y0(fun (p: SeriesPoint) _ _ -> -(p.Y0()) * 7.)
                .Y1(fun (p: SeriesPoint) _ _ -> -(p.Y1()) * 7.)
                .Curve(D3.CurveMonotoneX)

        let group = svg.Append("g").Attr("transform", "translate(300,360)")
        series
        |> Array.iteri (fun i (s: SeriesPoint[]) ->
            group.Append("path")
                .Attr("d", area.Apply(s))
                .Attr("fill", palette (i + 1))
                .Attr("opacity", 0.82)
            |> ignore)

    let LineAndScales (svg: Selection<obj>) =
        let values = [| 12.; 19.; 7.; 15.; 22.; 9. |]
        let x = D3.ScalePoint<float>().Domain(values).Range([| 0.; 300. |]).Padding(0.35)
        let y = D3.ScaleLinear().Domain([| 0.; 24. |]).Range([| 100.; 0. |])

        let color =
            D3.ScaleSequential<string>()
                .Domain([| 0.; 24. |])
                .Interpolator(fun t -> D3.InterpolateTurbo(t))

        let line =
            D3.Line<float>()
                .X(fun (d: float) _ _ -> x.Apply(d))
                .Y(fun (d: float) _ _ -> y.Apply(d))
                .Curve(D3.CurveCatmullRom.Alpha(0.2))

        let group = svg.Append("g").Attr("transform", "translate(620,120)")
        group.Append("path")
            .Attr("d", line.Apply(values))
            .Attr("fill", "none")
            .Attr("stroke", "#6366f1")
            .Attr("stroke-width", 3)
        |> ignore

        group.SelectAll("circle")
            .Data(values)
            .Enter()
            .Append("circle")
            .AttrFn("cx", fun d -> x.Apply(asFloat d))
            .AttrFn("cy", fun d -> y.Apply(asFloat d))
            .Attr("r", 5)
            .AttrFn("fill", fun d -> color.Apply(asFloat d))
        |> ignore

        // A diverging scale drives the colour legend strip.
        let diverging =
            D3.ScaleDiverging<string>()
                .Domain([| -1.; 0.; 1. |])
                .Range([| "#ef4444"; "#f8fafc"; "#22d3ee" |])

        group.SelectAll("rect")
            .Data([| -1. .. 0.25 .. 1. |])
            .Enter()
            .Append("rect")
            .AttrFn("x", fun d -> 60. + (asFloat d + 1.) / 2. * 180.)
            .Attr("y", 130)
            .Attr("width", 20)
            .Attr("height", 18)
            .AttrFn("fill", fun d -> diverging.Apply(asFloat d))
        |> ignore

    let Render (ctx: Dom.Element) =
        let svg =
            D3.Select(ctx)
                .Append("svg")
                .Attr("viewBox", "0 0 " + string Width + " " + string Height)
                .Attr("width", Width)
                .Attr("height", Height)
                .Attr("role", "img")
        PieAndArc svg
        Symbols svg
        StackedAreas svg
        LineAndScales svg

    let Sample =
        Samples.Build()
            .Id("Shapes")
            .FileName(__SOURCE_FILE__)
            .Keywords(["shape"; "arc"; "pie"; "symbol"; "stack"; "line"])
            .Render(Render)
            .Create()