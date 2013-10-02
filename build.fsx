#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("IntelliFactory.WebSharper.D3", "2.5")
        .References(fun r -> [r.Assembly "System.Web"])

let main =
    bt.WebSharper.Library("IntelliFactory.WebSharper.D3")
        .SourcesFromProject()

let test =
    bt.WebSharper.HtmlWebsite("IntelliFactory.WebSharper.D3.Tests")
        .SourcesFromProject()
        .References(fun r -> [r.Project main])

bt.Solution [

    main
    test

    bt.NuGet.CreatePackage()
        .Description("Bindings to D3")
        .Add(main)

]
|> bt.Dispatch
