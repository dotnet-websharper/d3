namespace IntelliFactory.WebSharper.D3

open IntelliFactory.WebSharper.InterfaceGenerator

module Definition =
    let mutable classList = [] : CodeModel.NamespaceEntity list

    let addToClassList c =
        classList <- upcast c :: classList
    let ( |>! ) x f =
        f x
        x

    let Record name propList =
        Class name
        |+> Protocol (
            propList |> List.map (fun (n, t) -> upcast (n =@ t))
        )
        |>! addToClassList

    let O = T<unit>
    let String = T<string>
    let Int = T<int>
    let Float = T<float>
    let Obj = T<obj>
    let Bool = T<bool>
    let Error = T<exn>
    let ( !| ) x = Type.ArrayOf x

    let Int2 = Int * Int
    let Int2x2 = Int2 * Int2
    let Float2 = Float * Float
    let Float3 = Float * Float * Float
    let Float2x2 = Float2 * Float2
    let Comparator = Obj * Obj ^-> Int

    let Date = T<IntelliFactory.WebSharper.EcmaScript.Date>
    let DateTime = T<System.DateTime>

    let Element = T<IntelliFactory.WebSharper.Dom.Element>
    let NodeList = T<IntelliFactory.WebSharper.Dom.NodeList>
    let Event = T<IntelliFactory.WebSharper.Dom.Event>

    let nameArg = String?name

    let WithThis t ps r = (t -* ps ^-> r) + (ps ^-> r)

//    let WithNum n = (n Int) + (n Float)

    let selectionCallback ret = WithThis Element (Obj?d * Int?i) ret

//    let selectionCallback ret = (Element -* Obj?d * Int?i ^-> ret) + (Obj?d * Int?i ^-> ret)

    let getVal t = O ^-> t
//    let setVal chained t = chained t?value
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

    let UpdateSelection = Type.New()
    let Transition      = Type.New()

    let selector = (String + selectionCallback !|Element + selectionCallback NodeList)?selector

    let ChainedClassG name (t: Type.Type) (members : ((Type.IParameters -> Type.Type) -> CodeModel.Member list)) =
        Class name |=> t |+> Protocol (members <| fun args -> args ^-> t)

    let ChainedClass name (t: Type.Type) (members : ((Type.IParameters -> Type.Type) -> CodeModel.Member list)) =
        Class name |=> t |+> Protocol (members <| fun args -> args ^-> t)
        |>! addToClassList

    let ChainedClassNew name members =
        ChainedClass name (Type.New()) members

    let ChainedClassNewInherits name inherits members =
        ChainedClassNew name members |=> Inherits inherits

    let ChainedClassInheritsG name t inherits members =
        ChainedClassG name t members |=> Inherits inherits

    let Selection =
        let self = Type.New()
        ChainedClass "Selection" self <| fun chained ->
        [
            "attr"       => getProp Obj + chained (setPropF Obj) + chained NameValuePair?attrValueMap
            "classed"    => getSetPropF chained Bool
            "style"      => getProp Obj + chained (setPropF Obj * !?String?priority)
            "property"   => getSetPropF chained Obj
            "text"       => getSetValF chained String
            "html"       => getSetValF chained String
            "append"     => chained nameArg
            "insert"     => chained (nameArg * !?(String + selectionCallback Element)?before)
            "remove"     => chained O
            "data"       => ((!|Obj + selectionCallback !|Obj)?values * !?(selectionCallback String)?key ^-> UpdateSelection) + (O ^-> !|Obj)
            "datum"      => chained (Obj + selectionCallback Obj)?value
            "filter"     => chained (String + selectionCallback Bool)?selector
            "sort"       => chained O
            "order"      => chained O
            "on"         => String?``type`` * !?(selectionCallback O)?listener ^-> O
            "transition" => O ^-> Transition
            "interrupt"  => chained O
            "each"       => chained (selectionCallback O)
            "call"       => chained (!+Obj * (self ^-> O)?callback)
            "empty"      => O ^-> Bool
            "node"       => O ^-> Element
            "size"       => O ^-> Int
            "select"     => chained selector
            "selectAll"  => chained selector
        ]

    addToClassList (
        Class "UpdateSelection"
        |=> UpdateSelection
        |=> Inherits Selection
        |+> Protocol [
            "enter" => O ^-> Selection
            "exit"  => O ^-> Selection
        ]
    )

    let tweenCallback   = (Element -* Obj?d * Int?i * Obj?a ^-> (Float?t ^-> Obj))
    let factoryCallback = (Element -* O ^-> (Element -* Float?t ^-> Obj))

    let easing = T<float -> float>

    let TransitionClass =
        ChainedClass "Transition" Transition <| fun chained ->
        [
            "delay"      => chained Int?delay
            "duration"   => chained Int?duration
            "ease"       => chained (!+Float * String?value) + chained easing?value
            "attr"       => chained (nameArg * (Obj + selectionCallback Obj)?value)
            "attrTween"  => chained (nameArg * tweenCallback?value)
            "style"      => chained (nameArg * (Obj + selectionCallback Obj)?value * !? String?priority)
            "styleTween" => chained (nameArg * tweenCallback?value * !? String?priority)
            "text"       => chained (setValF String)
            "tween"      => chained (nameArg * factoryCallback?factory)
            "remove"     => chained O
            "select"     => chained selector
            "selectAll"  => chained selector
            "filter"     => chained (String + selectionCallback Bool)?selector
            "transition" => chained O
            "each"       => chained (!?String?``type`` * selectionCallback O)
            "call"       => chained (!+Obj * (Selection ^-> O)?callback)
            "empty"      => O ^-> Bool
            "node"       => O ^-> Element
            "size"       => O ^-> Int
        ]

    let KeyValuePair =
        Record "KeyValuePair" [
            "key"   , String
            "value" , Obj
        ]

    let Map =
        let self = Type.New()
        Class "Map"
        |=> self
        |+> Protocol [
            "has"     => String?key ^-> Bool
            "get"     => String?key ^-> Float
            "set"     => String?key * Obj?value ^-> O
            "remove"  => String?key ^-> Bool
            "keys"    => O ^-> !|String
            "values"  => O ^-> !|Obj
            "entries" => O ^-> !|KeyValuePair
            "forEach" => WithThis self (String?key * Obj?value) O
        ]

    let Set =
        let self = Type.New()
        Class "Set"
        |=> self
        |+> Protocol [
            "has"     => String?value ^-> Bool
            "add"     => String?value ^-> String
            "remove"  => String?value ^-> Bool
            "values"  => O ^-> !|String
            "forEach" => WithThis self String?value O
        ]

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
        |+> Protocol [
            "apply" => Date?date ^-> String
            "parse" => String?string ^-> Date
        ]
        |>! addToClassList

    let Transform =
        Class "Transform"
        |+> Protocol [
            "rotate"    =@ Float
            "translate" =@ Int2
            "skew"      =@ Float
            "scale"     =@ Float2
            "toString"  => O ^-> String
        ]
        |>! addToClassList

    let Xhr =
        ChainedClassNew "Xhr" <| fun chained ->
        [
            "header"       => getSetProp chained String
            "mimeType"     => getSetVal chained String
            "responseType" => getSetVal chained String
            "response"     => getSetVal chained (String?request ^-> Obj)
            "get"          => !?(Error?error * Obj?response ^-> O)?callback ^-> O
            "get"          => !?String?data * !?(Error?error * Obj?response ^-> O)?callback ^-> O
            "send"         => String?``method`` * !?String?data * !?(Error?error * Obj?response ^-> O)?callback ^-> O
            "abort"        => O ^-> O
            "on"           => String?``type`` ^-> (Error?error * Obj?response ^-> O)
            "on"           => String?``type`` * (Error?error * Obj?response ^-> O)?listener ^-> O
        ]

    let Rgb = Type.New()

    let ColorType name convName convType =
        let chained args = args ^-> Rgb
        Class name
        |+> Protocol (
            name |> Seq.map (fun n ->
                string n =@ Int :> CodeModel.Member
            )
            |> List.ofSeq
        )
        |+> Protocol [
            "brighter" => chained !?Float?k
            "darker"   => chained !?Float?k
            convName   => O ^-> convType
            "toString" => O ^-> String
        ]

    let Hsl = ColorType "hsl" "rgb" Rgb
    let Lab = ColorType "lab" "rgb" Rgb
    let Hcl = ColorType "hcl" "rgb" Rgb
    let RgbClass = ColorType "rgb" "hsl" Hsl |=> Rgb

    let QualifiedNs =
        Record "QualifiedNs" [
            "space" , String
            "local" , String
        ]

    let interpolate argType = argType?a * argType?b ^-> argType

    let Scale = Class "Scale"

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
            "invertExtent" => Float?y ^-> Float2
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
            Generic / fun tData ->
            ChainedClassInheritsG "OrdinalScale" self.[tData] Scale <| fun chained ->
            [
                "apply"           => tData?x ^-> Float |> WithInline "$this($x)"
                "domain"          => getSetVal chained !|tData
                "range"           => getSetVal chained !|Float
                "rangePoints"     => chained (Float2?interval * !?Float?padding)
                "rangeBands"      => chained (Float2?interval * !?Float?padding * !?Float?outerPadding)
                "rangeRoundBands" => chained (Float2?interval * !?Float?padding * !?Float?outerPadding)
                "rangeBand"       => O ^-> Float
                "rangeExtent"     => O ^-> Float2
                "copy"            => chained O
            ]
        addToClassList <| Generic - gen
        gen

    let TimeScale = getQuantScale "TimeScale" Date

    let Interpolation =
        Pattern.EnumStrings "Interpolation" [
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
        |>! addToClassList

    let ChainedClassCoord name members =
        let self = Type.New()
        let gen =
            Generic / fun tData ->
            ChainedClassG name self.[tData] <| fun chained ->
            let coord name = name => getSetVal chained (tData ^-> Float) + chained Float
            members chained coord tData
        addToClassList <| Generic - gen
        gen

    let Line =
        ChainedClassCoord "Line" <| fun chained coord tData ->
        [
            coord "x"
            coord "y"
            "interpolate" => getSetVal chained Interpolation
            "tension" => getSetVal chained String
            "defined" => getSetVal chained (tData ^-> Bool)
        ]

    let RadialLine =
        ChainedClassCoord "RadialLine" <| fun chained coord tData ->
        [
            coord "radius"
            coord "angle"
            "defined" => getSetVal chained (tData ^-> Bool)
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
        ]

    let Arc =
        ChainedClassCoord "Arc" <| fun chained coord tData ->
        [
            coord "innerRadius"
            coord "outerRadius"
            coord "startAngle"
            coord "endAngle"
            "centroid" => Obj?datum * !?Int?index ^-> Float2
        ]

    let SymbolType =
        Pattern.EnumStrings "SymbolType" [
            "circle"
            "cross"
            "diamond"
            "square"
            "triange-down"
            "triange-up"
        ]
        |>! addToClassList

    let Symbol =
        ChainedClassNew "Symbol" <| fun chained ->
        [
            "type" => getSetVal chained SymbolType.Type
            "size" => getSetVal chained Int
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
            "projection" => getSetVal chained (Float2 ^-> Float2)
        ]

    let Orientation =
        Pattern.EnumStrings "Orientation" [
            "top"
            "bottom"
            "left"
            "right"
        ]
        |>! addToClassList

    let Axis =
        ChainedClassNew "Axis" <| fun chained ->
        [
            "apply"  => (Selection + Transition)?selection ^-> O |> WithInline "$this($selection)"
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
        Pattern.EnumStrings "BrushEvent" [
            "brushstart"
            "brush"
            "brushend"
        ]
        |>! addToClassList

    let Brush =
        ChainedClassNew "Brush" <| fun chained ->
        [
            "apply" => (Selection + Transition)?selection ^-> O |> WithInline "$this($selection)"
            "x" => getSetVal chained Scale.Type
            "y" => getSetVal chained Scale.Type
            "extent" => getSetVal chained (Int2 + Int2x2)
            "clamp" => getSetVal chained (Bool + Bool * Bool)
            "clear" => chained O
            "empty" => O ^-> Bool
            "on"    => (BrushEvent?``type`` ^-> (O ^-> O)) + chained (BrushEvent?``type`` * (O ^-> O)?listener)
            "event" => (Selection + Transition)?selection ^-> O
        ]

    let TimeInterval =
        ChainedClassNew "TimeInterval" <| fun chained ->
        [
        ]

    let Link =
        Generic / fun t ->
            Class "Link"
            |+> Protocol [
                "source" =@ t
                "target" =@ t
            ]
    addToClassList <| Generic - Link

    let BundleNode =
        let self = Type.New()
        Record "BundleNode" [
            "parent" , self
        ]

    let Bundle =
        ChainedClassNew "Bundle" <| fun chained ->
        [
            "links"      => (!|BundleNode)?nodes ^-> !|(Link BundleNode)
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
            "chords" => O ^-> !|(Link ChordNode)
            "groups" => O ^-> !|(Link ChordNode)
        ]

    let ClusterNode =
        let self = Type.New()
        Record "ClusterNode" [
            "parent"   , self
            "children" , !|self
            "depth"    , Int
            "x"        , Int
            "y"        , Int
        ]
        |=> self

    let Cluster =
        ChainedClassNew "Cluster" <| fun chained ->
        [
            "nodes"      => ClusterNode?root ^-> !|ClusterNode
            "links"      => (!|ClusterNode)?nodes ^-> !|(Link ClusterNode)
            "children"   => getSetVal chained (Obj ^-> Obj)
            "sort"       => getSetVal chained Comparator
            "separation" => getSetVal chained (ClusterNode * ClusterNode ^-> Int)
            "size"       => getSetVal chained Float2
            "nodeSize"   => getSetVal chained Float2
            "value"      => getSetVal chained (Obj ^-> Float)
        ]

    let ForceNode =
        Record "ForceNode" [
            "index"  , Int
            "x"      , Int
            "y"      , Int
            "px"     , Int
            "py"     , Int
            "fixed"  , Bool
            "weight" , Int
        ]

    let ForceEvent =
        Pattern.EnumStrings "ForceEvent" [
            "start"
            "tick"
            "end"
        ]
        |>! addToClassList

    let Force =
        ChainedClassNew "Force" <| fun chained ->
        [
            "on" => (ForceEvent?``type`` ^-> (O ^-> O)) + chained (ForceEvent?``type`` * (O ^-> O)?listener)
            "nodes" => ForceNode?root ^-> !|ForceNode
            "links" => getSetVal chained !|(Link ForceNode)
            "start" => O ^-> O
            "alpha" => getSetVal chained Float
            "resume" => O ^-> O
            "start" => O ^-> O
            "tick"  => O ^-> O
            "drag"  => O ^-> O
        ]

    let HierarchyNode =
        let self = Type.New()
        Record "HierarchyNode" [
            "parent"   , self
            "children" , !|self
            "value"    , Obj
            "depth"    , Int
        ]
        |=> self

    let Hierarchy =
        ChainedClassNew "Hierarchy" <| fun chained ->
        [
            "nodes"    => HierarchyNode?root ^-> !|HierarchyNode
            "links"    => getSetVal chained !|(Link HierarchyNode)
            "children" => getSetVal chained (Obj ^-> Obj)
            "sort"     => getSetVal chained Comparator
            "value"    => getSetVal chained (Obj ^-> Float)
            "revalue"  => chained HierarchyNode?root
        ]

    let binningFunc = Float2 * !|Obj * Int ^-> !|Float

    let Histogram =
        ChainedClassNew "Histogram" <| fun chained ->
        [
            "value"      => getSetVal chained (Obj ^-> Float)
            "range"      => getVal Float2 + chained (Float2 + !|Obj * Int ^-> Float2)
            "bins"       => getVal binningFunc + chained(Int + !|Float + binningFunc)
            "frequency"  => getSetVal chained Bool
        ]

    let PackNode =
        let self = Type.New()
        Record "PackNode" [
            "parent"   , self
            "children" , !|self
            "value"    , Obj
            "depth"    , Int
            "x"        , Int
            "y"        , Int
            "r"        , Int
        ]
        |=> self

    let Pack =
        ChainedClassNew "Pack" <| fun chained ->
        [
            "nodes"    => PackNode?root ^-> !|PackNode
            "links"    => getSetVal chained !|(Link PackNode)
            "children" => getSetVal chained (Obj ^-> Obj)
            "sort"     => getSetVal chained Comparator
            "value"    => getSetVal chained (Obj ^-> Float)
            "size"     => getSetVal chained Int2
            "radius"   => getSetVal chained Int
            "padding"  => getSetVal chained Int
        ]

    let PartitionNode =
        let self = Type.New()
        Record "PartitionNode" [
            "parent"   , self
            "children" , !|self
            "value"    , Obj
            "depth"    , Int
            "x"        , Int
            "y"        , Int
            "dx"       , Int
            "dy"       , Int
        ]
        |=> self

    let Partition =
        ChainedClassNew "Partition" <| fun chained ->
        [
            "nodes"    => PartitionNode?root ^-> !|PartitionNode
            "links"    => getSetVal chained !|(Link PartitionNode)
            "children" => getSetVal chained (Obj ^-> Obj)
            "sort"     => getSetVal chained Comparator
            "value"    => getSetVal chained (Obj ^-> Float)
            "size"     => getSetVal chained Int2
        ]

    let Pie =
        ChainedClassNew "Pie" <| fun chained ->
        [
            "value"    => getSetVal chained (Obj ^-> Float)
            "sort"     => getSetVal chained Comparator
            "startAngle" => getVal Float + chained (Float + Obj * Int ^-> Float)
            "endAngle" => getVal Float + chained (Float + Obj * Int ^-> Float)
        ]

    let StackOffset =
        Pattern.EnumStrings "StackOffset" [
            "silhouette"
            "wiggle"
            "expand"
            "zero"
        ]
        |>! addToClassList

    let StackOrder =
        Pattern.EnumStrings "StackOrder" [
            "inside-out"
            "reverse"
            "default"
        ]
        |>! addToClassList

    let Stack =
        ChainedClassNew "Stack" <| fun chained ->
        [
            "values" => getSetVal chained (Obj ^-> Obj)
            "offset" => getVal (!|Float2 ^-> !|Float) + chained (StackOffset + (Float2 ^-> !|Float))
            "order"  => getVal (!|Float2 ^-> !|Int) + chained (StackOrder + (!|Float2 ^-> !|Int))
            "x"      => getSetVal chained (Obj ^-> Float)
            "y"      => getSetVal chained (Obj ^-> Float)
            "out"    => getSetVal chained (Obj?d * Float?y0 * Float?y ^-> O)
        ]

    let TreeNode =
        let self = Type.New()
        Record "TreeNode" [
            "parent"   , self
            "children" , !|self
            "depth"    , Int
            "x"        , Int
            "y"        , Int
        ]
        |=> self

    let Tree =
        ChainedClassNew "Tree" <| fun chained ->
        [
            "nodes"      => TreeNode?root ^-> !|TreeNode
            "links"      => (!|TreeNode)?nodes ^-> !|(Link TreeNode)
            "children"   => getSetVal chained (Obj ^-> Obj)
            "sort"       => getSetVal chained Comparator
            "separation" => getSetVal chained (TreeNode * TreeNode ^-> Int)
            "size"       => getSetVal chained Float2
            "nodeSize"   => getSetVal chained Float2
        ]

    let TreemapMode =
        Pattern.EnumStrings "TreemapMode" [
            "squarify"
            "slice"
            "dice"
            "slice-dice"
        ]
        |>! addToClassList

    let Treemap =
        ChainedClassNew "Treemap" <| fun chained ->
        [
            "nodes"      => PartitionNode?root ^-> !|PartitionNode
            "links"      => (!|PartitionNode)?nodes ^-> !|(Link PartitionNode)
            "children"   => getSetVal chained (Obj ^-> Obj)
            "sort"       => getSetVal chained Comparator
            "value"    => getSetVal chained (Obj ^-> Float)
            "size"       => getSetVal chained Float2
            "padding"  => getSetVal chained Int
            "round"   => getSetVal chained Bool
            "sticky"  => getSetVal chained Bool
            "mode"    => getSetVal chained TreemapMode
        ]

    let PathContext =
        Interface "PathContext"
        |+> [
            "beginPath" => O ^-> O
            "moveTo"    => Float?x * Float?y ^-> O
            "lineTo"    => Float?x * Float?y ^-> O
            "arc"       => Float?x * Float?y * Float?radius * Float?startAngle * Float?endAngle ^-> O
            "closePath" => O ^-> O
        ]

    let Feature = Type.New()

    let Path =
        ChainedClassNew "Path" <| fun chained ->
        [
            "projection"  => getSetVal chained (Float2 ^-> Float2)
            "context"     => getSetVal chained PathContext.Type
            "pointRadius" => getSetVal chained Float
            "area"        => Feature ^-> Int
            "centroid"    => Feature ^-> Int2
            "bounds"      => Feature ^-> Int2x2
        ]

    let MultiLineString = Type.New()
    let LineString = Type.New()
    let Polygon = Type.New()

    let Graticule =
        ChainedClassNew "Graticule" <| fun chained ->
        [
            "lines" => O ^-> !|LineString
            "outline" => O ^-> Polygon
            "extent" => getSetVal chained Float2x2
            "majorExtent" => getSetVal chained Float2x2
            "minorExtent" => getSetVal chained Float2x2
            "step" => getSetVal chained Float2
            "majorStep" => getSetVal chained Float2
            "minorStep" => getSetVal chained Float2
            "precision" => getSetVal chained Float
        ]

    let Circle =
        ChainedClassNew "Circle" <| fun chained ->
        [
            "origin"    => getVal Float2 + chained (Float2 + Obj ^-> Float2)
            "angle"     => getSetVal chained Float
            "precision" => getSetVal chained Float
        ]

    let Rotation =
        ChainedClassNew "Rotation" <| fun chained ->
        [
            "apply"   => Float2?location ^-> Float2 |> WithInline "$this($location)"
            "invvert" => Float2?location ^-> Float2
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
            "apply"      => Float2?location ^-> Float2 |> WithInline "$this($location)"
            "invert"     => Float2?location ^-> Float2
            "rotate"     => getSetVal chained Float3
            "center"     => getSetVal chained Float2
            "translate"  => getSetVal chained Float2
            "scale"      => getSetVal chained Float
            "clipAngle"  => getSetVal chained Float
            "clipExtent" => getSetVal chained Float2x2
            "precision"  => getSetVal chained Int
            "stream"     => Listener?listener ^-> Listener
        ]

    let AlbersProjection =
        ChainedClassNewInherits "AlbersProjection" Projection <| fun chained ->
        [
            "parallels" => getVal Float2 + chained Float2
        ]

    let StreamTransform =
        ChainedClassNew "StreamTransform" <| fun chained ->
        [
            "stream" => Listener?listener ^-> Listener
        ]

    let ClipTransform =
        ChainedClassNewInherits "ClipTransform" StreamTransform <| fun chained ->
        [
            "extent" => getSetVal chained Float2x2
        ]

    let Voronoi =
        ChainedClassNew "Voronoi" <| fun chained ->
        [
            "x"     => getSetVal chained (Obj ^-> Float)
            "y"     => getSetVal chained (Obj ^-> Float)
            "clipExtent" => getSetVal chained Float2x2
            "links" => !|Obj ^-> !|Obj
            "data" => !|Obj ^-> !|Obj
        ]

    let QuadtreeNode =
        let self = Type.New()
        Class "QuadtreeNode"
        |=> self
        |+> Protocol [
            "nodes" =? self * self * self * self
            "leaf"  =? Bool
            "point" =? Float2
            "x"     =? Float
            "y"     =? Float
        ]

    let QuadtreeRoot =
        ChainedClassNewInherits "QuadtreeRoot" QuadtreeNode <| fun chained ->
        [
            "add"   => Float2 ^-> O
            "visit" => (QuadtreeNode?node * Float?x1 * Float?y1 * Float?x2 * Float?y2 ^-> Bool)?callback ^-> O
        ]

    let Quadtree =
        ChainedClassNew "Quadtree" <| fun chained ->
        [
            "apply"  => (!|Float2)?points ^-> QuadtreeRoot |> WithInline "$this($points)"
            "x"     => getSetVal chained (Obj ^-> Float)
            "y"     => getSetVal chained (Obj ^-> Float)
            "extent" => getSetVal chained Float2x2
        ]

    let PolygonClass =
        ChainedClassNew "Polygon" (fun chained ->
            [
                "area"     => O ^-> Float
                "centroid" => O ^-> Float2
                "clip"     => chained Polygon
            ]
        )
        |=> Polygon

    let Hull =
        ChainedClassNew "Hull" <| fun chained ->
        [
            "apply" => !|Obj ^-> !|Obj
            "x"     => getSetVal chained (Obj ^-> Float)
            "y"     => getSetVal chained (Obj ^-> Float)
        ]

    let DragEvent =
        Pattern.EnumStrings "DragEvent" [
            "dragstart"
            "drag"
            "dragend"
        ]
        |>! addToClassList

    let Drag =
        ChainedClassNew "Drag" <| fun chained ->
        [
            "origin" => getSetVal chained (selectionCallback Float2)
            "on"     => (DragEvent?``type`` ^-> (O ^-> O)) + chained (DragEvent?``type`` * (O ^-> O)?listener)
        ]

    let ZoomEvent =
        Pattern.EnumStrings "ZoomType" [
            "zoomstart"
            "zoom"
            "zoomend"
        ]
        |>! addToClassList

    let Zoom =
        ChainedClassNew "Zoom" <| fun chained ->
        [
            "scale"       => getSetVal chained Float
            "translate"   => getSetVal chained Float2
            "scaleExtent" => getSetVal chained Float2
            "center"      => getSetVal chained Float2
            "size"        => getSetVal chained Float2
            "x"           => getSetVal chained Scale
            "y"           => getSetVal chained Scale
            "on"          => (ZoomEvent?``type`` ^-> (O ^-> O)) + chained (ZoomEvent?``type`` * (O ^-> O)?listener)
            "event"       => (Selection + Transition)?selection ^-> O
        ]

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
        |+> [
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

    let interpolateTo argType resType = argType?a * argType?b ^-> resType

    let xhrMime xhrType = String?url * !?String?mimeType * (Error?error * xhrType?response ^-> O)?callback ^-> O
    let xhr xhrType = String?url * (xhrType?response ^-> O)?callback ^-> O

    let D3 =
        Class "d3"
        |+> [
            // Selections
            "select"      => selector ^-> Selection
            "selectAll"   => selector ^-> Selection
            "selection"   => O ^-> Selection
            "event"       =? Event
            "mouse"       => Element?container ^-> Int2
            "touches"     => Element?container * !?Obj?touches ^-> !|Int2x2

            // Transitions
            "transition"  => !?Selection?selection ^-> Transition
            "ease"        => (String?``type`` *+ Float) ^-> easing
            "timer"       => O ^-> Bool?``function`` * !?Int?delay * !?T<System.DateTime>?time ^-> O
            Generic - fun t -> "interpolate" => interpolate t
            "interpolateNumber" => interpolate Float
            "interpolateRound"  => interpolateTo Float Int
            "interpolateString" => interpolate String
            "interpolateRgb"    => interpolateTo (Rgb + String) String
            "interpolateHsl"    => interpolateTo (Hsl + String) String
            "interpolateLab"    => interpolateTo (Lab + String) String
            "interpolateHcl"    => interpolateTo (Hcl + String) String
            Generic - fun t -> "interpolateArray" => interpolate !|T
            Generic - fun t -> "interpolateObject" => interpolate t
            "interpolateTransform" => interpolate Transform.Type
            "interpolateZoom" => interpolate Float3
            //geo.Interpolate
            "interpolators" =? T<(obj * obj -> obj)[]>

            // Working with Arrays
            "ascending"   => Float?a * Float?b ^-> Int
            "descending"  => Float?a * Float?b ^-> Int
            Generic - fun t   -> "min" => (!|t)?array ^-> t
            Generic - fun t u -> "min" => (!|t)?array * (t ^-> u)?accessor ^-> u
            Generic - fun t   -> "max" => (!|t)?array ^-> t
            Generic - fun t u -> "max" => (!|t)?array * (t ^-> u)?accessor ^-> u
            Generic - fun t   -> "extent" => (!|t)?array ^-> t * t
            Generic - fun t u -> "extent" => (!|t)?array * (t ^-> u)?accessor ^-> u * u
            Generic - fun t   -> "mean" => (!|t)?array ^-> t
            Generic - fun t u -> "mean" => (!|t)?array * (t ^-> u)?accessor ^-> u
            Generic - fun t   -> "median" => (!|t)?array ^-> t
            Generic - fun t u -> "median" => (!|t)?array * (t ^-> u)?accessor ^-> u
            Generic - fun t   -> "quantile" => (!|t)?numbers * Float?p ^-> t
            Generic - fun t   -> "bisectLeft" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int
            Generic - fun t   -> "bisect" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int
            Generic - fun t   -> "bisectRight" => (!|t)?array * Int?x * !?Int?lo * !?Int?hi ^-> Int
            Generic - fun t u -> "bisector" => (t ^-> u) ^-> Bisector
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

            "xhr" => String?url * !?String?mimeType ^-> Xhr
            "xhr" => xhrMime Xhr.Type

            "text" => xhrMime String
            "json" => xhr Obj
            "html" => xhr T<IntelliFactory.WebSharper.Dom.Document>
            "xml"  => xhrMime T<IntelliFactory.WebSharper.Dom.Document>
            "csv"  => xhr (!| !|Obj)
            "tsv"  => xhr (!| !|Obj)
            "dsv"  => xhr (!| !|Obj)

            "format" => String?specifier ^-> Float?number ^-> String
            "fomatPefix" => Float?value * !?Float?precision ^-> Prefix
            "requote" => String ^-> String
            "round" => Float?x * Int?n ^-> Float

            Generic - fun t -> "functor" => t ^-> O ^-> t
            "rebind" => Obj?target * Obj?source *+ String ^-> O
            "dispatch" => !+ String ^-> Dispatcher
        ]
        |=> Nested [
            Class "d3.timer"
            |+> [
                "flush" => O ^-> O
            ]
            Class "d3.random"
            |+> [
                "normal"    => !?Float?mean * !?Float?deviation ^-> Float
                "logNormal" => !?Float?mean * !?Float?deviation ^-> Float
                "irwinHall" => Int?count ^-> Float
            ]
            Class "d3.ns"
            |+> [
                "prefix"  =? Map
                "qualify" => nameArg ^-> QualifiedNs
            ]
            Class "d3.time"
            |+> [
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
                |+> [
                    "utc" => String?specifier ^-> TimeFormat
                    "iso" =? TimeFormat
                ]
            ]
            Class "d3.layout"
            |+> [
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
            |+> [
                "linear"    => O ^-> QuantitativeScale
                "sqrt"      => O ^-> QuantitativeScale
                "pow"       => O ^-> QuantitativeScale
                "log"       => O ^-> QuantitativeScale
                "quantize"  => O ^-> DiscreteScale
                "threshold" => O ^-> DiscreteScale
                "quantile"  => O ^-> QuantileScale
                "identity"  => O ^-> IdentityScale
                Generic - fun tData -> "ordinal"   => O ^-> OrdinalScale tData
            ]
            Class "d3.svg"
            |+> [
                Generic - fun tData -> "line"        => O ^-> Line       tData
                Generic - fun tData -> "line.radial" => O ^-> RadialLine tData
                Generic - fun tData -> "area"        => O ^-> Area       tData
                Generic - fun tData -> "area.radial" => O ^-> RadialArea tData
                Generic - fun tData -> "arc"         => O ^-> Arc        tData
                "symbol"          => O ^-> Symbol
                "symbolTypes"     =? !|SymbolType
                Generic - fun tData -> "chord"       => O ^-> Chord      tData
                "diagonal"        => O ^-> Diagonal
                "diagonal.radial" => O ^-> Diagonal
                "axis"            => O ^-> Axis
                "brush"           => O ^-> Brush
            ]
            Class "d3.geo"
            |+> [
                "path"      => O ^-> Path
                "graticule" => O ^-> Graticule
                "circle"    => O ^-> Circle
                "area"      => Feature ^-> Float
                "centroid"  => Feature ^-> Float2
                "bounds"    => Feature ^-> Float2x2
                "distance"  => Float2?a * Float2?b ^-> Float
                "rotation"  => (!|Float)?rotate ^-> Rotation
                "projection" => (Float2 ^-> Float2)?raw ^-> Projection
                "projectionMutator" => (Float2 ^-> Float2 ^-> Float2)?rawFactory ^-> Projection

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

                "albersUsa.raw"            => Float2 ^-> Projection
                "azimuthalEqualArea.raw"   => O ^-> Projection
                "azimuthalEquidistant.raw" => O ^-> Projection
                "conicConformal.raw"       => Float2 ^-> Projection
                "conicEquidistant.raw"     => Float2 ^-> Projection
                "conicEquidistant.raw"     => Float2 ^-> Projection
                "equirectangular.raw"      => O ^-> Projection
                "gnomonic.raw"             => O ^-> Projection
                "mercator.raw"             => O ^-> Projection
                "orthographic.raw"         => O ^-> Projection
                "stereographic.raw"        => O ^-> Projection
            ]
            Class "d3.geom"
            |+> [
                "voronoi"  => O ^-> Voronoi
                "quadtree" => O ^-> Quadtree
                "polygon"  => (!|Float2)?vertices ^-> Polygon
                "hull"     => O ^-> Hull
            ]
            Class "d3.behavior"
            |+> [
                "drag" => O ^-> Drag
                "zoom" => O ^-> Zoom
            ]
        ]
        |>! addToClassList

    let D3Assembly =
        Assembly [
            Namespace "IntelliFactory.WebSharper.D3" classList
            Namespace "IntelliFactory.WebSharper.D3.Resources" [
                (Resource "D3" "d3.v3.min.js").AssemblyWide()
            ]
        ]

[<Sealed>]
type D3Extension() =
    interface IExtension with
        member ext.Assembly = Definition.D3Assembly

[<assembly: Extension(typeof<D3Extension>)>]
do ()
