[<ReflectedDefinition>]
module Site.Client

open IntelliFactory.WebSharper

let All =
    let ( !+ ) x = Samples.Set.Singleton(x)
    Samples.Set.Create [
        !+ Circles.Sample
        !+ FocusBrushing.Sample
    ]

let Main = All.Show()
