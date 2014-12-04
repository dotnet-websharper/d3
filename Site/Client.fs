namespace Site

open IntelliFactory.WebSharper

[<JavaScript>]
module Client =

    let All =
        let ( !+ ) x = Samples.Set.Singleton(x)
        Samples.Set.Create [
            !+ Circles.Sample
            !+ FocusBrushing.Sample
            !+ CompaniesGraph.UI.Sample
            !+ WorldTour.Sample
        ]

    let Main = All.Show()
