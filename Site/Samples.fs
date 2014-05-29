[<ReflectedDefinition>]
module Site.Samples

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.JQuery
open IntelliFactory.WebSharper.Html

type Sample =
    private {
        FileName : string
        Keywords : list<string>
        Render : Dom.Element -> unit
        Title : string
    }

    static member Create(title, fileName, keywords, render) =
        {
            FileName = fileName
            Keywords = keywords
            Render = render
            Title = title
        }

type Builder =
    private {
        BFileName : string
        mutable BKeywords : list<string>
        mutable BRender : Dom.Element -> unit
        mutable BTitle : string
    }

    member b.Create() =
        Sample.Create(b.BTitle, b.BFileName, b.BKeywords, b.BRender)

    member b.Keywords(x) = b.BKeywords <- x; b
    member b.Render(x) = b.BRender <- x; b
    member b.Title(x) = b.BTitle <- x; b

let Build (fn: string) =
    {
        BFileName = fn
        BKeywords = []
        BRender = ignore
        BTitle = "Sample"
    }

let private Clear (el: Dom.Element) =
    while el.HasChildNodes() do
        el.RemoveChild(el.FirstChild) |> ignore

type Sample with

    member s.Show() =
        let mainLeft = Dom.Document.Current.GetElementById("main-left")
        let mainRight = Dom.Document.Current.GetElementById("main-right")
        Clear mainLeft
        Clear mainRight
        s.Render(mainRight)
        let url = "http://github.com/intellifactory/websharper.d3/blob/master/Site/" + s.FileName
        let label = A [Attr.Class "btn btn-primary btn-lg"; HRef url] -< [Text "Source"]
        label.AppendTo("main-left")

type Set =
    private
    | Set of list<Sample>

    static member Create(ss) = Set [for (Set xs) in ss do yield! xs]
    static member Singleton(s) = Set [s]

    member s.Show() =
        JQuery.JQuery.Of(fun () ->
            let (Set samples) = s
            let doc = Dom.Document.Current
            let select (s: Sample) (dom: Dom.Element) =
                let j = JQuery.Of("#sample-navs ul").Children("li").RemoveClass("active")
                JQuery.Of(dom).AddClass("active").Ignore
                s.Show()
            let rec navs =
                UL [Attr.Class "nav nav-pills"] -< (
                    samples
                    |> List.mapi (fun i s ->
                        LI [A [HRef "#"] -< [Text s.Title]]
                        |>! OnAfterRender (fun self -> if i = 0 then select s self.Dom)
                        |>! OnClick (fun self _ -> select s self.Dom))
                )
            navs.AppendTo("sample-navs"))
