#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.D3")
        .VersionFrom("WebSharper")
        .References(fun r -> [r.Assembly "System.Web"])
        .WithFramework(fun fw -> fw.Net40)

let main =
    bt.WebSharper4.Extension("WebSharper.D3")
        .Embed(["d3.v3.min.js"])
        .SourcesFromProject()

bt.Solution [
    main

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "WebSharper.D3-d3v3"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/websharper.d3"
                Description = "WebSharper Extensions for D3 3.3.6"
                RequiresLicenseAcceptance = true })
        .Add(main)

]
|> bt.Dispatch
