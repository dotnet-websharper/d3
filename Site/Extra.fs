namespace Site

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.D3

/// TODO: this should be part of the binding.
[<AutoOpen>]
module Extra =

    type UpdateSelection<'T> =

        [<Inline "$0.append($1)">]
        member u.Append(name: string) = u

        [<Inline "$0.attr($1, $2)">]
        member u.Attr(name: string, up: 'T -> string) = u

        [<Inline "$0.attr($1, $2)">]
        member u.Attr(name: string, up: 'T -> double) = u

        [<Inline "$0.attr($1, $2)">]
        member u.Attr(name: string, va: double) = u

        [<Inline "$0.enter()">]
        member u.Enter() = u

    type Selection with

        [<Inline "$0.data($1)">]
        member s.Data<'T>(data: 'T []) : UpdateSelection<'T> = X
