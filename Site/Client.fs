// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
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
open WebSharper.D3

[<JavaScript>]
module Client =

    type Data =
        {
            date: System.DateTime
            close: float
        }

    let LineChartSample =
        let width = 928.
        let height = 500.
        let marginTop = 20.
        let marginRight = 30.
        let marginBottom = 30.
        let marginLeft = 40.

        let aapl : Data array =
            let startDate = System.DateTime(2021,04,12)
            let daysToGenerate = 640
            let getValForD (d: int) =
                350. + float (System.Random().Next(-50, 50))
            [|
                for d in [0..daysToGenerate-1] do
                    {
                        date = startDate.AddDays (2. * float d)
                        close = getValForD d
                    }
            |]

        let x = D3.ScaleUtc(D3.Extent(aapl, fun d -> d.date), [|marginLeft; width - marginRight|])

        let y = D3.ScaleLinear([|0; 700|], [|height - marginBottom; marginTop|])

        let line =
            D3.Line()
                .X(fun d -> x.Get(d.date))
                .Y(fun d -> y.Get(d.close))

        let svg =
            D3.Create("svg")
                .Attr("width", width)
                .Attr("height", height)
                .Attr("viewBox", [0, 0, width, height])
                .Attr("style", "max-width: 100%; height: auto; height: intrinsic;");

        svg.Append("g")
            .Attr("transform", $"translate(0,{height - marginBottom})")
            .Call(D3.AxisBottom(x).Ticks(int width / 80).TickSizeOuter(0))
            |> ignore

        svg.Append("g")
            .Attr("transform", $"translate({marginLeft},0)")
            .Call(D3.AxisLeft(y).Ticks(int height / 40))
            .Call(fun g -> g.Select(".domain").Remove())
            .Call(fun g ->
                g.SelectAll(".tick line")
                    .Clone()
                    .Attr("x2", width - marginLeft - marginRight)
                    .Attr("stroke-opacity", 0.1))
            .Call(fun g -> 
                g.Append("text")
                    .Attr("x", -marginLeft)
                    .Attr("y", 10)
                    .Attr("fill", "currentColor")
                    .Attr("text-anchor", "start")
                    .Text("↑ Daily close ($)"))
            |> ignore

        svg.Append("path")
            .Attr("fill", "none")
            .Attr("stroke", "steelblue")
            .Attr("stroke-width", 1.5)
            .Attr("d", line.Generate(aapl))
            |> ignore

        svg.Node()

    [<SPAEntryPoint>]
    let Main() =
        WebSharper.JavaScript.JS.Document.GetElementById "main"
        |> fun m ->
            m.AppendChild(LineChartSample)
