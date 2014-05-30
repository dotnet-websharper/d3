namespace Site

/// Support types for graph samples.
module Graphs =

    type Node<'T> =
        | Node of int * 'T

    type Link =
        | Link of source: int * target: int

    type DataSet<'T> =
        {
            Links: Link[]
            Nodes: Node<'T>[]
        }
