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

open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.D3

/// Let us get started with the famous D3!
[<JavaScript>]
module Circles =

    [<Inline "+$x">]
    let inline double (x: 'T) = double x

    /// Imagine some sample data (from 10 to 100 with a step of 10).
    /// This could be coming from anywhere, including server-side F#.
    let Data = [| 10.0 .. 10.0 .. 100.0 |]

    /// D3 operates in terms of "joins".
    /// Roughly, a join syncs a data collection to a DOM element collection, 1:1.
    /// Existing elements are updated with their matching data points.
    /// For data points without a matching element, new elements are created ("enter").
    /// Elements without a matching data points, typically are removed ("exit").
    let Join (data: double[]) (context: Dom.Element) =

        /// First, setup a context.
        let ctx =
            D3.Select(context) // select the element
                .Append("svg") // append a new <svg/> and focus on it
                .Attr("height", 500) // give a height to the <svg/>; width = auto

        /// Let us define the join.
        /// Select some elements (SVG circles) and the data set.
        /// Note that WebSharper types the result as `Selection<double>`
        /// since you gave a `double[]` - to help you with types later on.
        let joined = ctx.SelectAll("circle").Data(data)

        /// Now, "enter" selection describes what happens to elements that
        /// enter the theater stage so to speak, that is, how do we create
        /// elements for data points that do not have an element yet.
        /// Here we create circles and set some attributes, dependent on data.
        joined.Enter()
            .Append("circle")
            .AttrFn("cx", fun x -> x * 5.) // center x coord
            .AttrFn("cy", fun x -> 50. + x * x / 40.) // center y coord
            .AttrIx("r", fun (x, i) -> // radius as a function of data point and index
                let p = double i / double data.Length
                7. + 5. * sin (3. * p * Math.PI))
        |> ignore

        /// If you look at D3.js examples, you will not
        /// find ".AttrFn" and ".AttrIx".
        /// In this WebSharper binding, these are all synonyms of ".Attr",
        /// but it is helpful to distinguish for better type inference.

    /// You may ignore this bit here - it simply registers this
    /// with the viewer at the WebSharper.D3 site.
    let Sample =
        Samples.Build()
            .Id("Circles")
            .FileName(__SOURCE_FILE__)
            .Keywords(["basic"])
            .Render(Join Data)
            .Create()
