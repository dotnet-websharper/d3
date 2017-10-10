namespace Site

open WebSharper

module Client =

    let All =
        let ( !+ ) x = Samples.Set.Singleton(x)
        Samples.Set.Create [
            !+ Circles.Sample
            !+ FocusBrushing.Sample
            !+ CompaniesGraph.UI.Sample
            !+ WorldTour.Sample
        ]

    [<SPAEntryPoint>]
    let Main() = All.Show()

[<assembly:JavaScript>]
do ()
