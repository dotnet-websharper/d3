#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.D3", "3.0-alpha")
        .References(fun r -> [r.Assembly "System.Web"])
    |> fun bt -> bt.WithFramework(bt.Framework.Net40)

let main =
    bt.WebSharper.Extension("IntelliFactory.WebSharper.D3")
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
                Description = "WebSharper Extensions for Google Maps d3v3"
                RequiresLicenseAcceptance = true })
        .Add(main)

]
|> bt.Dispatch
