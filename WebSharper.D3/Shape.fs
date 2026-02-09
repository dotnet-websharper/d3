namespace WebSharper.D3.Impl

open WebSharper.InterfaceGenerator
open WebSharper.InterfaceGenerator.Pervasives

module Shape =

    let Line =
        Generic - fun t ->
        Class "Line"
        |=> Implements [Base.Shape]
        |+> Instance [
            "x" => (t ^-> T<float>) ^-> TSelf[t]
            "y" => (t ^-> T<float>) ^-> TSelf[t]
            "generate" => (!| t)?data ^-> T<obj>
            |> WithInline "$this($data)"
        ]