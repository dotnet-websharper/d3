namespace Site.CompaniesGraph

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.JQuery
open IntelliFactory.WebSharper.D3
open Site

#nowarn "40"

/// Visualizes consumer companies as a force graph where links are
/// industry connections. Company size is proportional to revenue.
/// Data taken from FreeBase.
[<JavaScript>]
module UI =

    module Data =

        let Load () =
            Async.FromContinuations (fun (ok, _, _) ->
                JQuery.GetJSON("/" + Data.FileName, fun (x, _) ->
                    ok (x :?> Data.DataSet))
                |> ignore)

    type Config<'T> =
        {
            CanvasHeight : double
            CanvasWidth : double
            DataSet : Graphs.DataSet<'T>
            OnMouseOver : 'T -> unit
            Radius : 'T -> double
        }

    [<JavaScript>]
    let Render config =
        let height = config.CanvasHeight
        let width = config.CanvasWidth
        let forceNodes =
            config.DataSet.Nodes
            |> Array.map (fun (Graphs.Node (i, label)) ->
                let node = ForceNode.Create()
                node.Index <- i
                node?Label <- label
                node)
            |> Array.sortBy (fun node -> node.Index)
        let forceLinks =
            config.DataSet.Links
            |> Array.map (fun (Graphs.Link (s, t)) ->
                let lnk = Link<ForceNode>.Create()
                lnk.Source <- forceNodes.[s]
                lnk.Target <- forceNodes.[t]
                lnk)
        let rec tick () =
            link.Attr("x1", fun (d, i) -> As<Link<ForceNode>>(d).Source.X)
                .Attr("y1", fun (d, i) -> As<Link<ForceNode>>(d).Source.Y)
                .Attr("x2", fun (d, i) -> As<Link<ForceNode>>(d).Target.X)
                .Attr("y2", fun (d, i) -> As<Link<ForceNode>>(d).Target.Y)
            |> ignore
            node.Attr("cx", fun (d, i) -> forceNodes.[i].X)
                .Attr("cy", fun (d, i) -> forceNodes.[i].Y)
                .Attr("r", fun (d, i) -> config.Radius forceNodes.[i]?Label)
            |> ignore
        and force =
            D3.Layout.Force()
                .Nodes(forceNodes)
                .Links(forceLinks)
                .Size(width, height)
                .On(ForceEvent.Tick, tick)
                .Start()
        and svg =
            D3.Select("body").Append("svg")
                .Attr("class", "CompaniesGraph")
                .Attr("width", width)
                .Attr("height", height)
        and node : Selection =
            svg.SelectAll(".node")
                .Data(As<obj[]> forceNodes)
                .Enter()
                .Append("circle")
                    .Attr("class", "node")
                .Call(force.Drag)
                .On("mouseover", fun (d, i) ->
                    config.OnMouseOver(d?Label))
        and link : Selection =
            svg.SelectAll(".link")
                .Data(As<obj[]> forceLinks)
                .Enter()
                .Insert("line", ".node")
                .Attr("class", "link")
        ()

    [<JavaScript>]
    let Start (el: Element) =
        async {
            let! data = Data.Load()
            return Render {
                CanvasHeight = 500.
                CanvasWidth = 960.
                DataSet = data
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
                    el.Text <- t
            }
        }
        |> Async.Start

    let Show (ctx: Dom.Element) =
        let label = Div []
        let main =
            Div [
                Span [Text "In Focus: "]
                label
            ]
            |>! OnAfterRender (fun _ ->
                Start label)
        ctx.AppendChild(main.Body) |> ignore
        (main :> IPagelet).Render()

    let Sample =
        Samples.Build(__SOURCE_FILE__)
            .Title("CompaniesGraph")
            .Keywords(["force"])
            .Render(Show)
            .Create()
