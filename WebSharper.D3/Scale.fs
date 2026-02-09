namespace WebSharper.D3.Impl

open System
open WebSharper.InterfaceGenerator
open WebSharper.InterfaceGenerator.Pervasives

module Scale =

    let Linear =
        Generic - fun t ->
            Class "Linear"
            |=> Implements [Base.Scale]
            |+> Instance [
                "invert" => t ^-> t
                "domain" => !| T<float> ^-> TSelf[t]
                "range" => !| t ^-> TSelf[t]
                "rangeRound" => !| t ^-> TSelf[t]
                "clamp" => T<bool> ^-> TSelf[t]
                "unknown" => t ^-> TSelf[t]
                "interpolate" => t ^-> TSelf[t]
                "ticks" => T<unit> ^-> !| T<float>
                "tickFormat" => !? T<int> * !? T<string> ^-> (T<float -> string>)
                "nice" => T<unit> ^-> TSelf[t]
                "copy" => T<unit> ^-> TSelf[t]
                "get" => t?d ^-> T<float>
                |> WithInline "$this($d)"
            ]

    let Time =
        Class "Time"
        |=> Implements [Base.Scale]
        |+> Instance [
            "ticks" => T<int> ^-> !| T<DateTime>
            "tickFormat" => !? T<int> * !? T<string> ^-> (T<DateTime -> string>)
            "nice" => T<unit> ^-> TSelf
            "get" => T<DateTime>?d ^-> T<float>
            |> WithInline "$this($d)"
        ]
