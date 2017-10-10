namespace Site.CompaniesGraph

open Site
open WebSharper

/// Support types for CompaniesGraph sample.
module Data =

    type Label =
        | Company of name: string * revenue: double
        | Industry of name: string

    type Node = Graphs.Node<Label>
    type DataSet = Graphs.DataSet<Label>
    let FileName = "CompaniesGraph.json"

