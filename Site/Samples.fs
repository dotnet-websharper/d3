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
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html

/// Support code for the sample catalog.
[<JavaScript>]
module Samples =

    type Sample =
        private {
            FileName : string
            Id : string
            Keywords : list<string>
            Render : Dom.Element -> unit
            Title : string
        }

    let private ( ++ ) a b =
        match a with
        | Some _ -> a
        | None -> b

    let private req name f =
        match f with
        | None -> failwith ("Required property not set: " + name)
        | Some r -> r

    type Builder =
        private {
            mutable BFileName : option<string>
            mutable BId : option<string>
            mutable BKeywords : list<string>
            mutable BRender : option<Dom.Element -> unit>
            mutable BTitle : option<string>
        }

        member b.Create() =
            let id = req "Id" (b.BId ++ b.BTitle)
            let title = defaultArg (b.BTitle ++ b.BId) "Sample"
            {
                FileName = req "FileName" b.BFileName
                Id = id
                Keywords = b.BKeywords
                Render = req "Render" b.BRender
                Title = title
            }

        member b.FileName(x) = b.BFileName <- Some x; b
        member b.Id(x) = b.BId <- Some x; b
        member b.Keywords(x) = b.BKeywords <- x; b
        member b.Render(x) = b.BRender <- Some x; b
        member b.Title(x) = b.BTitle <- Some x; b

    let Build () =
        {
            BId = None
            BFileName = None
            BKeywords = []
            BRender = None
            BTitle = None
        }

    type Sample with

        /// The "About" pane: the sample's description, cloned from the hidden
        /// `.sample-data` block in index.html, plus a source link.
        member s.SideDoc =
            let url = "https://github.com/dotnet-websharper/d3/blob/main/Site/" + s.FileName
            Doc.Concat [
                div [
                    on.afterRender (fun self ->
                        match JS.Document.GetElementById(s.Id) with
                        | null -> ()
                        | el ->
                            let copy = el.CloneNode(true) :?> Dom.Element
                            copy.Attributes.RemoveNamedItem("id") |> ignore
                            self.AppendChild(copy) |> ignore)
                ] []
                a [attr.``class`` "source-link"; attr.href url] [text "View source"]
            ]

        /// A Doc that renders the sample into a fresh container once mounted.
        member s.MainDoc =
            div [ on.afterRender (fun el -> s.Render el) ] []

    type Set =
        private
        | Set of list<Sample>

        static member Create(ss) = Set [for (Set xs) in ss do yield! xs]
        static member Singleton(s) = Set [s]

        /// Fills the template's tab bar, canvas and sidebar slots, showing the
        /// first sample.
        member s.Show (navs: Var<Doc>) (main: Var<Doc>) (side: Var<Doc>) =
            let (Set samples) = s
            let show (s: Sample) (dom: Dom.Element) =
                let j = JS.Document.QuerySelectorAll("#sample-navs ul li") //("li").RemoveClass("active")
                j.ForEach((fun (node, _, _, _) ->
                    let elem = node :?> Dom.Element
                    elem.ClassList.Remove "active"
                ), null)
                dom.ClassList.Add "active"
                main.Value <- s.MainDoc
                side.Value <- s.SideDoc
            navs.Value <-
                ul [Attr.Class "sample-tabs"] [
                    samples
                    |> List.mapi (fun i s ->
                        li [
                            on.afterRender (fun self -> if i = 0 then show s self)
                            on.click (fun self _ -> show s self)
                        ] [
                            a [attr.href "#"] [text s.Title]
                        ])
                    |> Doc.Concat
                ]
