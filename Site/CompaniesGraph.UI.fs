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
namespace Site.CompaniesGraph

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.D3
open Site

#nowarn "40"

/// Visualizes consumer companies as a force graph where links are
/// industry connections. Company size is proportional to revenue.
/// Data taken from FreeBase.
[<JavaScript>]
module UI =

    module Data =

        let sendXHR typE url (data:string) callback =
            let newXHR = JavaScript.XMLHttpRequest()
            newXHR.Open(typE, url, true)
            newXHR.Send(data)
            newXHR.Onreadystatechange =
                if newXHR.Status = 200 && newXHR.ReadyState = 4 then
                    callback(newXHR.ResponseText) else null

        let Load () =
            Async.FromContinuations (fun (ok, _, _) ->
                let xhrFun response =
                    JSON.Parse(response) |> ignore
                    null
                sendXHR "GET" Data.FileName null xhrFun
                |> ignore)

    type Config<'T> =
        {
            CanvasHeight : double
            CanvasWidth : double
            DataSet : Graphs.DataSet<'T>
            OnMouseOver : 'T -> unit
            Parent : Dom.Element
            Radius : 'T -> double
        }

    let Render config =
        let height = config.CanvasHeight
        let width = config.CanvasWidth
        let forceNodes : ForceNode[] =
            config.DataSet.Nodes
            |> Array.map (fun (Graphs.Node (i, label)) ->
                let node = ForceNode(Index = i)
                node?Label <- label
                node)
            |> Array.sortBy (fun node -> node.Index)
        let forceLinks =
            config.DataSet.Links
            |> Array.map (fun (Graphs.Link (s, t)) ->
                Link(Source = forceNodes.[s], Target = forceNodes.[t]))
        let rec force =
            D3.Layout.Force()
                .Nodes(forceNodes)
                .Links(forceLinks)
                .Size(width, height)
                .On(ForceEvent.Tick, fun () ->
                    link.AttrFn("x1", fun (d: Link<ForceNode>) -> d.Source.X)
                        .AttrFn("y1", fun (d: Link<ForceNode>) -> d.Source.Y)
                        .AttrFn("x2", fun (d: Link<ForceNode>) -> d.Target.X)
                        .AttrFn("y2", fun (d: Link<ForceNode>) -> d.Target.Y)
                    |> ignore
                    node.AttrFn("cx", fun (d: ForceNode) -> d.X)
                        .AttrFn("cy", fun (d: ForceNode) -> d.Y)
                        .AttrFn("r", fun d -> config.Radius d?Label)
                    |> ignore)
                .Start()
        and svg =
            D3.Select(config.Parent).Append("svg")
                .Attr("class", "CompaniesGraph")
                .Attr("width", width)
                .Attr("height", height)
        and node : Selection<ForceNode> =
            svg.SelectAll(".node")
                .Data(forceNodes)
                .Enter()
                .Append("circle")
                    .Attr("class", "node")
                .Call(force.Drag)
                .On("mouseover", fun (d, i) ->
                    config.OnMouseOver(d?Label))
        and link : Selection<Link<ForceNode>> =
            svg.SelectAll(".link")
                .Data(forceLinks)
                .Enter()
                .Insert("line", ".node")
                .Attr("class", "link")
        link.Attr("x1", 0)
        |> ignore
        ()

    let Start (parent: Dom.Element) (out: Dom.Element) =
        async {
            let! data = Data.Load()
            return Render {
                CanvasHeight = 500.
                CanvasWidth = 960.
                DataSet = data
                Parent = parent
                Radius = fun label ->
                    match label with
                    | Data.Company (n, 0.0) -> 4.
                    | Data.Company (n, r) -> max 4. (sqrt r / 30000.)
                    | Data.Industry _ -> 4.
                OnMouseOver = fun label ->
                    let t =
                        match label with
                        | Data.Company (n, r) -> n + " (revenue: " + string r + ")"
                        | Data.Industry i -> i
                    out.TextContent <- t
            }
        }
        |> Async.Start

    let Show (ctx: Dom.Element) =
        let label = div [] [] :?> Elt
        let main =
            div [
                on.afterRender (fun parent ->
                    Start parent label.Dom)
            ] [
                span [] [text "In Focus: "]
                label
            ] :?> Elt
        ctx.AppendChild(main.Dom) |> ignore

    let Sample =
        Samples.Build()
            .Id("CompaniesGraph")
            .FileName(__SOURCE_FILE__)
            .Keywords(["force"])
            .Render(Show)
            .Create()
