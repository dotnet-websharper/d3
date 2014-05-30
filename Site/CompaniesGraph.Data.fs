namespace Site.CompaniesGraph

open Site

/// Support types for CompaniesGraph sample.
[<ReflectedDefinition>]
module Data =

    type Label =
        | Company of name: string * revenue: double
        | Industry of name: string

    type Node = Graphs.Node<Label>
    type DataSet = Graphs.DataSet<Label>
    let FileName = "CompaniesGraph.json"

