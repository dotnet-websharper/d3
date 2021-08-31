// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
namespace WebSharper.D3

open WebSharper
open WebSharper.InterfaceGenerator
open WebSharper.JavaScript.Dom

module Definition =

    let mutable classList = [] : CodeModel.NamespaceEntity list

    let addToClassList c =
        classList <- upcast c :: classList

    let ( |>! ) x f =
        f x
        x

    let EnumStrings name words = Pattern.EnumStrings name words |>! addToClassList

    let DefineRecord name propList =
        Class name
        |+> Instance (propList |> List.map (fun (n, t) -> upcast (n =@ t)))

    let Record name propList =
        DefineRecord name propList
        |>! addToClassList

    let O = T<unit>
    let String = T<string>
    let Int = T<int>
    let Float = T<float>
    let Obj = T<obj>
    let Bool = T<bool>
    let Error = T<exn>
    let ( !| ) x = Type.ArrayOf x

    let Point = !| Int //[x, y]

    let Int2T = Type.Tuple [Int; Int]
    let Int2x2T = Type.Tuple [Int2T; Int2T]
    let Float2T = Type.Tuple [Float; Float]
    let Float3T = Type.Tuple [Float; Float; Float]
    let Float2x2T = Type.Tuple [Float2T; Float2T]
    let Comparator = Obj * Obj ^-> Int

    let Date = T<JavaScript.Date>
    let DateTime = T<System.DateTime>

    let Element = T<Element>
    let NodeList = T<NodeList>
    let Event = T<Event>

    let FetchRequest = (T<string> * T<JavaScript.RequestOptions>) + T<JavaScript.Request>

    let nameArg = String?name

    let WithThis t ps r = (t -* ps ^-> r) + (ps ^-> r)

    let selectionCallback ret = WithThis Element (Obj?d * Int?i) ret

    let getVal t = O ^-> t
    //let setVal chained (t: obj) = chained t?value
    let setValF t = (t + selectionCallback t)?value
    let getSetValF chained t = getVal t + chained (setValF t)
    let getSetVal chained (t: Type.IType) = getVal t + chained t

    let getProp t = nameArg ^-> t
    let setProp t = nameArg * t?value
    let setPropF t = nameArg * (t + selectionCallback t)?value
    let getSetPropF chained t = getProp t + chained (setPropF t)
    let getSetProp chained (t: Type.Type) = getProp t + chained (setProp t)

    let NameValuePair =
        Record "NameValuePair" [
            "name"  , String
            "value" , String
        ]

    let Transition = Class "Transition"

    let selector = (String + Element + selectionCallback !|Element + selectionCallback NodeList)?selector

    let ChainedG (t: CodeModel.TypeParameter) (members : ((Type.IParameters -> Type.Type) -> CodeModel.IClassMember list)) (c: CodeModel.Class) =
        c |+> Instance (members <| fun args -> args ^-> c.[t])

    let Chained (members : ((Type.IParameters -> Type.Type) -> CodeModel.IClassMember list)) (c: CodeModel.Class) =
        c |+> Instance (members <| fun args -> args ^-> c)
        |>! addToClassList

    let ChainedClassG name (t: CodeModel.TypeParameter) (members : ((Type.IParameters -> Type.Type) -> CodeModel.IClassMember list)) =
        Class name |+> Instance (members <| fun args -> args ^-> TSelf.[t])

    let ChainedClass name (members : ((Type.IParameters -> Type.Type) -> CodeModel.IClassMember list)) =
        Class name |+> Instance (members <| fun args -> args ^-> TSelf)
        |>! addToClassList

    let ChainedClassNew name members =
        Class name |> Chained members

    let ChainedClassNewInherits name inherits members =
        Class name |=> Inherits inherits |> Chained members

    let ChainedClassInheritsG name (t: CodeModel.TypeParameter) inherits members =
        Class name |=> Inherits inherits |> ChainedG t members

//    let Selection = Type.New()

//    let UpdateSelectionT =
//        Generic / fun t ->
//            Class "UpdateSelection"
//            |=> Inherits Selection
////            |+> Instance
////                [
////                    "data" =>
////                        (
////                            let data = !|Obj + !|Float + !|Int + selectionCallback !|Obj
////                            let keyFunc = selectionCallback String
////                            data?values * !?keyFunc?key ^-> UpdateSelection
////                        )
////                    "data"       => O ^-> !|Obj
////                    "datum"      => chained (Obj + selectionCallback Obj)?value
////                    "filter"     => chained (String + selectionCallback Bool)?selector
////                    "sort"       => chained O
////                    "order"      => chained O
////                    "each"       => chained (selectionCallback O)
////                ]
//
//    addToClassList <| Generic - UpdateSelectionT

    let Selection =
        let selfG = Class "Selection"
        let attrValue = String + Float + Int + Obj
        Generic - fun (t: CodeModel.TypeParameter) ->
            let self = selfG.[t]
            let chained x = x ^-> self
            let data : CodeModel.IClassMember list = // .data(..)
                let elFunc x y = WithThis Element (x?datum * T<int>?index) y
                [
                    Generic - fun x -> "data" => (!|x)?data ^-> selfG.[x]
                    Generic -- fun x y -> "data" => (elFunc x !|y)?proj ^-> selfG.[x]
                    Generic -- fun x y -> "data" => (!|x)?data * (elFunc x y)?key ^-> selfG.[x]
                    "data" => O ^-> !|t
                ]
            selfG
            |+> Instance
                [
                    "attr" => String ^-> Obj
                    "attr" => String * Obj ^-> self
                    "attr" => String * (WithThis Element t attrValue) ^-> self
                    |> WithSourceName "AttrFn" // disambiguating for better completion
                    ("attr" => String * (WithThis Element (t * Int) attrValue) ^-> self)
                    |> WithSourceName "AttrIx" // same as above
                    "classed"    => getSetPropF chained Bool
                    "style"      => getProp Obj + chained (setPropF Obj * !?String?priority)
                    "property"   => getSetPropF chained Obj
                    "text"       => getSetValF chained String
                    "html"       => getSetValF chained String
                    "append"     => chained nameArg
                    "insert"     => chained (nameArg * !?(String + selectionCallback Element)?before)
                    "remove"     => chained O

                    "datum"      => chained (Obj + selectionCallback Obj)?value
                    "filter"     => chained (String + selectionCallback Bool)?selector
                    "sort"       => chained !?Comparator?comparator
                    "order"      => chained O
                    "each"       => chained (selectionCallback O)

                    "enter" => O ^-> self
                    "exit" => O ^-> self
                    "on"         => String?``type`` * !?(selectionCallback O)?listener ^-> self
                    "transition" => O + String ^-> Transition.[t]
                    "interrupt"  => chained O
                    "call"       => (self ^-> O) ^-> self
                    "call"       => T<JavaScript.Function> ^-> self
                    "empty"      => O ^-> Bool
                    "node"       => O ^-> Element
                    "size"       => O ^-> Int
                    "select"     => chained selector
                    "selectAll"  => chained selector
                ]
            |+> Instance data

    addToClassList Selection

    let tweenCallback   = (Element -* Obj?d * Int?i * Obj?a ^-> (Float?t ^-> Obj))
    let factoryCallback = (Element -* O ^-> (Element -* Float?t ^-> Obj))

    let easing = T<float -> float>

    let TransitionClass =
        Generic - fun (t: CodeModel.TypeParameter) ->
            Transition |> Chained (fun _ ->
                let chained x = x ^-> TSelf.[t]
                [
                    "delay"      => chained Int?delay
                    "duration"   => chained Int?duration
                    "ease"       => chained (!+Float * String?value) + chained easing?value
                    "easeVarying" => chained (nameArg * factoryCallback?factory)
                    "attr"       => chained (nameArg * (Obj + selectionCallback Obj)?value)
                    "attrTween"  => chained (nameArg * tweenCallback?value)
                    "style"      => chained (nameArg * (Obj + selectionCallback Obj)?value * !? String?priority)
                    "styleTween" => chained (nameArg * tweenCallback?value * !? String?priority)
                    "text"       => chained (setValF String)
                    "textTween"      => chained (nameArg * factoryCallback?factory)
                    "tween"      => chained (nameArg * (O ^-> (Float?t ^-> O)))
                    "remove"     => chained O
                    "select"     => chained selector
                    "selectAll"  => chained selector
                    "selectChild" => chained selector
                    "selectChildren" => chained selector
                    "filter"     => chained (String + selectionCallback Bool)?selector
                    "merge" => chained selector
                    "transition" => chained O
                    "selection" => O ^-> Selection.[t]
                    "active" => chained selector
                    "end" => chained O
                    "on" => (Transition.[t]?``type`` ^-> (O ^-> O)) + chained (Transition.[t]?``type`` * (O ^-> O)?listener)
                    "each"       => chained (!?String?``type`` * selectionCallback O)
                    "call"       => chained (!+Obj * (Selection.[Obj] ^-> O)?callback)
                    "empty"      => O ^-> Bool
                    "nodes" => O ^-> !| Element
                    "node"       => O ^-> Element
                    "size"       => O ^-> Int
                ])

    let KeyValuePair =
        Record "KeyValuePair" [
            "key"   , String
            "value" , Obj
        ]

    let Map =
        Class "Map"
        |+> Instance [
            "has"     => String?key ^-> Bool
            "get"     => String?key ^-> Float
            "set"     => String?key * Obj?value ^-> O
            "remove"  => String?key ^-> Bool
            "keys"    => O ^-> !|String
            "values"  => O ^-> !|Obj
            "entries" => O ^-> !|KeyValuePair
            "forEach" => WithThis TSelf (String?key * Obj?value) O
        ]
        |>! addToClassList

    let Set =
        Class "Set"
        |+> Instance [
            "has"     => String?value ^-> Bool
            "add"     => String?value ^-> String
            "remove"  => String?value ^-> Bool
            "values"  => O ^-> !|String
            "forEach" => WithThis TSelf String?value O
        ]
        |>! addToClassList

    let Iterable = !| Int + !| Float + !| String + !| Obj + Map + Set
    let Value = Int + Float + String + Obj

    let Nest =
        ChainedClassNew "Nest" <| fun chained ->
        [
            "key"        => chained (Obj ^-> String)?``function``
            "sortKeys"   => chained Comparator?comparator
            "sortValues" => chained Comparator?comparator
            "rollup"     => chained (!|Obj ^-> Obj)?``function``
            "map"        => (!|Obj)?array ^-> !|Obj
            Generic - fun t -> "map" => (!|Obj)?array * (!?Obj ^-> t)?mapType ^-> t
            "entries"    => (!|Obj)?array ^-> !|KeyValuePair
        ]

    let TimeFormat =
        Class "TimeFormat"
        |+> Instance [
            "apply" => Date?date ^-> String
            "parse" => String?string ^-> Date
        ]
        |>! addToClassList

    let Transform =
        Class "Transform"
        |+> Instance [
            "rotate"    =@ Float
            "translate" =@ Int2T
            "skew"      =@ Float
            "scale"     =@ Type.Tuple [Float; Float]
            "toString"  => O ^-> String
            "scale" => Int ^-> TSelf
            "translate" => Int * Int ^-> TSelf
            "apply" => !| Int ^-> TSelf
            "applyX" => Int ^-> TSelf
            "applyY" => Int ^-> TSelf
            "invert" => !| Int ^-> TSelf
            "invertX" => Int ^-> TSelf
            "invertY" => Int ^-> TSelf
            "rescaleX" => !| Int ^-> !| Int
            "rescaleY" => !| Int ^-> !| Int
        ]
        |>! addToClassList

    let Xhr =
        ChainedClassNew "Xhr" <| fun chained ->
        [
            "header"       => getSetProp chained String
            "mimeType"     => getSetVal chained String
            "responseType" => getSetVal chained String
            "response"     => getSetVal chained (String?request ^-> Obj)
            "get"          => !?String?data * !?(Error?error * Obj?response ^-> O)?callback ^-> O
            "send"         => String?``method`` * !?String?data * !?(Error?error * Obj?response ^-> O)?callback ^-> O
            "abort"        => O ^-> O
            "on"           => String?``type`` ^-> (Error?error * Obj?response ^-> O)
            "on"           => String?``type`` * (Error?error * Obj?response ^-> O)?listener ^-> O
        ]

    let Rgb = Class "rgb"

    let ColorType name convName convType =
        let chained args = args ^-> Rgb
        (if name = "rgb" then Rgb else Class name)
        |+> Instance (
            name |> Seq.map (fun n ->
                string n =@ Int :> CodeModel.IClassMember
            )
            |> List.ofSeq
        )
        |+> Instance [
            "brighter" => chained !?Float?k
            "darker"   => chained !?Float?k
            convName   => O ^-> convType
            "toString" => O ^-> String
        ]
        |>! addToClassList

    let Hsl = ColorType "hsl" "rgb" Rgb
    let Lab = ColorType "lab" "rgb" Rgb
    let Hcl = ColorType "hcl" "rgb" Rgb
    let RgbClass = ColorType "rgb" "hsl" Hsl

    let QualifiedNs =
        Record "QualifiedNs" [
            "space" , String
            "local" , String
        ]

    let ContinuousScale =
        ChainedClassNew "ContinuousScale" <| fun chained ->
            [
                "invert" => Int ^-> Int
                "domain" => chained !| Int
                "range" => chained !| Int
                "rangeRound" => chained !| Int
                "clamp" => chained Int
                "unknown" => chained !| Int
                "interpolate" => chained !| Int
                "count" => chained Int
                "tickFormat" => (!? Int ^-> !? String) ^-> String
                "nice" => chained Int
                "copy" => chained O
            ]

    let PowScale =
        ChainedClassNew "PowScale" <| fun chained ->
            [
                "invert" => Int ^-> Int
                "domain" => chained !| Int
                "range" => chained !| Int
                "rangeRound" => chained !| Int
                "clamp" => chained Int
                "unknown" => chained !| Int
                "interpolate" => chained !| Int
                "count" => chained Int
                "tickFormat" => (!? Int ^-> !? String) ^-> String
                "nice" => chained Int
                "copy" => chained O
            ]

    let LogScale =
        ChainedClassNew "LogScale" <| fun chained ->
            [
                "invert" => Int ^-> Int
                "domain" => chained !| Int
                "range" => chained !| Int
                "rangeRound" => chained !| Int
                "clamp" => chained Int
                "unknown" => chained !| Int
                "interpolate" => chained !| Int
                "count" => chained Int
                "tickFormat" => (!? Int ^-> !? String) ^-> String
                "nice" => chained Int
                "copy" => chained O
            ]

    let ContTimeScale =
        ChainedClassNew "ContTimeScale" <| fun chained ->
            [
                "invert" => Int ^-> Int
                "domain" => chained !| Int
                "range" => chained !| Int
                "rangeRound" => chained !| Int
                "clamp" => chained Int
                "unknown" => chained !| Int
                "interpolate" => chained !| Int
                "count" => chained Int
                "tickFormat" => (!? Int ^-> !? String) ^-> String
                "nice" => chained Int
                "copy" => chained O
            ]

    let ContScale =
        Class "ContScale"
        |+> Instance [
            "continuous" =? ContinuousScale
            "pow" =? PowScale
            "log" =? LogScale
            "time" =? ContTimeScale
        ]
        |>! addToClassList

    let interpolate argType = argType?a * argType?b ^-> argType

    let Scale = Class "Scale" |>! addToClassList

    let getQuantScale name (domainType: Type.Type) =
        ChainedClassNewInherits name Scale <| fun chained ->
        [
            "apply"       => domainType?x ^-> Float |> WithInline "$this($x)"
            "invert"      => Float?y ^-> domainType
            "domain"      => getSetVal chained !|domainType
            "range"       => getSetVal chained !|Float
            "rangeRound"  => chained !|Float
            "interpolate" => getSetVal chained (interpolate Float)
            "clamp"       => getSetVal chained Bool
            "nice"        => chained !?Int?count
            "ticks"       => !?Int?count ^-> !|Float
            "tickFormat"  => Int?count * !?String?format ^-> Float?number ^-> String
            "copy"        => chained O
        ]

    let QuantitativeScale = getQuantScale "QuantitativeScale" Float

    let DiscreteScale =
        ChainedClassNewInherits "DiscreteScale" Scale <| fun chained ->
        [
            "apply"        => Float?x ^-> Float |> WithInline "$this($x)"
            "invertExtent" => Float?y ^-> Float2T
            "domain"       => getSetVal chained !|Float
            "range"        => getSetVal chained !|Float
            "copy"         => chained O
        ]

    let QuantileScale =
        ChainedClassNewInherits "QuantileScale" DiscreteScale <| fun chained ->
        [
            "quantiles" => O ^-> !|Float
        ]

    let IdentityScale =
        ChainedClassNewInherits "IdentityScale" Scale <| fun chained ->
        [
            "apply"       => Float?x ^-> Float |> WithInline "$this($x)"
            "invert"      => Float?y ^-> Float
            "domain"      => getSetVal chained !|Float
            "range"       => getSetVal chained !|Float
            "ticks"       => !?Int?count ^-> !|Float
            "tickFormat"  => Int?count * !?String?format ^-> Float?number ^-> String
        ]

    let OrdinalScale =
        let gen =
            Generic - fun tData ->
            ChainedClassInheritsG "OrdinalScale" tData Scale <| fun chained ->
            [
                "apply"           => tData?x ^-> Float |> WithInline "$this($x)"
                "domain"          => getSetVal chained !|tData
                "range"           => getSetVal chained !|Float
                "rangePoints"     => chained (Float2T?interval * !?Float?padding)
                "rangeBands"      => chained (Float2T?interval * !?Float?padding * !?Float?outerPadding)
                "rangeRoundBands" => chained (Float2T?interval * !?Float?padding * !?Float?outerPadding)
                "rangeBand"       => O ^-> Float
                "rangeExtent"     => O ^-> Float2T
                "copy"            => chained O
            ]
        addToClassList gen
        gen


    let TimeScale = getQuantScale "TimeScale" Date

    let Interpolation =
        EnumStrings "Interpolation" [
            "linear"
            "step"
            "step-before"
            "step-after"
            "basis"
            "basis-open"
            "cardinal"
            "cardinal-open"
            "monotone"
        ]

    let Node =
        ChainedClassNew "Node" <| fun chained ->
        [
            "data" =? Obj
            "depth" =? Int
            "height" =? Int
            "parent" =? TSelf
            "children" =? !| TSelf
            "value" =? Int + Float

            "ancestors" => O ^-> !| TSelf
            "descendants" => O ^-> !| TSelf
            "leafes" => O ^-> !| TSelf
            "filter" => (TSelf ^-> Bool) ^-> !| TSelf
            "path" => TSelf ^-> !| TSelf
            "links" => O ^-> !| Obj
            "sum" => (TSelf ^-> Int + Float) ^-> TSelf
            "count" => O ^-> Int
            "sort" => (TSelf * TSelf ^-> Int + Float) ^-> TSelf
            "each" => (TSelf * Int * TSelf) ^-> !| TSelf
            "eachBefore" => (TSelf * Int * TSelf) ^-> !| TSelf
            "eachAfter" => (TSelf * Int * TSelf) ^-> !| TSelf
            "copy" => O ^-> TSelf

        ]

    let StratifyCluster =
        ChainedClassNew "StratifyCluster" <| fun chained ->
        [
            "size" => getSetVal chained Int
            "nodeSize" => getSetVal chained Int
            "separation" => getSetVal chained (Obj * Obj ^-> Int)
        ]

    let Stratify =
        ChainedClassNew "Stratify" <| fun chained ->
        [
            "id" => getSetVal chained Obj
            "parentID" => getSetVal chained Obj
            "cluster" => StratifyCluster
        ] 

    let ChainedClassCoord name members =
        let gen =
            Generic - fun tData ->
            ChainedClassG name tData <| fun chained ->
            let coord name = name => getSetVal chained (tData ^-> Float) + chained Float
            members chained coord tData
        addToClassList gen
        gen

    let Line =
        ChainedClassCoord "Line" <| fun chained coord tData ->
        [
            coord "x"
            coord "y"
            "interpolate" => getSetVal chained Interpolation
            "tension" => getSetVal chained String
            "defined" => getSetVal chained (tData ^-> Bool)
            "x" => getSetVal chained Int
            "y" => getSetVal chained Int
            "curve" => getSetVal chained (!| (Int * Int) * !| (Int * Int))
            "context" => getSetVal chained Obj
        ]

    let RadialLine =
        ChainedClassCoord "RadialLine" <| fun chained coord tData ->
        [
            coord "radius"
            coord "angle"
            "defined" => getSetVal chained (tData ^-> Bool)
            "angle" => getSetVal chained Int
            "radius" => getSetVal chained Int
            "curve" => getSetVal chained (!| (Int * Int) * !| (Int * Int))
            "context" => getSetVal chained Obj
        ]

    let Area =
        ChainedClassCoord "Area" <| fun chained coord tData ->
        [
            coord "x"
            coord "x0"
            coord "x1"
            coord "y"
            coord "y0"
            coord "y1"
            "interpolate" => getSetVal chained Interpolation
            "tension" => getSetVal chained String
            "defined" => getSetVal chained (tData ^-> Bool)
            "curve" => getSetVal chained (!| (Int * Int) * !| (Int * Int))
            "context" => getSetVal chained Obj

            "lineX0" => Line
            "lineY0" => Line
            "lineX1" => Line
            "lineY1" => Line
        ]

    let RadialArea =
        ChainedClassCoord "RadialArea" <| fun chained coord tData ->
        [
            coord "radius"
            coord "innerRadius"
            coord "outerRadius"
            coord "angle"
            coord "startAngle"
            coord "endAngle"
            "defined" => getSetVal chained (tData ^-> Bool)
            "curve" => getSetVal chained (!| (Int * Int) * !| (Int * Int))
            "context" => getSetVal chained Obj

            "lineX0" => Line
            "lineY0" => Line
            "lineX1" => Line
            "lineY1" => Line        ]

    let Arc =
        ChainedClassCoord "Arc" <| fun chained coord tData ->
        [
            coord "innerRadius"
            coord "outerRadius"
            coord "startAngle"
            coord "endAngle"
            "centroid" => Obj?datum * !?Int?index ^-> Float2T
            "innerRadius" => getSetVal chained Int
            "outerRadius" => getSetVal chained Int
            "cornerRadius" => getSetVal chained Int
            "startAngle" => getSetVal chained Int
            "endAngle" => getSetVal chained Int
            "padAngle" => getSetVal chained Int
            "padRadius" => getSetVal chained Int
            "context" => getSetVal chained Obj
        ]

    let Curve =
        ChainedClassNew "Curve" <| fun chained ->
            [
                "areaStart" => getSetVal chained O
                "areaEnd" => getSetVal chained O
                "lineStart" => getSetVal chained O
                "lineEnd" => getSetVal chained O
                "point" => Int * Int ^-> O
            ]

    let SymbolType =
        EnumStrings "SymbolType" [
            "circle"
            "cross"
            "diamond"
            "square"
            "triange-down"
            "triange-up"
        ]

    let Symbol =
        ChainedClassNew "Symbol" <| fun chained ->
        [
            "type" => getSetVal chained SymbolType.Type
            "size" => getSetVal chained Int
            "context" => getSetVal chained Obj
        ]

    let Chord =
        ChainedClassCoord "Chord" <| fun chained coord tData ->
        [
            coord "radius"
            coord "innerRadius"
            coord "outerRadius"
            coord "angle"
            "source" => getSetVal chained (tData?d * Int?i ^-> Obj) + chained Obj
            "target" => getSetVal chained (tData?d * Int?i ^-> Obj) + chained Obj

            
        ]

    let Diagonal =
        ChainedClassNew "Diagonal" <| fun chained ->
        [
            "apply"  => Obj?datum * !?Int?index ^-> String
            "source" => getSetVal chained (Obj?d * Int?i ^-> Obj) + chained Obj
            "target" => getSetVal chained (Obj?d * Int?i ^-> Obj) + chained Obj
            "projection" => getSetVal chained (Float2T ^-> Float2T)
        ]

    let Orientation =
        EnumStrings "Orientation" [
            "top"
            "bottom"
            "left"
            "right"
        ]

    let Axis =
        ChainedClassNew "Axis" <| fun chained ->
        [
            "apply"  => (Selection.[Obj] + Transition.[Obj])?selection ^-> O |> WithInline "$this($selection)"
            "scale"  => getSetVal chained Scale.Type
            "orient" => getSetVal chained Orientation.Type
            "ticks"  => chained !+Obj
            "tickValues" => getSetVal chained !|Obj
            "tickSize"  => getSetVal chained Int
            "tickSizeInner"  => getSetVal chained Int
            "tickSizeOuter"  => getSetVal chained Int
            "tickPadding" => getSetVal chained Int
            "tickFormat" => getSetVal chained (Obj ^-> String)
            "tickArguments" => getSetVal chained !| Obj
            "offset" => getSetVal chained (Float + Int)
        ]

    let BrushEvent =
        EnumStrings "BrushEvent" [
            "start"
            "brush"
            "end"
        ]

    let Brush =
        ChainedClassNew "Brush" <| fun chained ->
        [
            "apply" => (Selection.[Obj] + Transition.[Obj])?selection ^-> O |> WithInline "$this($selection)"
            "x" => getSetVal chained Scale.Type
            "y" => getSetVal chained Scale.Type
            "extent" => getSetVal chained (Int2T + Int2x2T)
            "clear" => chained O
            "empty" => O ^-> Bool
            "on"    => (BrushEvent?``type`` ^-> (O ^-> O)) + chained (BrushEvent?``type`` * (O ^-> O)?listener)
            "event" => (Selection.[Obj] + Transition.[Obj])?selection ^-> O
            "filter" => getSetVal chained (BrushEvent ^-> Bool)
            "touchable" => getSetVal chained (O ^-> Bool)
            "keyModifiers" => getSetVal chained Bool
            "handleSize" => getSetVal chained Int
        ]

    let Timer =
        ChainedClassNew "Timer" <| fun chained ->
        [
            "restart" => (Int ^-> O) * Int ^-> O
            "stop" => chained O
        ]

    let TimeInterval =
        ChainedClassNew "TimeInterval" <| fun chained ->
        [
            "floor" => chained Date
            "round" => chained Date
            "ceil" => chained Date
            "offset" => Date * !? Int ^-> Date
            "range" => Date * Date * !? Int ^-> !| Date
            
            "timeMilliseconds" => Date * Date * !? Int ^-> !| Date
            "utcMilliseconds" => Date * Date * !? Int ^-> !| Date
            "timeSeconds" => Date * Date * !? Int ^-> !| Date
            "utcSeconds" => Date * Date * !? Int ^-> !| Date
            "timeMinutes" => Date * Date * !? Int ^-> !| Date
            "utcMinutes" => Date * Date * !? Int ^-> !| Date
            "timeHours" => Date * Date * !? Int ^-> !| Date
            "utcHours" => Date * Date * !? Int ^-> !| Date
            "timeDays" => Date * Date * !? Int ^-> !| Date
            "utcDays" => Date * Date * !? Int ^-> !| Date
            "timeWeeks" => Date * Date * !? Int ^-> !| Date
            "utcWeeks" => Date * Date * !? Int ^-> !| Date
            "timeMonths" => Date * Date * !? Int ^-> !| Date
            "utcMonths" => Date * Date * !? Int ^-> !| Date
            "timeYears" => Date * Date * !? Int ^-> !| Date
            "utcYears" => Date * Date * !? Int ^-> !| Date
            
            "timeMondays" => Date * Date * !? Int ^-> !| Date
            "utcMondays" => Date * Date * !? Int ^-> !| Date
            "timeTuesdays" => Date * Date * !? Int ^-> !| Date
            "utcTuesdays" => Date * Date * !? Int ^-> !| Date
            "timeWednesdays" => Date * Date * !? Int ^-> !| Date
            "utcWednesdays" => Date * Date * !? Int ^-> !| Date
            "timeThursdays" => Date * Date * !? Int ^-> !| Date
            "utcThursdays" => Date * Date * !? Int ^-> !| Date
            "timeFridays" => Date * Date * !? Int ^-> !| Date
            "utcFridays" => Date * Date * !? Int ^-> !| Date
            "timeSaturdays" => Date * Date * !? Int ^-> !| Date
            "utcSaturdays" => Date * Date * !? Int ^-> !| Date
            "timeSundays" => Date * Date * !? Int ^-> !| Date
            "utcSundays" => Date * Date * !? Int ^-> !| Date

            "filter" => (Date ^-> Bool) ^-> TSelf
            "every" => Int ^-> TSelf
            "count" => Date * Date ^-> Int
        ]

    let WithDefaultConstructor (x: CodeModel.Class) =
        x |+> Static [
            Constructor O
            |> WithInline "({})"
        ]

    let Link =
        Generic - fun t ->
            Class "Link"
            |> WithDefaultConstructor
            |+> Instance [
                "source" =@ t
                "target" =@ t
                "x" =@ t
                "y" =@ t
                "context" =@ t
            ]

    addToClassList Link



    let BundleNode =
        Record "BundleNode" [
            "parent" , TSelf
        ]

    let Bundle =
        ChainedClassNew "Bundle" <| fun chained ->
        [
            "links"      => (!|BundleNode)?nodes ^-> !|Link.[BundleNode]
        ]

    let ChordNode =
        Record "ChordObject" [
            "index"      , Int
            "subIndex"   , Int
            "startAngle" , Float
            "endAngle"   , Float
            "value"      , Float
        ]

    let ChordLayout =
        ChainedClassNew "ChordLayout" <| fun chained ->
        [
            "matrix"  => getSetVal chained (!| !|Float)
            "sortGroups"    => getSetVal chained Comparator
            "sortSubgroups" => getSetVal chained Comparator
            "sortChords"    => getSetVal chained Comparator
            "padAngle" => getSetVal chained Float
        ]

    let Ribbon =
        ChainedClassNew "Ribbon" <| fun chained ->
        [
            "source" => getSetVal chained Obj
            "target" => getSetVal chained Obj
            "radius" => getSetVal chained Int
            "sourceRadius" => getSetVal chained Int
            "targetRadius" => getSetVal chained Int
            "startAngle" => getSetVal chained Float
            "endAngle" => getSetVal chained Float
            "padAngle" => getSetVal chained Float
            "context" => getSetVal chained Obj
        ]

    let RibbonArrow =
        ChainedClassNew "RibbonArrow" <| fun chained ->
        [
            "headRadius" => getSetVal chained Int
        ]

    let ClusterNode =
        Record "ClusterNode" [
            "parent"   , TSelf
            "children" , !|TSelf
            "depth"    , Int
            "x"        , Int
            "y"        , Int
        ]

    /// Pseudo-property getting and setting a 2-element double array [x,y].
    let propF2 self name : list<CodeModel.IClassMember> =
        let getter =
            (name => Float * Float ^-> self)
            |> WithInline (sprintf "$0.%s([$1,$2])" name)
        let setter = name => O ^-> Float * Float
        [getter; setter]

    /// Pseudo-property getting and setting a 3-element triple array [x,y,z].
    let propF3 self name : list<CodeModel.IClassMember> =
        let getter =
            (name => Float * Float * Float ^-> self)
            |> WithInline (sprintf "$0.%s([$1,$2,$3])" name)
        let setter = name => O ^-> Float * Float * Float
        [getter; setter]

    /// Pseudo-property getting and setting something of the form [[x,y],[a,b]] with all floats.
    let propF4 self name : list<CodeModel.IClassMember> =
        let getter =
            (name => Float2T * Float2T ^-> self)
            |> WithInline (sprintf "$0.%s([$1,$2])" name)
        let setter = name => O ^-> Float2T * Float2T
        [getter; setter]

    let Cluster =
        ChainedClassNew "Cluster" <| fun chained ->
            [
                "nodes"      => ClusterNode?root ^-> !|ClusterNode
                "links"      => (!|ClusterNode)?nodes ^-> !|Link.[ClusterNode]
                "children"   => getSetVal chained (Obj ^-> Obj)
                "sort"       => getSetVal chained Comparator
                "separation" => getSetVal chained (ClusterNode * ClusterNode ^-> Int)
                "value"      => getSetVal chained (Obj ^-> Float)
            ]
            @ propF2 TSelf "nodeSize"
            @ propF2 TSelf "size"

    let ForceNode =
        DefineRecord "ForceNode" [
            "index"  , Int
            "x"      , Int
            "y"      , Int
            "px"     , Int
            "py"     , Int
            "fixed"  , Bool
            "weight" , Int
        ]
        |> WithDefaultConstructor
        |>! addToClassList

    let ForceEvent =
        EnumStrings "ForceEvent" [
            "start"
            "tick"
            "end"
        ]

    let Force =
        ChainedClassNew "Force" <| fun chained ->
            [
                "on" => (ForceEvent?``type`` ^-> (O ^-> O)) + chained (ForceEvent?``type`` * (O ^-> O)?listener)
                "nodes" => getSetVal chained !|ForceNode
                "links" => getSetVal chained !|Link.[ForceNode]
                "start" => chained O
                "alpha" => getSetVal chained Float
                "resume" => O ^-> O
                "tick" => O ^-> O
                "drag" =? T<JavaScript.Function>
                "linkDistance" => chained !?(Int + (Obj?d * Int?i ^-> Int))?distance
                "charge" => chained !?(Int + (Obj?d * Int?i ^-> Int))?charge
            ]
            @ propF2 TSelf "size"

    let HierarchyNode =
        Record "HierarchyNode" [
            "parent"   , TSelf
            "children" , !|TSelf
            "value"    , Obj
            "depth"    , Int
        ]

    let Hierarchy =
        ChainedClassNew "Hierarchy" <| fun chained ->
        [
            "nodes"    => HierarchyNode?root ^-> !|HierarchyNode
            "links"    => getSetVal chained !|Link.[HierarchyNode]
            "children" => getSetVal chained (Obj ^-> Obj)
            "sort"     => getSetVal chained Comparator
            "value"    => getSetVal chained (Obj ^-> Float)
            "revalue"  => chained HierarchyNode?root
        ]

    let binningFunc = Float2T?range * !|Obj * Int ^-> !|Float

    let Histogram =
        ChainedClassNew "Histogram" <| fun chained ->
        [
            "value"      => getSetVal chained (Obj ^-> Float)
            "range"      => getVal Float2T + chained (Float2T + !|Obj * Int ^-> Float2T)
            "bins"       => getVal binningFunc + chained(Int + !|Float + binningFunc)
            "frequency"  => getSetVal chained Bool
        ]

    let PackNode =
        Record "PackNode" [
            "parent"   , TSelf
            "children" , !|TSelf
            "value"    , Obj
            "depth"    , Int
            "x"        , Int
            "y"        , Int
            "r"        , Int
        ]

    let Pack =
        ChainedClassNew "Pack" <| fun chained ->
            [
                "nodes"    => PackNode?root ^-> !|PackNode
                "links"    => getSetVal chained !|Link.[PackNode]
                "children" => getSetVal chained (Obj ^-> Obj)
                "sort"     => getSetVal chained Comparator
                "value"    => getSetVal chained (Obj ^-> Float)
                "radius"   => getSetVal chained Int
                "padding"  => getSetVal chained Int
            ]
            @ propF2 TSelf "size"

    let PartitionNode =
        Record "PartitionNode" [
            "parent"   , TSelf
            "children" , !|TSelf
            "value"    , Obj
            "depth"    , Int
            "x"        , Int
            "y"        , Int
            "dx"       , Int
            "dy"       , Int
        ]

    let Partition =
        ChainedClassNew "Partition" <| fun chained ->
            [
                "nodes"    => PartitionNode?root ^-> !|PartitionNode
                "links"    => getSetVal chained !|Link.[PartitionNode]
                "children" => getSetVal chained (Obj ^-> Obj)
                "sort"     => getSetVal chained Comparator
                "value"    => getSetVal chained (Obj ^-> Float)
            ]
            @ propF2 TSelf "size"

    let Pie =
        ChainedClassNew "Pie" <| fun chained ->
        [
            "apply"      => (!|Obj)?values * !?Int?index ^-> !|Obj |> WithInline "$this($values, $index)"
            "value"    => getSetVal chained (Obj ^-> Float)
            "sort"     => getSetVal chained Comparator
            "sortValues" => getSetVal chained Comparator
            "startAngle" => getVal Float + chained (Float + Obj * Int ^-> Float)
            "endAngle" => getVal Float + chained (Float + Obj * Int ^-> Float)
            "padAngle" => getSetVal chained (O ^-> Int)

        ]

    let StackOffset =
        EnumStrings "StackOffset" [
            "silhouette"
            "wiggle"
            "expand"
            "zero"
        ]

    let StackOrder =
        EnumStrings "StackOrder" [
            "inside-out"
            "reverse"
            "default"
        ]

    let Stack =
        ChainedClassNew "Stack" <| fun chained ->
        [
            "apply"      => (!|Obj)?layers * !?Int?index ^-> !|Obj |> WithInline "$this($layers, $index)"
            "values" => getSetVal chained (Obj ^-> Obj)
            "offset" => getVal (!|Float2T ^-> !|Float) + chained (StackOffset + (Float2T ^-> !|Float))
            "order"  => getVal (!|Float2T ^-> !|Int) + chained (StackOrder + (!|Float2T ^-> !|Int))
            "x"      => getSetVal chained (Obj ^-> Float)
            "y"      => getSetVal chained (Obj ^-> Float)
            "out"    => getSetVal chained (Obj?d * Float?y0 * Float?y ^-> O)
            "keys" => getSetVal chained (!| Obj ^-> !| Obj)
        ]

    let TreeNode =
        Record "TreeNode" [
            "parent"   , TSelf
            "children" , !|TSelf
            "depth"    , Int
            "x"        , Int
            "y"        , Int
        ]

    let Tree =
        ChainedClassNew "Tree" <| fun chained ->
            [
                "nodes"      => TreeNode?root ^-> !|TreeNode
                "links"      => (!|TreeNode)?nodes ^-> !|Link.[TreeNode]
                "children"   => getSetVal chained (Obj ^-> Obj)
                "sort"       => getSetVal chained Comparator
                "separation" => getSetVal chained (TreeNode * TreeNode ^-> Int)
            ]
            @ propF2 TSelf "nodeSize"
            @ propF2 TSelf "size"

    let TreemapMode =
        EnumStrings "TreemapMode" [
            "squarify"
            "slice"
            "dice"
            "slice-dice"
        ]

    let Treemap =
        ChainedClassNew "Treemap" <| fun chained ->
            [
                "nodes"     => PartitionNode?root ^-> !|PartitionNode
                "links"     => (!|PartitionNode)?nodes ^-> !|Link.[PartitionNode]
                "children"  => getSetVal chained (Obj ^-> Obj)
                "sort"      => getSetVal chained Comparator
                "value"     => getSetVal chained (Obj ^-> Float)
                "padding"   => getSetVal chained Int
                "round"     => getSetVal chained Bool
                "sticky"    => getSetVal chained Bool
                "mode"      => getSetVal chained TreemapMode
            ]
            @ propF2 TSelf "size"

    let PathContext =
        Interface "PathContext"
        |+> [
            "beginPath" => O ^-> O
            "moveTo"    => Float?x * Float?y ^-> O
            "lineTo"    => Float?x * Float?y ^-> O
            "quadraticCurveTo" => Float?cpx * Float?cpy * Float?x * Float?y ^-> O
            "arcTo" => Float?x1 * Float?x2 * Float?y1 * Float?y2 * Int?radius ^-> O
            "bezierCurveTo" => Float?cpx1 * Float?cpy1 * Float?cpx2 * Float?cpy2 * Float?x * Float?y ^-> O
            "arc"       => Float?x * Float?y * Float?radius * Float?startAngle * Float?endAngle ^-> O
            "closePath" => O ^-> O
            "rect" => Int * Int * Int *Int ^-> TSelf
            "toString" => O ^-> String
        ]
        |>! addToClassList

    let Feature =
        ChainedClassNew "Feature" <| fun chained -> []

    let MultiLineString =
        ChainedClassNew "MultiLineString" <| fun chained -> []

    let LineString =
        ChainedClassNew "LineString" <| fun chained -> []

    let Circle =
        ChainedClassNew "Circle" <| fun chained ->
            [
                "origin"    => (Float2T + Obj ^-> Float2T) ^-> TSelf
                "angle"     => getSetVal chained Float
                "precision" => getSetVal chained Float
            ]
            @ propF2 TSelf "origin"

    let Rotation =
        ChainedClassNew "Rotation" <| fun chained ->
        [
            "apply" => Float2T?location ^-> Float2T |> WithInline "$this($location)"
            "invert" => Float2T?location ^-> Float2T
        ]

    let Listener =
        Interface "Listener"
        |+> [
            "point"        => Float?x * Float?y * !?Float?z ^-> O
            "lineStart"    => O ^-> O
            "lineEnd"      => O ^-> O
            "polygonStart" => O ^-> O
            "polygonEnd"   => O ^-> O
            "sphere"       => O ^-> O
        ]
        |>! addToClassList

    let Projection =
        ChainedClassNew "Projection" <| fun chained ->
            [
                "apply"      => Float2T?location ^-> Float2T |> WithInline "$this($location)"
                "invert"     => Float2T?location ^-> Float2T
                "scale"      => getSetVal chained Float
                "clipAngle"  => getSetVal chained Float
                "clipExtent" => getSetVal chained Float2x2T
                "precision"  => getSetVal chained Int
                "stream"     => Listener?listener ^-> Listener
                "reflectX" => getSetVal chained Bool
                "reflectY" => getSetVal chained Bool
                "fitExtend" => !| (Int * Int) * Obj ^-> TSelf
                "fitSize" => Int * Obj ^-> TSelf
                "fitHeight" => Int * Obj ^-> TSelf
                "fitWidth" => Int * Obj ^-> TSelf
            ]
            @ propF2 TSelf "center"
            @ propF2 TSelf "translate"
            @ propF3 TSelf "rotate"
            @ propF3 TSelf "angle"

    let Path =
        ChainedClassNew "Path" <| fun chained ->
        [
            "call" => Obj?feature ^-> String
            |> WithInline "$0($1)"
            "call" => Obj?feature * Int?ix ^-> String
            |> WithInline "$0($1,$2)"
            "projection"  => getSetVal chained ((Float2T ^-> Float2T) + Projection)
            "context"     => getSetVal chained (PathContext.Type + Obj)
            "pointRadius" => getSetVal chained Float
            "area"        => Feature ^-> Int
            "centroid"    => Feature ^-> Int2T
            "bounds"      => Feature ^-> Int2x2T
        ]


    let AlbersProjection =
        Class "AlbersProjection"
        |=> Inherits Projection
        |+> Instance [ yield! propF2 TSelf "parallels" ]
        |>! addToClassList

    let StreamTransform =
        ChainedClassNew "StreamTransform" <| fun chained ->
        [
            "stream" => Listener?listener ^-> Listener
        ]

    let ClipTransform =
        ChainedClassNewInherits "ClipTransform" StreamTransform <| fun chained ->
        [
            "extent" => getSetVal chained Float2x2T
        ]

    let Voronoi =
        ChainedClassNew "Voronoi" <| fun chained ->
        [
            "x" => getSetVal chained (Obj ^-> Float)
            "y" => getSetVal chained (Obj ^-> Float)
            "clipExtent" => getSetVal chained Float2x2T
            "links" => !|Obj ^-> !|Obj
            "data" => !|Obj ^-> !|Obj
        ]

    let QuadtreeNode =
        Class "QuadtreeNode"
        |+> Instance [
            "nodes" =? TSelf * TSelf * TSelf * TSelf
            "leaf"  =? Bool
            "point" =? Float2T
            "x"     =? Float
            "y"     =? Float
        ]
        |>! addToClassList

    let QuadtreeRoot =
        ChainedClassNewInherits "QuadtreeRoot" QuadtreeNode <| fun chained ->
        [
            "add"   => Float2T ^-> O
            "visit" => (QuadtreeNode?node * Float?x1 * Float?y1 * Float?x2 * Float?y2 ^-> Bool)?callback ^-> O
        ]

    let Quadtree =
        ChainedClassNew "Quadtree" <| fun chained ->
        [
            "apply"  => (!|Float2T)?points ^-> QuadtreeRoot |> WithInline "$this($points)"
            "x"     => getSetVal chained (Obj ^-> Float)
            "y"     => getSetVal chained (Obj ^-> Float)
            "extent" => getSetVal chained Float2x2T
            "cover" => Float * Float ^-> TSelf
            "add" => Date ^-> TSelf
            "addAll" => Date ^-> TSelf
            "remove" => Date ^-> TSelf
            "removeAll" => Date ^-> TSelf
            "copy" => O ^-> TSelf
            "root" => O ^-> TreeNode
            "data" => O ^-> !| Obj
            "size" => O ^-> Int
            "find" => Int * Int * !? Int ^-> Date
            "visit" => (TreeNode * Int * Int * Int * Int ^-> Bool) ^-> TSelf
            "visitAfter" => (TreeNode * Int * Int * Int * Int ^-> Bool) ^-> TSelf
        ]

    let PolygonClass =
        ChainedClassNew "Polygon" (fun chained ->
            [
                "area"     => O ^-> Float
                "centroid" => O ^-> Float2T
                "hull"     => chained TSelf
                "contains" => Int * Int ^-> Bool
                "length" => O ^-> Int
            ]
        )

    let Graticule =
        ChainedClassNew "Graticule" <| fun chained ->
            [
                "lines" => O ^-> !|LineString
                "outline" => O ^-> PolygonClass
                "precision" => getSetVal chained Float
            ]
            @ propF4 TSelf "extent"
            @ propF4 TSelf "minorExtent"
            @ propF4 TSelf "majorExtent"
            @ propF2 TSelf "step"
            @ propF2 TSelf "majorStep"
            @ propF2 TSelf "minorStep"

    let Hull =
        ChainedClassNew "Hull" <| fun chained ->
        [
            "apply" => !|Obj ^-> !|Obj
            "x"     => getSetVal chained (Obj ^-> Float)
            "y"     => getSetVal chained (Obj ^-> Float)
        ]

    let DragEvent =
        EnumStrings "DragEvent" [
            "dragstart"
            "drag"
            "dragend"

            "target"
            "type"
            "subject"
            "x"
            "y"
            "dx"
            "dy"
            "identifier"
            "active"
            "sourceEvent"
        ]

    let Drag =
        ChainedClassNew "Drag" <| fun chained ->
        [
            "origin" => getSetVal chained (selectionCallback Float2T)
            "on"     => (DragEvent?``type`` ^-> (O ^-> O)) + chained (DragEvent?``type`` * (O ^-> O)?listener)
            "container" => getSetVal chained TSelf
            "filter" => getSetVal chained TSelf
            "touchable" => getSetVal chained TSelf
            "subject" => getSetVal chained TSelf
            "clickDistance" => getSetVal chained TSelf
        ]

    let ZoomEvent =
        EnumStrings "ZoomType" [
            "zoomstart"
            "zoom"
            "zoomend"
        ]

    let Zoom =
        ChainedClassNew "Zoom" <| fun chained ->
            [
                "scale"       => getSetVal chained Float
                "x"           => getSetVal chained Scale
                "y"           => getSetVal chained Scale
                "on"          => (ZoomEvent?``type`` ^-> (O ^-> O)) + chained (ZoomEvent?``type`` * (O ^-> O)?listener)
                "event"       => (Selection.[Obj] + Transition.[Obj])?selection ^-> O
                "transform" => Selection.[Obj] * TSelf ^-> O
                "translateBy" => Selection.[Obj] * Int * Int ^-> O
                "translateTo" => Selection.[Obj] * Int * Int ^-> O
                "scaleBy" => Selection.[Obj] * Int ^-> O
                "scaleTo" => Selection.[Obj] * Int ^-> O
                "constrain" => chained T<JavaScript.Function>
                "filter" => chained T<JavaScript.Function>
                "touchable" => chained T<JavaScript.Function>
                "wheelDelta" => chained T<JavaScript.Function>
                "extent" => chained T<JavaScript.Function>
                "scaleExtent" => chained T<JavaScript.Function>
                "translateExtent" => chained T<JavaScript.Function>
                "clickDistane" => chained !?Int
                "tapDistane" => chained !?Int
                "duration" => chained !?Int
                "interpolate" => chained !? T<JavaScript.Function>
            ]
            @ propF2 TSelf "center"
            @ propF2 TSelf "size"
            @ propF2 TSelf "scaleExtent"
            @ propF2 TSelf "translate"

    let Bisector =
        ChainedClassNew "Bisector" <| fun chained ->
        [
            Generic - fun t -> "apply" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int |> WithInline "$this($arguments)"
            
            "left" => Iterable * Value * !? Int * !? Int ^-> Point
            "right" => Iterable * Value * !? Int * !? Int ^-> Point
            "center" => Iterable * Value * !? Int * !? Int ^-> Point
            "quickselect" => Iterable * Int * Int * (Value * Value ^-> Int)?compare ^-> Iterable
            "ascending" => Value * Value ^-> Int
            "descending" => Value * Value ^-> Int
        ]

    let Prefix =
        Record "Prefix" [
            "symbol" , String
            "scale"  , Float ^-> Float
        ]

    let Dispatcher =
        ChainedClassNew "Dispatcher" <| fun chained ->
        [
            "on" => String
            "copy" => O ^-> TSelf
            "call" => String + TSelf + !| Obj ^-> O
            "apply" => String + TSelf + !| Obj ^-> O
        ]

    let SvgTransform =
        Class "SvgTransform"
        |+> Static [
            "matrix" => Float?a * Float?b * Float?c * Float?d * Float?e * Float?f ^-> String
                |> WithInline "'matrix(' + $a + ',' + $b + ',' + $c + ',' + $d + ',' + $e + ',' + $f + ')'"
            "translate" => Float?tx ^-> String
                |> WithInline "'translate(' + $tx + ')'"
            "translate" => Float?tx * Float?ty ^-> String
                |> WithInline "'translate(' + $tx + ',' + $ty + ')'"
            "scale" => Float?sx ^-> String
                |> WithInline "'scale(' + $sx + ')'"
            "scale" => Float?sx * Float?sy ^-> String
                |> WithInline "'scale(' + $sx + ',' + $sy + ')'"
            "rotate" => Float?rotateAngle ^-> String
                |> WithInline "'rotate(' + $rotateAngle + ')'"
            "rotate" => Float?rotateAngle * Float?cx * Float?cy ^-> String
                |> WithInline "'rotate(' + $rotateAngle + ',' + $cx + ',' + $cy + ')'"
            "skewX" => Float?skewAngle ^-> String
                |> WithInline "'skewX(' + skewAngle + ')'"
            "skewY" => Float?skewAngle ^-> String
                |> WithInline "'skewY(' + skewAngle + ')'"
        ]
        |>! addToClassList

    let Simulation =
        ChainedClassNew "Simulation" <| fun chained ->
        [
            "restart" => O ^-> TSelf
            "stop" => O ^-> TSelf
            "tick" => Int ^-> TSelf
            "nodes" => getSetVal chained !| Obj
            "alpha" => getSetVal chained Int
            "alphaMin" => getSetVal chained Int
            "alphaDecay" => getSetVal chained Int
            "alphaTarget" => getSetVal chained Int
            "velocityDecay" => getSetVal chained Int
            "force" => String ^-> getSetVal chained TSelf
            "find" => Int * Int * !? Int ^-> Node
            "randomSource" => getSetVal chained (O ^-> Float)
            "on" => getSetVal chained Listener
        ]

    let Center =
        ChainedClassNew "Center" <| fun chained ->
        [
            "x" => getSetVal chained Int
            "y" => getSetVal chained Int
            "strength" => getSetVal chained Float
        ]

    let Collide =
        ChainedClassNew "Collide" <| fun chained ->
        [
            "radius" => getSetVal chained Float
            "strength" => getSetVal chained Float
            "iterations" => getSetVal chained Int
        ]
    
    let ManyBody =
        ChainedClassNew "ManyBody" <| fun chained ->
        [
            "strength" => getSetVal chained Float
            "theta" => getSetVal chained Float
            "distance" => getSetVal chained Int
            "distanceMin" => getSetVal chained Int
            "distanceMax" => getSetVal chained Int
        ]   

    let ForceX =
        ChainedClassNew "X" <| fun chained ->
        [
            "strength" => getSetVal chained Float
            "x" => getSetVal chained Int
        ]

    let ForceY =
        ChainedClassNew "Y" <| fun chained ->
        [
            "strength" => getSetVal chained Float
            "y" => getSetVal chained Int
        ]

    let Radial =
        ChainedClassNew "Radial" <| fun chained ->
        [
            "strength" => getSetVal chained Float
            "radius" => getSetVal chained Int
            "x" => getSetVal chained Int 
            "y" => getSetVal chained Int 
        ]

    let Dsv =
        ChainedClassNew "Dsv" <| fun chained ->
        [
            "parse" => String * !? String ^-> !| Obj
            "parseRows" => String * !? String ^-> !| Obj
            "format" => !| Obj * !| String ^-> !| String
            "formatRows" => !| !| String ^-> String
            "formatRow" => !| String ^-> String
            "formatValue" => Obj ^-> String
        ]

    let Voronoy =
        Class "Voronoy"
        |+> Instance [
            "circumcenters" =? !| Float
            "vectors" =? !| Float
            "xmin" =? Int
            "ymin" =? Int
            "xmax" =? Int
            "ymax" =? Int

            "contains" => Int * Int * Int ^-> Bool
            "neighbors" => Int ^-> !| Int
            "render" => Obj ^-> O
            "renderBounds" => Obj ^-> O
            "renderCell" => Obj ^-> O
            "cellPolygons" => O ^-> !| !| Point
            "cellPolygon" => !| Point
            "update" => O ^-> O
        ]

    let Delaunay =
        Class "Delaunay"
        |+> Static [
            Constructor(!| Point)
        ]
        |+> Instance [
            "from" => !| Point * !? (Point ^-> Int) * !? (Point ^-> Int) ^-> TSelf
            "find" => Int * Int * !? Int ^-> Int
            "neighbors" => Point ^-> !| Point
            "renderer" => !? Obj ^-> O
            "renderHull" => !? Obj ^-> O
            "renderTriangle" => !| Int * !? Obj ^-> O
            "renderPoints" => !? Obj * !? Int ^-> O
            "hullPolygon" => O ^-> !| Point
            "trianglePolygon" => !| Int ^-> !| Point
            "update" => O ^-> O
            "voronoi" => !| Int ^-> Voronoi
            
            "points" =? !| Int
            "halfedges" =? !| Int
            "hull" =? !| Int
            "triangles" =? !| Int
            "inedges" =? !| Int
        ]

    let Contours =
        ChainedClassNew "Contours" <| fun chained ->
        [
            "contour" => !| Int + !| Float * Int + Float ^-> Obj
            "size" => getSetVal chained Int
            "smoothing" => getSetVal chained Bool
            "tresholds" => getSetVal chained (!| Int + !| Float)
        ]

    let ContourDensity =
        ChainedClassNew "ContourDensity" <| fun chained ->
        [
            "data" => !| Int + !| Float ^-> !| Obj
            "x" => getSetVal chained Int
            "y" => getSetVal chained Int
            "weight" => getSetVal chained Float
            "size" => getSetVal chained !| Int
            "cellSize" => getSetVal chained Int
            "treshold" => getSetVal chained (!| Int + !| Float)
            "bandWidth" => getSetVal chained Float
        ]

    let Color =
        ChainedClassNew "Color" <| fun chained ->
        [
            "opacity" =? Float
            "rgb" => O ^-> TSelf
            "copy" => !| Obj ^-> TSelf
            "brighter" => Float + Int ^-> TSelf
            "darker" => Float + Int ^-> TSelf
            "displayable" => O ^-> Bool
            "formatHex" => O ^-> String
            "formatHsl" => O ^-> String
            "formatRgb" => O ^-> String
            "toString" => O ^-> String
        ]

    let Adder =
        ChainedClassNew "Adder" <| fun chained ->
        [
            "add" => Int + Float ^-> TSelf
            "valueOf" => O ^-> Int + Float
        ]

    let Bin =
        ChainedClassNew "Bin" <| fun chained ->
        [
            "value" => getSetVal chained (O ^-> Value)
            "domain" => getSetVal chained Obj
            "tresholds" => getSetVal chained !| Value
        ]

    let D3 =
        Class "d3"
        |+> Static [
            // Selections
            "select"      => selector ^-> Selection.[Obj]
            "selectAll"   => selector ^-> Selection.[Obj]
            "selection"   => O ^-> Selection.[Obj]
            "event"       =? Event
            "mouse"       => Element?container ^-> Int2T
            "touches"     => Element?container * !?Obj?touches ^-> !|Int2x2T

            // Transitions
            "transition"  => !?Selection.[Obj]?selection ^-> Transition.[Obj]
            "ease"        => (String?``type`` *+ Float) ^-> easing
            "timer"       => O ^-> Bool?``function`` * !?Int?delay * !?T<System.DateTime>?time ^-> O

            // Working with Arrays
            "ascending"   => Float?a * Float?b ^-> Int
            "descending"  => Float?a * Float?b ^-> Int
            Generic - fun t   -> "min" => (!|t)?array ^-> t
            Generic -- fun t u -> "min" => (!|t)?array * (t ^-> u)?accessor ^-> u
            Generic - fun t   -> "max" => (!|t)?array ^-> t
            Generic -- fun t u -> "max" => (!|t)?array * (t ^-> u)?accessor ^-> u
            Generic - fun t   -> "extent" => (!|t)?array ^-> t * t
            Generic -- fun t u -> "extent" => (!|t)?array * (t ^-> u)?accessor ^-> u * u
            Generic - fun t   -> "mean" => (!|t)?array ^-> t
            Generic -- fun t u -> "mean" => (!|t)?array * (t ^-> u)?accessor ^-> u
            Generic - fun t   -> "median" => (!|t)?array ^-> t
            Generic -- fun t u -> "median" => (!|t)?array * (t ^-> u)?accessor ^-> u
            Generic - fun t   -> "quantile" => (!|t)?numbers * Float?p ^-> t
            Generic - fun t   -> "bisectLeft" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int
            Generic - fun t   -> "bisect" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int
            Generic - fun t   -> "bisectRight" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int
            Generic -- fun t u -> "bisector" => (t ^-> u) ^-> Bisector
            Generic - fun t   -> "shuffle" => (!|t)?array ^-> !|t
            "keys"    => Obj?``object`` ^-> !|String
            "values"  => Obj?``object`` ^-> !|Obj
            "entries" => Obj?``object`` ^-> !|KeyValuePair
            "map"     => !?Obj?``object`` ^-> Map
            "set"     => !?(!|String)?array ^-> Set
            Generic - fun t -> "merge" => !+ !|t ^-> !|t
            "range"     => (!?Float?start * Float?stop * !?Float?step ^-> !|Float) + (!?Int?start * Int?stop * !?Int?step ^-> !|Int)
            Generic - fun t -> "permute" => (!|t)?array * (!|Int)?indexes ^-> !|t
            "zip" => !+ !|Obj ^-> !|Obj
            Generic - fun t -> "transpose" => (!| !|t)?matrix ^-> !| !|t
            Generic - fun t -> "pairs" => (!|t)?array ^-> !|(t * t)

            "nest" => O ^-> Nest

            "transform" => String?string ^-> Transform

            "format" => String?specifier ^-> Float?number ^-> String
            "formatPefix" => Float?value * !?Float?precision ^-> Prefix
            "formatSpecifier" => String ^-> Obj
            "precisionFixed" => Float ^-> Float
            "precisionPrefix" => Float * Float ^-> Float
            "precisionRound" => Float * Float ^-> Float
            "formatLocale" => Obj ^-> Obj
            "formatDefaultLocale" => Obj ^-> Obj
            "requote" => String ^-> String
            "round" => Float?x * Int?n ^-> Float

            Generic - fun t -> "functor" => t ^-> O ^-> t
            "rebind" => Obj?target * Obj?source *+ String ^-> O
            "dispatch" => !+ String ^-> Dispatcher

            //time

            "timeFormat" => String ^-> (Date ^-> String)
            "timeParse" => String ^-> (String ^-> Date)
            "utcFormat" => String ^-> (Date ^-> String)
            "utcParse" => String ^-> (String ^-> Date)
            "isoFormat" =? (Date ^-> String)
            "isoParse" =? (String ^-> Date)

            "interval" => (Date ^-> Date) * (Date * Int ^-> Date) * !? (Date * Date ^-> Int) * !? (Date ^-> Int) ^-> TimeInterval

            "timeMillisecond" =? TimeInterval
            "utcMillisecond" =? TimeInterval
            "timeSecond" =? TimeInterval
            "utcSecond" =? TimeInterval
            "timeMinute" =? TimeInterval
            "utcMinute" =? TimeInterval
            "timeHour" =? TimeInterval
            "utcHour" =? TimeInterval
            "timeDay" =? TimeInterval
            "utcDay" =? TimeInterval
            "timeWeek" =? TimeInterval
            "utcWeek" =? TimeInterval
            "timeMonth" =? TimeInterval
            "utcMonth" =? TimeInterval
            "timeYear" =? TimeInterval
            "utcYear" =? TimeInterval
            
            "timeMonday" =? TimeInterval
            "utcMonday" =? TimeInterval
            "timeTuesday" =? TimeInterval
            "utcTuesday" =? TimeInterval
            "timeWednesday" =? TimeInterval
            "utcWednesday" =? TimeInterval
            "timeThursday" =? TimeInterval
            "utcThursday" =? TimeInterval
            "timeFriday" =? TimeInterval
            "utcFriday" =? TimeInterval
            "timeSaturday" =? TimeInterval
            "utcSaturday" =? TimeInterval
            "timeSunday" =? TimeInterval
            "utcSunday" =? TimeInterval

            "timeTicks" => Date * Date * Int ^-> !| Date
            "utcTicks" => Date * Date * Int ^-> !| Date
            "timeTickInterval" => Date * Date * Int ^-> !| TimeInterval
            "utcTickInterval" => Date * Date * Int ^-> !| TimeInterval

            //timer

            "timer" => (Int ^-> O) * !? Int ^-> Timer
            "timerFlush" => O ^-> O
            "timeout" => (Int ^-> O) * !? Int ^-> Timer
            "interval" => (Int ^-> O) * !? Int ^-> Timer

            //curve

            Generic - fun t -> "curveBasis" => t ^-> !| Point
            Generic - fun t -> "curveBasisClosed" => t ^-> !| Point
            Generic - fun t -> "curveBasisOpen" => t ^-> !| Point
            Generic - fun t -> "curveBumpX" => t ^-> !| Point
            Generic - fun t -> "curveBumpY" => t ^-> !| Point
            Generic - fun t -> "curveBundle" => t ^-> !| Point
            "curveBundle" => Int ^-> !| Point
            Generic - fun t -> "curveCardinal" => t ^-> !| Point
            Generic - fun t -> "curveCardinalClosed" => t ^-> !| Point
            Generic - fun t -> "curveCardinalOpen" => t ^-> !| Point
            Generic - fun t -> "curveCatMulRom" => t ^-> !| Point
            Generic - fun t -> "curveCatMulRomClosed" => t ^-> !| Point
            Generic - fun t -> "curveCatMulRomOpen" => t ^-> !| Point
            Generic - fun t -> "curveLinear" => t ^-> !| Point
            Generic - fun t -> "curveLinearClosed" => t ^-> !| Point
            Generic - fun t -> "curveMonotoneX" => t ^-> !| Point
            Generic - fun t -> "curveMonotonY" => t ^-> !| Point
            Generic - fun t -> "curveNatural" => t ^-> !| Point
            Generic - fun t -> "curveStep" => t ^-> !| Point
            Generic - fun t -> "curveStepAfter" => t ^-> !| Point
            Generic - fun t -> "curveStepBefore" => t ^-> !| Point

            "curve" => Curve

            //link
            
            Generic - fun t -> "link" => Obj ^-> Link.[t]
            Generic - fun t -> "linkVertical" => O ^-> Link.[t]
            Generic - fun t -> "linkHorizontal" => O ^-> Link.[t]
            Generic - fun t -> "linkRadial" => O ^-> Link.[t]

            //hierarchy

            "node" => O ^-> Node
            "stratify" => O ^-> Stratify

            //projection

            "path" => O ^-> Path

            "projection" => O ^-> Projection

            "geoAzimuthalEqualArea" => O ^-> Projection
            "geoAzimuthalEqualAreaRaw" =? Projection
            
            "geoAzimuthalEquidistant" => O ^-> Projection
            "geoAzimuthalEquidistantRaw" =? Projection
            
            "geoGnomonic" => O ^-> Projection
            "geoGnomonicRaw" =? Projection
            
            "geoOrthographic" => O ^-> Projection
            "geoOrthographicRaw" =? Projection
            
            "geoStereographic" => O ^-> Projection
            "geoStereographicRaw" =? Projection
            
            "geoEqualEarth" => O ^-> Projection
            "geoEqualEarthRaw" =? Projection
            
            "geoAlbersUsa" => O ^-> Projection
            
            "geoAlbers" => O ^-> Projection

            "geoConicConformal" => O ^-> Projection
            "geoConicConformalRaw" => Int * Int ^-> Projection

            "geoConicEqualArea" => O ^-> Projection
            "geoConicEqualAreaRaw" => Int * Int ^-> Projection

            "geoConicEquidistant" => O ^-> Projection
            "geoConicEquidistantRaw" => Int * Int ^-> Projection

            "geoEquirectangular" => O ^-> Projection
            "geoEquirectangularRaw" =? Projection
            
            "geoMercator" => O ^-> Projection
            "geoMercatorRaw" =? Projection
            
            "geoTransfersMercator" => O ^-> Projection
            "geoTransfersMercatorRaw" =? Projection
            
            "geoNaturalEarth1" => O ^-> Projection
            "geoNaturalEarth1Raw" =? Projection
            
            //forces

            "forceSimulation" => !? !| Node ^-> Simulation
            "forceCenter" => !? (Int * Int) ^-> Center
            "forceCollide" => !? Float ^-> Collide
            "forceManyBody" => O ^-> ManyBody
            "forceX" => !? Int ^-> ForceX
            "forceY" => !? Int ^-> ForceY
            "forceRadial" => Int * !? Int * !? Int ^-> Radial

            //fetches
            
            "blob" => FetchRequest ^-> T<JavaScript.Promise<JavaScript.Blob>>
            "buffer" => FetchRequest ^-> T<JavaScript.Promise<JavaScript.ArrayBuffer>>
            "html" => FetchRequest ^-> T<JavaScript.Promise<JavaScript.Dom.Document>>
            "image" => FetchRequest ^-> T<JavaScript.Promise<JavaScript.ImageData>> //?
            "json" => FetchRequest ^-> T<JavaScript.Promise<obj>>
            "text" => FetchRequest ^-> T<JavaScript.Promise<string>>
            "xml" => FetchRequest ^-> T<JavaScript.Promise<JavaScript.Dom.Document>>

            "csv" => FetchRequest * !? Int ^-> T<JavaScript.Promise<string>>
            "dsv" => FetchRequest * !? Int ^-> T<JavaScript.Promise<string>>
            "svg" => FetchRequest ^-> T<JavaScript.Promise<string>>
            "tsv" => FetchRequest * !? Int ^-> T<JavaScript.Promise<string>>
            
            //ease

            "easeLinear" => Float ^-> Float

            "easePolyIn" => Float ^-> Float
            "easePolyOut" => Float ^-> Float
            "easePoly" => Float ^-> Float
            "easePolyInOut" => Float ^-> Float

            "easeQuadIn" => Float ^-> Float
            "easeQuadOut" => Float ^-> Float
            "easeQuad" => Float ^-> Float
            "easeQuadInOut" => Float ^-> Float
            
            "easeCubicIn" => Float ^-> Float
            "easeCubicOut" => Float ^-> Float
            "easeCubic" => Float ^-> Float
            "easeCubicInOut" => Float ^-> Float
                        
            "easeSinIn" => Float ^-> Float
            "easeSinOut" => Float ^-> Float
            "easeSin" => Float ^-> Float
            "easeSinInOut" => Float ^-> Float
                                    
            "easeExpIn" => Float ^-> Float
            "easeExpOut" => Float ^-> Float
            "easeExp" => Float ^-> Float
            "easeExpInOut" => Float ^-> Float
                                                
            "easeCircleIn" => Float ^-> Float
            "easeCircleOut" => Float ^-> Float
            "easeCircle" => Float ^-> Float
            "easeCircleInOut" => Float ^-> Float
                                                         
            "easeElasticIn" => Float ^-> Float
            "easeElasticOut" => Float ^-> Float
            "easeElastic" => Float ^-> Float
            "easeElasticInOut" => Float ^-> Float
                                                                    
            "easeBackIn" => Float ^-> Float
            "easeBackOut" => Float ^-> Float
            "easeBack" => Float ^-> Float
            "easeBackInOut" => Float ^-> Float
                                                                                
            "easeBounceIn" => Float ^-> Float
            "easeBounceOut" => Float ^-> Float
            "easeBounce" => Float ^-> Float
            "easeBounceInOut" => Float ^-> Float
            
            //dsv

            "dsvFormat" => T<char> ^-> Dsv
            "autoType" => Obj ^-> Obj

            //contours

            "contours" => O ^-> !| Int + !| Float ^-> !| Contours
            "contourDensity" => O ^-> ContourDensity

            //colors

            "schemeCategory10" =? !| String
            "schemeAccent" =? !| String
            "schemeDark2" =? !| String
            "schemePaired" =? !| String
            "schemePastel1" =? !| String
            "schemePastel2" =? !| String
            "schemeSet1" =? !| String
            "schemeSet2" =? !| String
            "schemeSet3" =? !| String
            "schemeTableau10" =? !| String
            "interpolateBrBG" => Float ^-> String
            "interpolatePRGn" => Float ^-> String
            "interpolatePiYG" => Float ^-> String
            "interpolatePuOr" => Float ^-> String
            "interpolateRdBu" => Float ^-> String
            "interpolateRdGy" => Float ^-> String
            "interpolateRdYlBu" => Float ^-> String
            "interpolateRdYlGn" => Float ^-> String
            "interpolateSpectral" => Float ^-> String
            "interpolateBlues" => Float ^-> String
            "interpolateGreens" => Float ^-> String
            "interpolateGreys" => Float ^-> String
            "interpolateOranges" => Float ^-> String
            "interpolatePurples" => Float ^-> String
            "interpolateReds" => Float ^-> String
            "interpolateTurbo" => Float ^-> String
            "interpolateViridis" => Float ^-> String
            "interpolateInferno" => Float ^-> String
            "interpolateMagma" => Float ^-> String
            "interpolatePlasma" => Float ^-> String
            "interpolateCividis" => Float ^-> String
            "interpolateWarm" => Float ^-> String
            "interpolateCool" => Float ^-> String
            "interpolateCubeHelixDefault" => Float ^-> String
            "interpolateBuGn" => Float ^-> String
            "interpolateBuPu" => Float ^-> String
            "interpolateGnBu" => Float ^-> String
            "interpolateOrRd" => Float ^-> String
            "interpolatePuBuGn" => Float ^-> String
            "interpolatePuBu" => Float ^-> String
            "interpolatePuRd" => Float ^-> String
            "interpolateRdPu" => Float ^-> String
            "interpolateYlGnBu" => Float ^-> String
            "interpolateYlGn" => Float ^-> String
            "interpolateYlOrBr" => Float ^-> String
            "interpolateYlOrRd" => Float ^-> String
            "interpolateRainbow" => Float ^-> String
            "interpolateSinebow" => Float ^-> String

            "color" => String ^-> Color

            "rgb" => Int * Int * Int * !? Int ^-> Color
            "rgb" => String ^-> Color
            "rgb" => Color ^-> Color
            
            "hsl" => Int * Int * Int * !? Int ^-> Color
            "hsl" => String ^-> Color
            "hsl" => Color ^-> Color
            
            "lab" => Int * Int * Int * !? Int ^-> Color
            "lab" => String ^-> Color
            "lab" => Color ^-> Color
            
            "hcl" => Int * Int * Int * !? Int ^-> Color
            "hcl" => String ^-> Color
            "hcl" => Color ^-> Color
            
            "lch" => Int * Int * Int * !? Int ^-> Color
            "lch" => String ^-> Color
            "lch" => Color ^-> Color
            
            "cubehelix" => Int * Int * Int * !? Int ^-> Color
            "cubehelix" => String ^-> Color
            "cubehelix" => Color ^-> Color
            
            "gray" => Int  * !? Int ^-> Color

            //chords

            "ribbon" => Ribbon
            "ribbonArrow" => RibbonArrow

            //axis

            "axisTop" => !| !| Int ^-> Axis
            "axisRight" => !| !| Int ^-> Axis
            "axisBottom" => !| !| Int ^-> Axis
            "axisLeft" => !| !| Int ^-> Axis

            //statistics

            "min" => Iterable ^-> Value
            "minIndex" => Iterable ^-> Int
            "max" => Iterable ^-> Value
            "maxIndex" => Iterable ^-> Int
            "extent" => Iterable ^-> !| Value
            "mode" => Iterable ^-> Value
            "mean" => Iterable ^-> Value
            "median" => Iterable ^-> Value
            "sum" => Iterable ^-> Int + Float
            "cumsum" => Iterable ^-> Int + Float
            "quantile" => Iterable * Int + Float ^-> Int
            "quantileSorted" => Iterable * Int + Float ^-> Int
            "variance" => Iterable ^-> Int + Float
            "deviation" => Iterable ^-> Int + Float
            "fsum" => !| Int + !| Float ^-> Int + Float
            "fcumsum" => !| Int + !| Float ^-> Int + Float

            "adder" => O ^-> Adder

            //search

            "least" => Iterable ^-> !| Value
            "leastIndex" => Iterable ^-> !| Int
            "greatest" => Iterable ^-> !| Value
            "greatestIndex" => Iterable ^-> !| Int
            "bisectLeft" => Iterable * Value * !? Int * !? Int ^-> Point
            "bisect" => Iterable * Value * !? Int * !? Int ^-> Point
            "bisectRight" => Iterable * Value * !? Int * !? Int ^-> Point
            "bisectCenter" => Iterable * Value * !? Int * !? Int ^-> Point

            "bisector" => (Obj ^-> Value) ^-> Bisector
            "bisector" => (Obj  * Value ^-> Value) ^-> Bisector

            //transformations

            "group" => Iterable * (Obj ^-> Value) ^-> Map
            "groups" => Iterable * (Obj ^-> Value) ^-> !| Iterable
            "flatGroup" => Iterable * (Obj ^-> Value) ^-> !| Iterable
            "index" => Iterable * (Obj ^-> Value) ^-> Map
            "indexes" => Iterable * (Obj ^-> Value) ^-> Iterable
            "rollup" => Iterable * (Obj ^-> Value) ^-> Map
            "rollups" => Iterable * (Obj ^-> Value) ^-> Iterable
            "flatRollup" => Iterable * (Obj ^-> Value) ^-> Iterable

            "groupSort" => Iterable * (Value * Value ^-> Int) * (Obj ^-> Value) ^-> !| Value
            "count" => Iterable ^-> Int
            "cross" => !| Iterable ^-> !| !| Value
            "merge" => !| Iterable ^-> Iterable
            "pair" => Value * Value ^-> Iterable
            "permute" => Iterable * !| Int ^-> Iterable
            "shuffle" => Iterable * !? Int * !? Int ^-> Iterable

            "ticks" => Int * Int * Int ^-> !| Int
            "tickIncrement" => Int * Int * Int ^-> !| Int
            "tickStep" => Int * Int * Int ^-> Int
            "nice" => Int * Int * Int ^-> !| Int
            "range" => !? Int * Int * !? Int + Float ^-> !| Int + Float
            "transpose" => !| !| Value ^-> !| Value
            "zip" => !| Value * !| Value ^-> !| !| Value

            //iterables

            "every" => Iterable * (Value ^-> Bool) ^-> Bool
            "some" => Iterable * (Value ^-> Bool) ^-> Bool
            "filter" => Iterable * (Value ^-> Bool) ^-> Iterable
            "map" => Iterable * (Value ^-> Value) ^-> Iterable
            "reduce" => Iterable * (Value * Value ^-> Value) ^-> Value
            "reverse" => Iterable ^-> Iterable
            "sort" => Iterable * (Value * Value ^-> Int) ^-> Iterable

            //sets

            "difference" => Iterable * !| Iterable ^-> Set
            "union" => !| Iterable ^-> Set
            "intersection" => !| Iterable ^-> Set
            "superset" => Iterable * Iterable ^-> Bool
            "subset" => Iterable * Iterable ^-> Bool
            "disjoint" => Iterable * Iterable ^-> Bool

            //bins

            "bin" => O ^-> Bin

            "tresholdFreedmanDiaconis" => !| Value * Value * Value ^-> Int
            "tresholdScott" => !| Value * Value * Value ^-> Int
            "tresholdSturges" => !| Value ^-> Int

            //interning

            "internMap" => !? Iterable * !? Value ^-> Map 
            "internSet" => !? Iterable * !? Value ^-> Set 
        ]
        |+> Static (
                // interpolation section
                let ipr t = Float ^-> t
                let ipFactory (x: Type.Type) (y: Type.Type) = x?arg1 * x?arg2 ^-> ipr y
                let ipF x = ipFactory x x
                [
                    Generic - fun t -> "interpolate" => ipF t.Type
                    "interpolateNumber" => ipF Float
                    "interpolateRound" => ipFactory Float Int
                    "interpolateString" => ipF String
                    "interpolateRgb" => ipFactory (Rgb + String) String
                    "interpolateHsl" => ipFactory (Hsl + String) String
                    "interpolateLab" => ipFactory (Lab + String) String
                    "interpolateHcl" => ipFactory (Hcl + String) String
                    Generic - fun t -> "interpolateArray" => ipF !|t.Type
                    Generic - fun t -> "interpolateObject" => ipF t.Type
                    "interpolateTransform" => ipF Transform.Type
                    "interpolateZoom" => ipF Float3T
                    "interpolators" =? !|(ipF Obj)
                ]
            )
        |+> Static (   // xhr section
                // approximate: need xhr-returning API
                // note: turns out d3 detects callback arity dynamically, and calls it differently!
                // therefore need extra care with tupled functions.
                let remote name t : list<CodeModel.IClassMember> =
                    [
                        name => String?url * (t ^-> O) ^-> O
                        name => String?url * (Obj?err * t ^-> O) ^-> O
                    ]
                let remoteMime name t : list<CodeModel.IClassMember> =
                    [
                        name => String?url * !?String?mimeType * (t ^-> O) ^-> O
                        name => String?url * (Obj?err * t ^-> O) ^-> O
                        name => String?url * String?mimeType * (Obj?err * t ^-> O) ^-> O
                    ]
                let doc = T<Document>
                List.concat [
                    remoteMime "text" String
                    remote "json" Obj
                    remoteMime "xml" doc
                    remote "html" doc
                    remote "csv" !|Obj
                    remote "tsv" !|Obj
                    ["xhr" => String?url * !?String?mimeType ^-> Xhr]
                    remoteMime "xhr" Xhr.Type
                ]
            )
        |=> Nested [

            Class "d3.timer"
            |+> Static [
                "flush" => O ^-> O
            ]
            Class "d3.random"
            |+> Static [
                "normal"    => !?Float?mean * !?Float?deviation ^-> Float
                "logNormal" => !?Float?mean * !?Float?deviation ^-> Float
                "irwinHall" => Int?count ^-> Float
                "bates" => Int?count ^-> Float
                "exponential" => Int?count ^-> Float
                "pareto" => Int?count ^-> Float
                "bernouli" => Int?count ^-> Float
                "geometric" => Int?count ^-> Float
                "binomial" => Int * Float ^-> Float
                "gamma" => Float * !? Float ^-> Float
                "beta" => Float * !? Float ^-> Float
                "weibull" => Float * !? Float * !? Float ^-> Float
                "cauchy" => Float ^-> Float
                "logistic" => !? Float * !? Float ^-> Float
                "poisson" => Float ^-> Float
                "uniform" => Float * !? Float ^-> (O ^-> Float)
                "int" => Int * !? Int ^-> (O ^-> Int)
                "logNormal" => Int * !? Int ^-> (O ^-> Int)
            ]
            Class "d3.ns"
            |+> Static [
                "prefix"  =? Map
                "qualify" => nameArg ^-> QualifiedNs
            ]
            Class "d3.time"
            |+> Static [
                "format"    => String?specifier ^-> TimeFormat

                "scale"     => O ^-> TimeScale

                "second"    =? TimeInterval
                "minute"    =? TimeInterval
                "hour"      =? TimeInterval
                "day"       =? TimeInterval
                "week"      =? TimeInterval
                "sunday"    =? TimeInterval
                "monday"    =? TimeInterval
                "tuesday"   =? TimeInterval
                "wednesday" =? TimeInterval
                "thursday"  =? TimeInterval
                "friday"    =? TimeInterval
                "saturday"  =? TimeInterval
                "month"     =? TimeInterval
                "year"      =? TimeInterval
            ]
            |=> Nested [
                Class "d3.time.format"
                |+> Static [
                    "utc" => String?specifier ^-> TimeFormat
                    "iso" =? TimeFormat
                ]
            ]
            Class "d3.layout"
            |+> Static [
                "bundle"    => O ^-> Bundle
                "chord"     => O ^-> ChordLayout
                "cluster"   => O ^-> Cluster
                "force"     => O ^-> Force
                "hierarchy" => O ^-> Hierarchy
                "histogram" => O ^-> Histogram
                "pack"      => O ^-> Pack
                "partition" => O ^-> Partition
                "pie"       => O ^-> Pie
                "stack"     => O ^-> Stack
                "tree"      => O ^-> Tree
                "treemap"   => O ^-> Treemap
            ]
            Class "d3.scale"
            |+> Static [
                "linear"    => O ^-> QuantitativeScale
                "sqrt"      => O ^-> QuantitativeScale
                "pow"       => O ^-> QuantitativeScale
                "log"       => O ^-> QuantitativeScale
                "quantize"  => O ^-> DiscreteScale
                "threshold" => O ^-> DiscreteScale
                "quantile"  => O ^-> QuantileScale
                "identity"  => O ^-> IdentityScale
                Generic - fun tData -> "ordinal"   => O ^-> OrdinalScale.[tData]
            ]
            Class "d3.geo"
            |+> Static [
                "path"      => O ^-> Path
                "graticule" => O ^-> Graticule
                "circle"    => O ^-> Circle
                "area"      => Feature ^-> Float
                "centroid"  => Feature ^-> Float2T
                "bounds"    => Feature ^-> Float2x2T
                "distance"  => Float2T?a * Float2T?b ^-> Float
                "rotation"  => (!|Float)?rotate ^-> Rotation
                "projection" => (Float2T ^-> Float2T)?raw ^-> Projection
                "projectionMutator" => (Float2T ^-> Float2T ^-> Float2T)?rawFactory ^-> Projection

                (* "albersUsa"            => O ^-> Projection
                "azimuthalEqualArea"   => O ^-> Projection
                "azimuthalEquidistant" => O ^-> Projection
                "conicConformal"       => O ^-> Projection
                "conicEquidistant"     => O ^-> Projection
                "conicEqualArea"       => O ^-> Projection
                "equirectangular"      => O ^-> Projection
                "gnomonic"             => O ^-> Projection
                "mercator"             => O ^-> Projection
                "orthographic"         => O ^-> Projection
                "stereographic"        => O ^-> Projection *)

//                "albersUsa.raw"            => Float2 ^-> Projection
//                "azimuthalEqualArea.raw"   => O ^-> Projection
//                "azimuthalEquidistant.raw" => O ^-> Projection
//                "conicConformal.raw"       => Float2 ^-> Projection
//                "conicEquidistant.raw"     => Float2 ^-> Projection
//                "equirectangular.raw"      => O ^-> Projection
//                "gnomonic.raw"             => O ^-> Projection
//                "mercator.raw"             => O ^-> Projection
//                "orthographic.raw"         => O ^-> Projection
//                "stereographic.raw"        => O ^-> Projection

            ]
            Class "d3.geom"
            |+> Static [
                "voronoi"  => O ^-> Voronoi
                "quadtree" => O ^-> Quadtree
                "polygon"  => (!|Float2T)?vertices ^-> PolygonClass
                "hull"     => O ^-> Hull
            ]
            Class "d3.behavior"
            |+> Static [
                "drag" => O ^-> Drag
                "dragDisable" => T<JavaScript.Window> ^-> O
                "dragEnable" => T<JavaScript.Window> ^-> O
                "zoom" => O ^-> Zoom
            ]
        ]
        |>! addToClassList

    let D3Assembly =
        Assembly [
            Namespace "WebSharper.D3" classList
            Namespace "WebSharper.D3.Resources" [
                (Resource "D3" "https://cdnjs.cloudflare.com/ajax/libs/d3/7.0.1/d3.min.js").AssemblyWide()
            ]
        ]

[<Sealed>]
type D3Extension() =
    interface IExtension with
        member ext.Assembly = Definition.D3Assembly

[<assembly: Extension(typeof<D3Extension>)>]
[<assembly: System.Reflection.AssemblyVersion("7.0.0.0")>]
do ()
