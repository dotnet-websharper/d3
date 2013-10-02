#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("IntelliFactory.WebSharper.D3", "2.5")
        .References(fun r -> [r.Assembly "System.Web"])

let main =
    bt.WebSharper.Extension("IntelliFactory.WebSharper.D3")
        .SourcesFromProject()
        .Embed(["d3.v3.min.js"])

let test =
    bt.WebSharper.Library("IntelliFactory.WebSharper.D3.Tests")
        .SourcesFromProject()
        .References(fun r -> [r.Project main])

let web =
    bt.WebSharper.HostWebsite("Web")
        .References(fun r ->
            [
                r.Project(main)
                r.Project(test)
            ])

bt.Solution [

    main
    test
    web

    bt.NuGet.CreatePackage()
        .Description("Bindings to D3")
        .Add(main)

]
|> bt.Dispatch
