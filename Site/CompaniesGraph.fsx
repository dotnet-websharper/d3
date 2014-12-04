// Running this script connects to FreeBase using Google APIs and
// re-generates the data json file used by the CompaniesGraph sample.
//
// To run, obtain a FreeBase API key here (IP-dependent):
//
// https://code.google.com/apis/console/
//
// Choose the "Server" key and provide your IP address.
// Paste the key below and run the script.

#r "../packages/FSharp.Data.2.0.8/lib/net40/FSharp.Data.dll"
#r "../packages/WebSharper.3.0.1.73-alpha/tools/net40/IntelliFactory.WebSharper.Core.dll"
#r "../packages/WebSharper.3.0.1.73-alpha/tools/net40/IntelliFactory.WebSharper.dll"
#load "Graphs.fs"
#load "CompaniesGraph.Data.fs"

open System
open System.IO
open FSharp.Data
module D = Site.Graphs
module J = IntelliFactory.WebSharper.Core.Json
module FCC = Site.CompaniesGraph.Data

[<Literal>]
let FreebaseApiKey =
    "<<ENTER-KEY-HERE>>"
    
type FreebaseDataWithKey =
    FreebaseDataProvider<Key=FreebaseApiKey>

let Data =
    FreebaseDataWithKey.GetDataContext()

let LoadDataSet () : FCC.DataSet =
    let dataset =
        query {
            for c in Data.``Products and Services``.Business.``Consumer companies`` do
            for i in c.Industry do
            for r in c.Revenue do
            let cur = r.Currency
            where (cur.Name = "United States Dollar")
            select (c.Name, i.Name, r.Amount)
        }
        |> Seq.toArray
    let companies =
        dataset
        |> Seq.groupBy (fun (n, _, _) -> n)
        |> Seq.map (fun (n, rest) ->
            let revenue =
                rest
                |> Seq.choose (fun (_, _, r) -> if r.HasValue then Some r.Value else None)
                |> Seq.append [0.]
                |> Seq.max
            let industries = [| for (_, i, _) in rest -> i |]
            (n, revenue, industries))
        |> Seq.toArray
    let nodes =
        seq {
            for (name, revenue, industries) in companies do
                yield FCC.Company (name, revenue)
                for i in industries do
                    yield FCC.Industry i
        }
        |> Seq.distinct
        |> Seq.mapi (fun i n -> D.Node(i, n))
        |> Seq.toArray
    let companyByName =
        seq {
            for node in nodes do
                match node with
                | D.Node (_, FCC.Company (n, _)) -> yield (n, node)
                | _ -> ()
        }
        |> dict
    let industryByName =
        seq {
            for node in nodes do
                match node with
                | D.Node (_, FCC.Industry n) -> yield (n, node)
                | _ -> ()
        }
        |> dict
    let ( ~+ ) (D.Node (i, _)) = i
    let links =
        seq {
            for (name, revenue, industries) in companies do
                for i in industries do
                    yield D.Link (+companyByName.[name], +industryByName.[i])
        }
        |> Seq.toArray
    {
        Links = links
        Nodes = nodes
    }

let GenerateJson () =
    let data = LoadDataSet ()
    let p = J.Provider.Create()
    let enc = p.GetEncoder<FCC.DataSet>()
    let str =
        enc.Encode(data) |> p.Pack
        |> J.Stringify
    File.WriteAllText(Path.Combine(__SOURCE_DIRECTORY__, FCC.FileName), str)

GenerateJson ()
