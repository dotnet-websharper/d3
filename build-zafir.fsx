#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("Zafir.D3")
        .VersionFrom("Zafir")
        .References(fun r -> [r.Assembly "System.Web"])
        .WithFramework(fun fw -> fw.Net40)

let main =
    bt.Zafir.Extension("WebSharper.D3")
        .Embed(["d3.v3.min.js"])
        .SourcesFromProject()

bt.Solution [
    main

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "Zafir.D3-d3v3"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/websharper.d3"
                Description = "Zafir Extensions for D3 3.3.6"
                RequiresLicenseAcceptance = true })
        .Add(main)

]
|> bt.Dispatch
