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

    let private Clear (el: Dom.Element) =
        while el.HasChildNodes() do
            el.RemoveChild(el.FirstChild) |> ignore

    type Sample with

        member s.Show() =
            let sMain = JS.Document.GetElementById("sample-main")
            let sSide = JS.Document.GetElementById("sample-side")
            Clear sMain
            Clear sSide
            s.Render(sMain)
            let url = "http://github.com/intellifactory/websharper.d3/blob/master/Site/" + s.FileName
            let side =
                div [] [
                    div [
                        on.afterRender (fun self ->
                            match JS.Document.GetElementById(s.Id) with
                            | null -> ()
                            | el ->
                                let copy = el.CloneNode(true)
                                copy.Attributes.RemoveNamedItem("id") |> ignore
                                self.AppendChild(copy) |> ignore) //?
                    ] []
                    a [attr.``class`` "btn btn-primary btn-lg"; attr.href url] [text "Source"]
                ]
            side |> Doc.RunAppendById ("sample-side")

    type Set =
        private
        | Set of list<Sample>

        static member Create(ss) = Set [for (Set xs) in ss do yield! xs]
        static member Singleton(s) = Set [s]

        member s.Show() =
            let (Set samples) = s
            let select (s: Sample) (dom: Dom.Element) =
                let j = JS.Document.QuerySelectorAll("#sample-navs ul li") //("li").RemoveClass("active")
                j.ForEach((fun (node, _, _, _) ->
                    let elem = node :?> Dom.Element
                    elem.ClassList.Remove "active"
                ), null)
                dom.ClassList.Add "active"
                s.Show()
            let rec navs =
                ul [Attr.Class "nav nav-pills"] [
                    samples
                    |> List.mapi (fun i s ->
                        li [
                            attr.href "#"
                            on.afterRender (fun self -> if i = 0 then select s self)
                            on.click (fun self _ -> select s self)
                        ] [
                            text s.Title
                        ])
                    |> Doc.Concat
                ]
            navs |> Doc.RunAppendById ("sample-navs")
