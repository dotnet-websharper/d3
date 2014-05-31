# WebSharper.D3

Provides [WebSharper](http://websharper.com) bindings to [D3](http://d3js.org), the JavaScript library for Data-Driven Documents.

|          |                             |
| -------- | --------------------------- |
| Library  | [D3js.org](http://d3js.org) |
| Version  | 20130910 (version 3.3.6) |
| Learn    | [WebSharper.D3 Examples](http://intellifactory.github.io/websharper.d3) |
| CI       | [![Build status](https://ci.appveyor.com/api/projects/status/j3imr7tbvfrpu08f)](https://ci.appveyor.com/project/t0yv0/websharper-d3)  |
| NuGet    | [WebSharper.D3](http://http://www.nuget.org/packages/WebSharper.D3/) |

The following is the introduction from [d3js.org][d3] with samples 
translated to F#, and some comments on typing.

# Introduction

D3 allows you to bind arbitrary data to a Document Object Model (DOM), and then apply data-driven transformations to the document. For example, you can use D3 to generate an HTML table from an array of numbers. Or, use the same data to create an interactive SVG bar chart with smooth transitions and interaction.

D3 is not a monolithic framework that seeks to provide every conceivable feature. Instead, D3 solves the crux of the problem: efficient manipulation of documents based on data. This avoids proprietary representation and affords extraordinary flexibility, exposing the full capabilities of web standards such as CSS3, HTML5 and SVG. With minimal overhead, D3 is extremely fast, supporting large datasets and dynamic behaviors for interaction and animation. D3's functional style allows code reuse through a diverse collection of components and plugins.

# Selections

For modifying DOM elements, D3 employs a declarative approach, operating on arbitrary sets of nodes called selections. For example, you can change the text color of paragraph elements:

```
D3.SelectAll("p").Style("color", "white") |> ignore
```
Yet, you can still manipulate individual nodes as needed:

```
D3.Select("body").Style("background-color", "black") |> ignore
```
Selectors are defined by the [W3C Selectors API][w3cselectorsapi] and supported natively by modern browsers. Backwards-compatibility for older browsers can be provided by [Sizzle][sizzle]. The above examples select nodes by tag name (`p` and `body`, respectively). Elements may be selected using a variety of predicates, including containment, attribute values, class and ID.

D3 provides numerous methods for mutating nodes: setting attributes or styles; registering event listeners; adding, removing or sorting nodes; and changing HTML or text content. These suffice for the vast majority of needs. Direct access to the underlying DOM is also possible, as each D3 selection is simply an array of nodes.

# Dynamic Properties

Readers familiar with other DOM frameworks such as [jQuery][jquery] or [Prototype][prototypejs] should immediately recognize similarities with D3. Yet styles, attributes, and other properties can be specified as functions of data in D3, not just simple constants. Despite their apparent simplicity, these functions can be surprisingly powerful; the `d3.geo.path` function, for example, projects [geographic coordinates][geocoordinates] into [SVG path data][pathdata]. D3 provides many built-in reusable functions and function factories, such as graphical primitives for area, line and pie charts.

For example, to randomly color paragraphs:

```
D3.SelectAll("p").Style("color", fun _ ->
  "hsl(" + string (EcmaScript.Math.random() * 360.) + ",100%,50%)"
) |> ignore
```

To alternate shades of gray for even and odd nodes:

```
D3.SelectAll("p").Style("color", fun (_, i) ->
  if i % 2 = 0 then "#eee" else "#fff"
) |> ignore
```

Computed properties often refer to bound data. Data is specified as an array of values, and each value is passed as the first argument (`d`) to selection functions. With the default join-by-index, the first element in the data array is passed to the first node in the selection, the second element to the second node, and so on. For example, if you bind an array of numbers to paragraph elements, you can use these numbers to compute dynamic font sizes:

```
D3.SelectAll("p")
    .Data([|4; 8; 15; 16; 23; 42|])
    .Style("font-size", fun (d, _) -> string d + "px") |> ignore
```
Once the data has been bound to the document, you can omit the `data` operator; D3 will retrieve the previously-bound data. This allows you to recompute properties without rebinding.

# Enter and Exit

Using D3's "enter" and "exit" selections, you can create new nodes for incoming data and remove outgoing nodes that are no longer needed.

When data is bound to a selection, each element in the data array is paired with the corresponding node in the selection. If there are fewer nodes than data, the extra data elements form the enter selection, which you can instantiate by appending to the `Enter` selection. For example:

```
D3.Select("body").SelectAll("p")
  .Data([|4; 8; 15; 16; 23; 42|])
  .Enter().Append("p")
  .Text(fun d -> "I'm number " + string d + "!") |> ignore
```
Updating nodes are the default selection—the result of the `Data` operator. Thus, if you forget about the enter and exit selections, you will automatically select only the elements for which there exists corresponding data. A common pattern is to break the initial selection into three parts: the updating nodes to modify, the entering nodes to add, and the exiting nodes to remove.

```
// Update...
let p =
    D3.Select("body").SelectAll("p")
      .Data([|4; 8; 15; 16; 23; 42|])
      .Text("updated") |> ignore

// Enter...
p.Enter().Append("p")
    .Text("new") |> ignore

// Exit...
p.Exit().Remove() |> ignore
```
By handling these three cases separately, you specify precisely which operations run on which nodes. This improves performance and offers greater control over transitions. For example, with a bar chart you might initialize entering bars using the old scale, and then transition entering bars to the new scale along with the updating and exiting bars.

D3 lets you transform documents based on data; this includes both creating and destroying elements. D3 allows you to change an existing document in response to user interaction, animation over time, or even asynchronous notification from a third-party. A hybrid approach is even possible, where the document is initially rendered on the server, and updated on the client via D3.

# Transformation, not Representation

D3 is not a new graphical representation. Unlike [Processing][processing], [Raphaël][raphael], or [Protovis][protovis], the vocabulary of marks comes directly from web standards: HTML, SVG and CSS. For example, you can create SVG elements using D3 and style them with external stylesheets. You can use composite filter effects, dashed strokes and clipping. If browser vendors introduce new features tomorrow, you'll be able to use them immediately—no toolkit update required. And, if you decide in the future to use a toolkit other than D3, you can take your knowledge of standards with you!

Best of all, D3 is easy to debug using the browser's built-in element inspector: the nodes that you manipulate with D3 are exactly those that the browser understands natively.

# Transitions

D3's focus on transformation extends naturally to animated transitions. Transitions gradually interpolate styles and attributes over time. Tweening can be controlled via easing functions such as "elastic", "cubic-in-out" and "linear". D3's interpolators support both primitives, such as numbers and numbers embedded within strings (font sizes, path data, etc.), and compound values. You can even extend D3's interpolator registry to support complex properties and data structures.

For example, to fade the background of the page to black:

```
D3.Select("body").Transition()
    .Style("background-color", "black") |> ignore
```
Or, to resize circles in a symbol map with a staggered delay:

```
d3.SelectAll("circle").Transition()
    .Duration(750)
    .Delay(fun (_, i) -> i * 10)
    .Attr("r", fun (d, _) -> sqrt(d * scale)) |> ignore
```
By modifying only the attributes that actually change, D3 reduces overhead and allows greater graphical complexity at high frame rates. D3 also allows sequencing of complex transitions via events. And, you can still use CSS3 transitions; D3 does not replace the browser’s toolbox, but exposes it in a way that is easier to use.


[d3]: http://d3js.org
[d3api]: https://github.com/mbostock/d3/wiki/API-Reference
[issues]: http://github.com/intellifactory/websharper.d3/issues
[license]: http://github.com/intellifactory/websharper.d3/blob/master/LICENSE.md
[ws]: http://github.com/intellifactory/websharper
[w3cselectorsapi]: http://www.w3.org/TR/selectors-api
[sizzle]: http://sizzlejs.com
[jquery]: http://jquery.com
[prototypejs]: http://www.prototypejs.org
[geocoordinates]: http://geojson.org
[pathdata]: http://www.w3.org/TR/SVG/paths.html#PathData
[processing]: http://processing.org
[raphael]: http://raphaeljs.com
[protovis]: http://vis.stanford.edu/protovis
