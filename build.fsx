#load "paket-files/wsbuild/intellifactory/websharper/tools/WebSharper.Fake.fsx"
open Fake
open WebSharper.Fake

let targets =
    WSTargets.Default (fun () -> GetSemVerOf "WebSharper" |> ComputeVersion)
    |> MakeTargets

Target "Build" DoNothing
targets.BuildDebug ==> "Build"

Target "CI-Release" DoNothing
targets.CommitPublish ==> "CI-Release"

RunTargetOrDefault "Build"
