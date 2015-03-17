namespace Site.CompaniesGraph

open WebSharper
open WebSharper.Html.Client
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.D3
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
                JQuery.GetJSON(Data.FileName, fun (x, _) ->
                    ok (x :?> Data.DataSet))
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

    [<JavaScript>]
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
        let rec tick () =
            link.AttrFn("x1", fun (d: Link<ForceNode>) -> d.Source.X)
                .AttrFn("y1", fun (d: Link<ForceNode>) -> d.Source.Y)
                .AttrFn("x2", fun (d: Link<ForceNode>) -> d.Target.X)
                .AttrFn("y2", fun (d: Link<ForceNode>) -> d.Target.Y)
            |> ignore
            node.AttrFn("cx", fun (d: ForceNode) -> d.X)
                .AttrFn("cy", fun (d: ForceNode) -> d.Y)
                .AttrFn("r", fun d -> config.Radius d?Label)
            |> ignore
        and force =
            D3.Layout.Force()
                .Nodes(forceNodes)
                .Links(forceLinks)
                .Size(width, height)
                .On(ForceEvent.Tick, tick)
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

    [<JavaScript>]
    let Start (parent: Dom.Element) (out: Element) =
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
                    out.Text <- t
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
            |>! OnAfterRender (fun parent ->
                Start parent.Dom label)
        ctx.AppendChild(main.Body) |> ignore
        main.Render()

    let Sample =
        Samples.Build()
            .Id("CompaniesGraph")
            .FileName(__SOURCE_FILE__)
            .Keywords(["force"])
            .Render(Show)
            .Create()
