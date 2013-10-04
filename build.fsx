#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.D3", "2.5")
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
        .Description("WebSharper bindings to D3 (d3js.org) version d3v3.")
        .ProjectUrl("http://github.com/intellifactory/websharper.d3")
        .Configure(fun c ->
            {
                c with
                    Authors = ["IntelliFactory"]
                    Id = "WebSharper.D3"
                    LicenseUrl = Some "http://github.com/intellifactory/websharper.d3/blob/master/LICENSE.md"
                    RequiresLicenseAcceptance = true
                    Title = Some "WebSharper.D3 (d3v3)"
            })
        .Add(main)

]
|> bt.Dispatch
