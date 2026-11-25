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
open WebSharper.UI.Templating

[<JavaScript>]
module Client =

    // The template is the whole document, loaded from the DOM, so editing
    // index.html and refreshing the browser is enough.
    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    // ------------------------------------------------------------------
    // Theme (light by default, choice persisted in localStorage)
    // ------------------------------------------------------------------

    // Keep in sync with the bootstrap script in index.html.
    let themeStorageKey = "websharper-d3-theme"

    let theme = Var.Create "light"
    let themeLabel = Var.Create "🌙 Dark mode"

    let applyTheme () =
        JS.Document.DocumentElement.SetAttribute("data-theme", theme.Value)
        themeLabel.Value <-
            if theme.Value = "dark" then "☀️ Light mode" else "🌙 Dark mode"

    let toggleTheme () =
        theme.Value <- (if theme.Value = "dark" then "light" else "dark")
        try JS.Window.LocalStorage.SetItem(themeStorageKey, theme.Value) with _ -> ()
        applyTheme ()

    // ------------------------------------------------------------------
    // Sample slots: the catalog swaps these in place.
    // ------------------------------------------------------------------

    let navs = Var.Create Doc.Empty
    let main = Var.Create Doc.Empty
    let side = Var.Create Doc.Empty

    let All =
        let ( !+ ) x = Samples.Set.Singleton(x)
        Samples.Set.Create [
            !+ Circles.Sample
            !+ Shapes.Sample
            !+ WorldTour.Sample
        ]

    [<SPAEntryPoint>]
    let Main () =
        try
            match JS.Window.LocalStorage.GetItem(themeStorageKey) with
            | null -> ()
            | stored when stored = "dark" || stored = "light" -> theme.Value <- stored
            | _ -> ()
        with _ -> ()
        applyTheme ()

        IndexTemplate()
            .themeLabel(themeLabel.V)
            .ToggleTheme(fun _ -> toggleTheme ())
            .Navs(navs.V)
            .Main(main.V)
            .Side(side.V)
            .Bind()

        All.Show navs main side