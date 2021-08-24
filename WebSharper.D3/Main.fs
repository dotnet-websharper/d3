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

    let ChainedG (members : ((Type.IParameters -> Type.Type) -> CodeModel.IClassMember list)) (c: CodeModel.Class) =
        c |+> Instance (members <| fun args -> args ^-> c)

    let Chained (members : ((Type.IParameters -> Type.Type) -> CodeModel.IClassMember list)) (c: CodeModel.Class) =
        c |+> Instance (members <| fun args -> args ^-> c)
        |>! addToClassList

    let ChainedClassG name (t: Type.Type) (members : ((Type.IParameters -> Type.Type) -> CodeModel.IClassMember list)) =
        Class name |=> t |+> Instance (members <| fun args -> args ^-> t)

    let ChainedClass name (t: Type.Type) (members : ((Type.IParameters -> Type.Type) -> CodeModel.IClassMember list)) =
        Class name |=> t |+> Instance (members <| fun args -> args ^-> t)
        |>! addToClassList

    let ChainedClassNew name members =
        Class name |> Chained members

    let ChainedClassNewInherits name inherits members =
        Class name |=> Inherits inherits |> Chained members

    let ChainedClassInheritsG name (t: Type.Type) inherits members =
        Class name |=> Inherits inherits |=> t |> ChainedG members

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
        let selfG = Type.New()
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
            Class "Selection"
            |=> selfG
            |+> Instance
                [
                    // TODO: NameValuePair
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

                    // TODO: datum, filter, sort, order, each - can make use of `t` parameter
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
        let self = Type.New()
        Class "Map"
        |=> self
        |+> Instance [
            "has"     => String?key ^-> Bool
            "get"     => String?key ^-> Float
            "set"     => String?key * Obj?value ^-> O
            "remove"  => String?key ^-> Bool
            "keys"    => O ^-> !|String
            "values"  => O ^-> !|Obj
            "entries" => O ^-> !|KeyValuePair
            "forEach" => WithThis self (String?key * Obj?value) O
        ]
        |>! addToClassList

    let Set =
        let self = Type.New()
        Class "Set"
        |=> self
        |+> Instance [
            "has"     => String?value ^-> Bool
            "add"     => String?value ^-> String
            "remove"  => String?value ^-> Bool
            "values"  => O ^-> !|String
            "forEach" => WithThis self String?value O
        ]
        |>! addToClassList

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

    /// TODO: check that this does not flatten arguments, when argType ~ Float3T for example.
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
        let self = Type.New()
        let gen =
            Generic - fun tData ->
            ChainedClassInheritsG "OrdinalScale" self.[tData] Scale <| fun chained ->
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

    let ChainedClassCoord name members =
        let self = Type.New()
        let gen =
            Generic - fun tData ->
            ChainedClassG name self.[tData] <| fun chained ->
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
            "projection" => getSetVal chained (Float2T ^-> Float2T) // TODO: seems to use .x/.y obj now in domain; range OK
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
            "innerTickSize" => getSetVal chained Int
            "outerTickSize" => getSetVal chained Int
            "tickPadding" => getSetVal chained Int
            "tickFormat" => getSetVal chained (Obj ^-> String)
        ]

    let BrushEvent =
        EnumStrings "BrushEvent" [
            "brushstart"
            "brush"
            "brushend"
        ]

    let Brush =
        ChainedClassNew "Brush" <| fun chained ->
        [
            "apply" => (Selection.[Obj] + Transition.[Obj])?selection ^-> O |> WithInline "$this($selection)"
            "x" => getSetVal chained Scale.Type
            "y" => getSetVal chained Scale.Type
            "extent" => getSetVal chained (Int2T + Int2x2T)
            "clamp" => getSetVal chained (Bool + Bool * Bool)
            "clear" => chained O
            "empty" => O ^-> Bool
            "on"    => (BrushEvent?``type`` ^-> (O ^-> O)) + chained (BrushEvent?``type`` * (O ^-> O)?listener)
            "event" => (Selection.[Obj] + Transition.[Obj])?selection ^-> O
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
        let self = Type.New()
        Record "BundleNode" [
            "parent" , self
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
            "padding" => getSetVal chained Float
            "sortGroups"    => getSetVal chained Comparator
            "sortSubGroups" => getSetVal chained Comparator
            "sortChords"    => getSetVal chained Comparator
            "chords" => O ^-> !|Link.[ChordNode]
            "groups" => O ^-> !|Link.[ChordNode]
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

    /// Pseudo-property getting and setting a 2-element double array [x,y,z].
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
        let self = Type.New()
        ChainedClass "Cluster" self <| fun chained ->
            [
                "nodes"      => ClusterNode?root ^-> !|ClusterNode
                "links"      => (!|ClusterNode)?nodes ^-> !|Link.[ClusterNode]
                "children"   => getSetVal chained (Obj ^-> Obj)
                "sort"       => getSetVal chained Comparator
                "separation" => getSetVal chained (ClusterNode * ClusterNode ^-> Int)
                "value"      => getSetVal chained (Obj ^-> Float)
            ]
            @ propF2 self "nodeSize"
            @ propF2 self "size"

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
        let self = Type.New()
        ChainedClass "Force" self <| fun chained ->
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
            @ propF2 self "size"

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
            "arc"       => Float?x * Float?y * Float?radius * Float?startAngle * Float?endAngle ^-> O
            "closePath" => O ^-> O
        ]
        |>! addToClassList

    let Feature =
        ChainedClassNew "Feature" <| fun chained -> []

    let ProjectionType = Type.New()

    let Path =
        ChainedClassNew "Path" <| fun chained ->
        [
            "call" => Obj?feature ^-> String
            |> WithInline "$0($1)"
            "call" => Obj?feature * Int?ix ^-> String
            |> WithInline "$0($1,$2)"
            "projection"  => getSetVal chained ((Float2T ^-> Float2T) + ProjectionType)
            "context"     => getSetVal chained (PathContext.Type + Obj)
            "pointRadius" => getSetVal chained Float
            "area"        => Feature ^-> Int
            "centroid"    => Feature ^-> Int2T
            "bounds"      => Feature ^-> Int2x2T
        ]

    let MultiLineString =
        ChainedClassNew "MultiLineString" <| fun chained -> []

    let LineString =
        ChainedClassNew "LineString" <| fun chained -> []

    let Polygon = Type.New()

    let Graticule =
        let self = Type.New()
        ChainedClass "Graticule" self <| fun chained ->
            [
                "lines" => O ^-> !|LineString
                "outline" => O ^-> Polygon
                "precision" => getSetVal chained Float
            ]
            @ propF4 self "extent"
            @ propF4 self "minorExtent"
            @ propF4 self "majorExtent"
            @ propF2 self "step"
            @ propF2 self "majorStep"
            @ propF2 self "minorStep"

    let Circle =
        let self = Type.New()
        ChainedClass "Circle" self <| fun chained ->
            [
                "origin"    => (Float2T + Obj ^-> Float2T) ^-> self
                "angle"     => getSetVal chained Float
                "precision" => getSetVal chained Float
            ]
            @ propF2 self "origin"

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
        let self = ProjectionType
        ChainedClass "Projection" self <| fun chained ->
            [
                "apply"      => Float2T?location ^-> Float2T |> WithInline "$this($location)"
                "invert"     => Float2T?location ^-> Float2T
                "scale"      => getSetVal chained Float
                "clipAngle"  => getSetVal chained Float
                "clipExtent" => getSetVal chained Float2x2T
                "precision"  => getSetVal chained Int
                "stream"     => Listener?listener ^-> Listener
            ]
            @ propF2 self "center"
            @ propF2 self "translate"
            @ propF3 self "rotate"

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
            "extent" => getSetVal chained Float2x2T // TODO: check
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
        let self = Type.New()
        Class "QuadtreeNode"
        |=> self
        |+> Instance [
            "nodes" =? self * self * self * self
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
        ]

    let PolygonClass =
        ChainedClass "Polygon" Polygon (fun chained ->
            [
                "area"     => O ^-> Float
                "centroid" => O ^-> Float2T
                "clip"     => chained Polygon
            ]
        )

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
        ]

    let Drag =
        ChainedClassNew "Drag" <| fun chained ->
        [
            "origin" => getSetVal chained (selectionCallback Float2T)
            "on"     => (DragEvent?``type`` ^-> (O ^-> O)) + chained (DragEvent?``type`` * (O ^-> O)?listener)
        ]

    let ZoomEvent =
        EnumStrings "ZoomType" [
            "zoomstart"
            "zoom"
            "zoomend"
        ]

    let Zoom =
        let self = Type.New()
        ChainedClass "Zoom" self <| fun chained ->
            [
                "scale"       => getSetVal chained Float
                "x"           => getSetVal chained Scale
                "y"           => getSetVal chained Scale
                "on"          => (ZoomEvent?``type`` ^-> (O ^-> O)) + chained (ZoomEvent?``type`` * (O ^-> O)?listener)
                "event"       => (Selection.[Obj] + Transition.[Obj])?selection ^-> O
                "transform" => Selection.[Obj] * self ^-> O
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
            @ propF2 self "center"
            @ propF2 self "size"
            @ propF2 self "scaleExtent"
            @ propF2 self "translate"

    let Bisector =
        ChainedClassNew "Bisector" <| fun chained ->
        [
            Generic - fun t -> "bisectLeft" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int
            Generic - fun t -> "apply" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int |> WithInline "$this($arguments)"
            Generic - fun t -> "bisectRight" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int
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
            "type" => String?``type`` *+ Obj ^-> O |> WithInline "$0.$type($arguments)"
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
            "fomatPefix" => Float?value * !?Float?precision ^-> Prefix
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

            //curve TODO

            "curve" => Curve

            //link
            
            Generic - fun t -> "link" => Obj ^-> Link.[t]
            Generic - fun t -> "linkVertical" => O ^-> Link.[t]
            Generic - fun t -> "linkHorizontal" => O ^-> Link.[t]
            Generic - fun t -> "linkRadial" => O ^-> Link.[t]
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
                // approximate: need xhr-returning API (TODO)
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
            Class "d3.svg"
            |+> Static [
                Generic - fun tData -> "line"        => O ^-> Line.[tData]
                Generic - fun tData -> "line.radial" => O ^-> RadialLine.[tData]
                Generic - fun tData -> "area"        => O ^-> Area.[tData]
                Generic - fun tData -> "area.radial" => O ^-> RadialArea.[tData]
                Generic - fun tData -> "arc"         => O ^-> Arc.[tData]
                "symbol"          => O ^-> Symbol

                "symbolCircle" =? Symbol
                "symbolCross" =? Symbol
                "symbolDiamond" =? Symbol
                "symbolSquare" =? Symbol
                "symbolStar" =? Symbol
                "symbolTriangle" =? Symbol
                "symbolWye" =? Symbol

                "symbolTypes"     =? !|SymbolType
                Generic - fun tData -> "chord"       => O ^-> Chord.[tData]
                "diagonal"        => O ^-> Diagonal
                "diagonal.radial" => O ^-> Diagonal
                "axis"            => O ^-> Axis
                "brush"           => O ^-> Brush
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

                "albersUsa"            => O ^-> Projection
                "azimuthalEqualArea"   => O ^-> Projection
                "azimuthalEquidistant" => O ^-> Projection
                "conicConformal"       => O ^-> Projection
                "conicEquidistant"     => O ^-> Projection
                "conicEqualArea"       => O ^-> Projection
                "equirectangular"      => O ^-> Projection
                "gnomonic"             => O ^-> Projection
                "mercator"             => O ^-> Projection
                "orthographic"         => O ^-> Projection
                "stereographic"        => O ^-> Projection

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
                "polygon"  => (!|Float2T)?vertices ^-> Polygon
                "hull"     => O ^-> Hull
            ]
            Class "d3.behavior"
            |+> Static [
                "drag" => O ^-> Drag
                "zoom" => O ^-> Zoom
            ]
        ]
        |>! addToClassList

    let D3Assembly =
        Assembly [
            Namespace "WebSharper.D3" classList
            Namespace "WebSharper.D3.Resources" [
                (Resource "D3" "d3.v3.min.js").AssemblyWide()
            ]
        ]

[<Sealed>]
type D3Extension() =
    interface IExtension with
        member ext.Assembly = Definition.D3Assembly

[<assembly: Extension(typeof<D3Extension>)>]
[<assembly: System.Reflection.AssemblyVersion("3.0.0.0")>]
do ()
