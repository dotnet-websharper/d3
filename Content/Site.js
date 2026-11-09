import Runtime from "./WebSharper.Core.JavaScript/Runtime.js"
Runtime.ScriptBasePath="/Scripts/";
import { CreateFuncWithArgs, Create as Create_1, Lazy, SetOptional, GetOptional } from "./WebSharper.Core.JavaScript/Runtime.js"
import * as _1 from "d3"
function isIDisposable(x){
  return"Dispose"in x;
}
function Main(){
  All().Show();
}
function All(){
  return _c.All;
}
class Set {
  Show(){
    const samples=this.$0;
    const select=(s, dom) => {
      globalThis.document.querySelectorAll("#sample-navs ul li").forEach(CreateFuncWithArgs((_2) => _2[0].classList.remove("active")), null);
      dom.classList.add("active");
      return s.Show();
    };
    const navs=Doc.Element("ul", [Class("sample-tabs")], [Doc.Concat(mapi((_2, _3) => Doc.Element("li", [Attr.A4((self_1) => {
      if(_2===0)select(_3, self_1);
    }), Attr.HandlerImpl("click", (self_1) =>() => select(_3, self_1))], [Doc.Element("a", [Attr.Create("href", "#")], [Doc.TextNode(_3.Title)])]), samples))]);
    LoadLocalTemplates("");
    Doc.RunAppendById("sample-navs", navs);
  }
  static Singleton(s){
    return Set.Set(ofArray([s]));
  }
  static Create(ss){
    return Set.Set(ofSeq(delay(() => collect((m) => m.$0, ss))));
  }
  static Set(Item){
    return Create_1(Set, {$:0, $0:Item});
  }
  $;
  $0;
}
function get(arr, n){
  checkBounds(arr, n);
  return arr[n];
}
function length(arr){
  return arr.dims===2?arr.length*arr.length:arr.length;
}
function checkBounds(arr, n){
  if(n<0||n>=arr.length)FailWith("Index was outside the bounds of the array.");
}
function set(arr, n, x){
  checkBounds(arr, n);
  arr[n]=x;
}
function Equals(a, b){
  let _2;
  if(a===b)return true;
  else {
    const m=typeof a;
    if(m=="object"){
      if(a===null||a===void 0||b===null||b===void 0||!Equals(typeof b, "object"))return false;
      else if("Equals"in a)return a.Equals(b);
      else if("Equals"in b)return false;
      else if(a instanceof Array&&b instanceof Array)return arrayEquals(a, b);
      else if(a instanceof Date&&b instanceof Date)return dateEquals(a, b);
      else {
        const a_1=a;
        const b_1=b;
        const eqR=[true];
        let k;
        for(var k_2 in a_1)if(((k_3) => {
          eqR[0]=!a_1.hasOwnProperty(k_3)||b_1.hasOwnProperty(k_3)&&Equals(a_1[k_3], b_1[k_3]);
          return!eqR[0];
        })(k_2))break;
        if(eqR[0]){
          let k_1;
          for(var k_3 in b_1)if(((k_4) => {
            eqR[0]=!b_1.hasOwnProperty(k_4)||a_1.hasOwnProperty(k_4);
            return!eqR[0];
          })(k_3))break;
          _2=void 0;
        }
        else _2=null;
        return eqR[0];
      }
    }
    else return m=="function"&&("$Func"in a?a.$Func===b.$Func&&a.$Target===b.$Target:"$Invokes"in a&&"$Invokes"in b&&arrayEquals(a.$Invokes, b.$Invokes));
  }
}
function arrayEquals(a, b){
  let eq, i;
  if(length(a)===length(b)){
    eq=true;
    i=0;
    while(eq&&i<length(a))
      {
        !Equals(get(a, i), get(b, i))?eq=false:void 0;
        i=i+1;
      }
    return eq;
  }
  else return false;
}
function dateEquals(a, b){
  return a.getTime()===b.getTime();
}
function Hash(o){
  const m=typeof o;
  return m=="function"?0:m=="boolean"?o?1:0:m=="number"?o:m=="string"?hashString(o):m=="object"?o==null?0:o instanceof Array?hashArray(o):hashObject(o):m=="bigint"?hashString(String(o)):m=="symbol"?hashString(o.description):0;
}
function hashString(s){
  let hash;
  if(s===null)return 0;
  else {
    hash=5381;
    for(let i=0, _2=s.length-1;i<=_2;i++)hash=hashMix(hash, s[i].charCodeAt());
    return hash;
  }
}
function hashArray(o){
  let h;
  h=-34948909;
  for(let i=0, _2=length(o)-1;i<=_2;i++)h=hashMix(h, Hash(get(o, i)));
  return h;
}
function hashObject(o){
  if("GetHashCode"in o)return o.GetHashCode();
  else {
    const ____=hashMix;
    const h=[0];
    let k;
    for(var k_1 in o)if(((key) => {
      h[0]=____(____(h[0], hashString(key)), Hash(o[key]));
      return false;
    })(k_1))break;
    return h[0];
  }
}
function Compare(a, b){
  if(a===b)return 0;
  else {
    const m=typeof a;
    switch(m=="boolean"?1:m=="number"?1:m=="bigint"?1:m=="string"?1:m=="object"?2:m=="function"?3:m=="symbol"?4:0){
      case 0:
        return typeof b=="undefined"?0:-1;
      case 1:
        return a<b?-1:1;
      case 2:
        let _2;
        if(a===null)return -1;
        else if(b===null)return 1;
        else if("CompareTo"in a)return a.CompareTo(b);
        else if("CompareTo0"in a)return a.CompareTo0(b);
        else if(a instanceof Array&&b instanceof Array)return compareArrays(a, b);
        else if(a instanceof Date&&b instanceof Date)return compareDates(a, b);
        else {
          const a_1=a;
          const b_1=b;
          const cmp=[0];
          let k;
          for(var k_2 in a_1)if(((k_3) =>!a_1.hasOwnProperty(k_3)?false:!b_1.hasOwnProperty(k_3)?(cmp[0]=1,true):(cmp[0]=Compare(a_1[k_3], b_1[k_3]),cmp[0]!==0))(k_2))break;
          if(cmp[0]===0){
            let k_1;
            for(var k_3 in b_1)if(((k_4) =>!b_1.hasOwnProperty(k_4)?false:!a_1.hasOwnProperty(k_4)&&(cmp[0]=-1,true))(k_3))break;
            _2=void 0;
          }
          else _2=null;
          return cmp[0];
        }
        break;
      case 3:
        return FailWith("Cannot compare function values.");
      case 4:
        return FailWith("Cannot compare symbol values.");
    }
  }
}
function hashMix(x, y){
  return(x<<5)+x+y;
}
function compareArrays(a, b){
  let cmp, i;
  if(length(a)<length(b))return -1;
  else if(length(a)>length(b))return 1;
  else {
    cmp=0;
    i=0;
    while(cmp===0&&i<length(a))
      {
        cmp=Compare(get(a, i), get(b, i));
        i=i+1;
      }
    return cmp;
  }
}
function compareDates(a, b){
  return Compare(a.getTime(), b.getTime());
}
class Sample {
  FileName;
  Id;
  Keywords;
  Render;
  Title;
  Show(){
    const sMain=globalThis.document.getElementById("sample-main");
    const sSide=globalThis.document.getElementById("sample-side");
    Clear(sMain);
    Clear(sSide);
    this.Render(sMain);
    const _2=Doc.Element("div", [], [Doc.Element("div", [Attr.A4((self_1) => {
      const m=globalThis.document.getElementById(this.Id);
      if(Equals(m, null))void 0;
      else {
        const copy=m.cloneNode(true);
        copy.attributes.removeNamedItem("id");
        self_1.appendChild(copy);
      }
    })], []), Doc.Element("a", [Attr.Create("class", "source-link"), Attr.Create("href", "https://github.com/dotnet-websharper/d3/blob/main/Site/"+this.FileName)], [Doc.TextNode("View source")])]);
    LoadLocalTemplates("");
    Doc.RunAppendById("sample-side", _2);
  }
  static New(FileName, Id, Keywords, Render_1, Title){
    return Create_1(Sample, {
      FileName:FileName, 
      Id:Id, 
      Keywords:Keywords, 
      Render:Render_1, 
      Title:Title
    });
  }
}
function GetFieldValues(o){
  let r=[];
  let k;
  for(var k_1 in o)r.push(o[k_1]);
  return r;
}
function Class(name){
  return ClassPred(name, true);
}
function ClassPred(name, isSet){
  return Attr.A3((el) => {
    if(isSet)AddClass(el, name);
    else RemoveClass(el, name);
  });
}
function FailWith(msg){
  throw new Error(msg);
}
function step(min, step_1, max_1){
  const s=Sign(step_1);
  return takeWhile((k) => s*(max_1-k)>=0, initInfinite((k) => min+k*step_1));
}
function KeyValue(kvp){
  return[kvp.K, kvp.V];
}
function Sign(x){
  const m=x;
  return m===0?0:m<0?-1:1;
}
function range(min, max_1){
  const count=1+max_1-min;
  return count<=0?[]:init_1(count, (x) => x+min);
}
function mapi(f, x){
  let r, l, i, go;
  if(x.$==0)return x;
  else {
    const res=Create_1(FSharpList, {$:1});
    r=res;
    l=x;
    i=0;
    go=true;
    while(go)
      {
        r.$0=f(i, l.$0);
        l=l.$1;
        if(l.$==0)go=false;
        else {
          const t=Create_1(FSharpList, {$:1});
          r=(r.$1=t,t);
          i=i+1;
        }
      }
    r.$1=FSharpList.Empty;
    return res;
  }
}
function ofArray(arr){
  let r;
  r=FSharpList.Empty;
  for(let i=length(arr)-1, _2=0;i>=_2;i--)r=FSharpList.Cons(get(arr, i), r);
  return r;
}
function ofSeq(s){
  if(s instanceof FSharpList)return s;
  else if(s instanceof Array)return ofArray(s);
  else {
    const e=Get(s);
    try {
      let go, r;
      go=e.MoveNext();
      if(!go)return FSharpList.Empty;
      else {
        const res=Create_1(FSharpList, {$:1});
        r=res;
        while(go)
          {
            r.$0=e.Current;
            if(e.MoveNext()){
              const t=Create_1(FSharpList, {$:1});
              r=(r.$1=t,t);
            }
            else go=false;
          }
        r.$1=FSharpList.Empty;
        return res;
      }
    }
    finally {
      const _2=e;
      if(typeof _2=="object"&&isIDisposable(_2))e.Dispose();
    }
  }
}
function head(l){
  return l.$==1?l.$0:listEmpty();
}
function tail(l){
  return l.$==1?l.$1:listEmpty();
}
function listEmpty(){
  return FailWith("The input list was empty.");
}
class Object_1 {
  Equals(obj){
    return this===obj;
  }
  GetHashCode(){
    return -1;
  }
}
class attr extends Object_1 { }
class Doc extends Object_1 {
  docNode;
  updates;
  static Concat(xs){
    return TreeReduce(Doc.Empty, Doc.Append, ofSeqNonCopying(xs));
  }
  static TextNode(v){
    return Doc.Mk(TextNodeDoc(globalThis.document.createTextNode(v)), Const());
  }
  static Append(a, b){
    return Doc.Mk(AppendDoc(a.docNode, b.docNode), Map2Unit(a.updates, b.updates));
  }
  static get Empty(){
    return Doc.Mk(null, Const());
  }
  static RunAppendById(id, doc){
    const m=globalThis.document.getElementById(id);
    if(Equals(m, null))FailWith("invalid id: "+id);
    else Doc.RunAppend(m, doc);
  }
  static Element(name, attr_1, children){
    const a=Attr.Concat(attr_1);
    const c=Doc.Concat(children);
    return Elt.New(globalThis.document.createElement(name), a, c);
  }
  static Mk(node, updates){
    return new Doc(node, updates);
  }
  static RunAppend(parent, doc){
    const rdelim=globalThis.document.createTextNode("");
    parent.appendChild(rdelim);
    Doc.RunBefore(rdelim, doc);
  }
  static RunBefore(rdelim, doc){
    const ldelim=globalThis.document.createTextNode("");
    rdelim.parentNode.insertBefore(ldelim, rdelim);
    Doc.RunBetween(ldelim, rdelim, doc);
  }
  static RunBetween(ldelim, rdelim, doc){
    LinkPrevElement(rdelim, doc.docNode);
    const st=CreateDelimitedRunState(ldelim, rdelim, doc.docNode);
    Sink(get_UseAnimations()||BatchUpdatesEnabled()?StartProcessor(PerformAnimatedUpdate(false, st, doc.docNode)):() => {
      PerformSyncUpdate(false, st, doc.docNode);
    }, doc.updates);
  }
  constructor(docNode, updates){
    super();
    this.docNode=docNode;
    this.updates=updates;
  }
}
let _c=Lazy((_i) => class $StartupCode_Client {
  static {
    _c=_i(this);
  }
  static All;
  static {
    const ____=Set.Singleton;
    let _2=Set.Create([____(Sample_1()), ____(Sample_2())]);
    this.All=_2;
  }
});
class FSharpList {
  static Empty=Create_1(FSharpList, {$:0});
  static Cons(Head, Tail){
    return Create_1(FSharpList, {
      $:1, 
      $0:Head, 
      $1:Tail
    });
  }
  GetEnumerator(){
    return new T(this, null, (e) => {
      const m=e.s;
      if(m.$==0)return false;
      else {
        const xs=m.$1;
        e.c=m.$0;
        e.s=xs;
        return true;
      }
    }, void 0);
  }
  $;
  $0;
  $1;
}
function Clear(el){
  while(el.hasChildNodes())
    el.removeChild(el.firstChild);
}
function Build(){
  return Builder.New(null, null, FSharpList.Empty, null, null);
}
function req(name, f){
  return f!=null&&f.$==1?f.$0:FailWith("Required property not set: "+name);
}
function op_PlusPlus(a, b){
  return a==null?b:a;
}
function NewFromSeq(fields){
  let _2;
  const r={};
  const e=Get(fields);
  try {
    while(e.MoveNext())
      {
        const f=e.Current;
        r[f[0]]=f[1];
      }
    _2=void 0;
  }
  finally {
    const _3=e;
    if(typeof _3=="object"&&isIDisposable(_3))e.Dispose();
  }
  return r;
}
class Attr {
  static HandlerImpl(event, q){
    return Attr.A3((el) => {
      el.addEventListener(event, (o) =>(q(el))(o), false);
    });
  }
  static Create(name, value){
    return Attr.A3((el) => {
      el.setAttribute(name, value);
    });
  }
  static A4(onAfterRender){
    return Create_1(Attr, {$:4, $0:onAfterRender});
  }
  static Concat(xs){
    const x=ofSeqNonCopying(xs);
    return TreeReduce(EmptyAttr(), (_2, _3) => AppendTree(_2, _3), x);
  }
  static A3(init_2){
    return Create_1(Attr, {$:3, $0:init_2});
  }
  static A2(Item1, Item2){
    return Create_1(Attr, {
      $:2, 
      $0:Item1, 
      $1:Item2
    });
  }
  $;
  $0;
  $1;
}
function ofSeqNonCopying(xs){
  if(xs instanceof Array)return xs;
  else if(xs instanceof FSharpList)return ofList(xs);
  else if(xs===null)return[];
  else {
    const q=[];
    const o=Get(xs);
    try {
      while(o.MoveNext())
        q.push(o.Current);
      return q;
    }
    finally {
      const _2=o;
      if(typeof _2=="object"&&isIDisposable(_2))o.Dispose();
    }
  }
}
function TreeReduce(defaultValue, reduction, array){
  const l=length(array);
  function loop(off){
    return(len) => {
      let _2;
      switch(len<=0?0:len===1?off>=0&&off<l?1:(_2=len,2):(_2=len,2)){
        case 0:
          return defaultValue;
        case 1:
          return get(array, off);
        case 2:
          const l2=len/2>>0;
          return reduction((loop(off))(l2), (loop(off+l2))(len-l2));
      }
    };
  }
  return(loop(0))(l);
}
function MapTreeReduce(mapping, defaultValue, reduction, array){
  const l=length(array);
  function loop(off){
    return(len) => {
      let _2;
      switch(len<=0?0:len===1?off>=0&&off<l?1:(_2=len,2):(_2=len,2)){
        case 0:
          return defaultValue;
        case 1:
          return mapping(get(array, off));
        case 2:
          const l2=len/2>>0;
          return reduction((loop(off))(l2), (loop(off+l2))(len-l2));
      }
    };
  }
  return(loop(0))(l);
}
function LoadLocalTemplates(baseName){
  !LocalTemplatesLoaded()?(set_LocalTemplatesLoaded(true),LoadNestedTemplates(globalThis.document.body, "")):void 0;
  LoadedTemplates().set_Item(baseName, LoadedTemplateFile(""));
}
function LocalTemplatesLoaded(){
  return _c_3.LocalTemplatesLoaded;
}
function set_LocalTemplatesLoaded(_2){
  _c_3.LocalTemplatesLoaded=_2;
}
function LoadNestedTemplates(root, baseName){
  const loadedTpls=LoadedTemplateFile(baseName);
  const rawTpls=new Dictionary("New_5");
  const wsTemplates=root.querySelectorAll("[ws-template]");
  for(let i=0, _2=wsTemplates.length-1;i<=_2;i++){
    const node=wsTemplates[i];
    const name=node.getAttribute("ws-template").toLowerCase();
    node.removeAttribute("ws-template");
    rawTpls.set_Item(name, FakeRootSingle(node));
  }
  const wsChildrenTemplates=root.querySelectorAll("[ws-children-template]");
  for(let i_1=0, _3=wsChildrenTemplates.length-1;i_1<=_3;i_1++){
    const node_1=wsChildrenTemplates[i_1];
    const name_1=node_1.getAttribute("ws-children-template").toLowerCase();
    node_1.removeAttribute("ws-children-template");
    rawTpls.set_Item(name_1, FakeRoot(node_1));
  }
  const html5TemplateBasedTemplates=root.querySelectorAll("template[id]");
  for(let i_2=0, _4=html5TemplateBasedTemplates.length-1;i_2<=_4;i_2++){
    const node_2=html5TemplateBasedTemplates[i_2];
    rawTpls.set_Item(node_2.getAttribute("id").toLowerCase(), FakeRootFromHTMLTemplate(node_2));
  }
  const html5TemplateBasedTemplates_1=root.querySelectorAll("template[name]");
  for(let i_3=0, _5=html5TemplateBasedTemplates_1.length-1;i_3<=_5;i_3++){
    const node_3=html5TemplateBasedTemplates_1[i_3];
    rawTpls.set_Item(node_3.getAttribute("name").toLowerCase(), FakeRootFromHTMLTemplate(node_3));
  }
  const instantiated=new HashSet("New_3");
  function prepareTemplate(name_2){
    if(!loadedTpls.ContainsKey(name_2)){
      let o;
      const m=(o=null,[rawTpls.TryGetValue(name_2, {get:() => o, set:(v) => {
        o=v;
      }}), o]);
      if(m[0]){
        instantiated.SAdd(name_2);
        rawTpls.RemoveKey(name_2);
        PrepareTemplateStrict(baseName, Some(name_2), m[1], Some(prepareTemplate));
      }
      else console.warn(instantiated.Contains(name_2)?"Encountered loop when instantiating "+name_2:"Local template does not exist: "+name_2);
    }
  }
  while(rawTpls.count>0)
    prepareTemplate(head_1(rawTpls.Keys));
}
function LoadedTemplates(){
  return _c_3.LoadedTemplates;
}
function LoadedTemplateFile(name){
  let o;
  const m=(o=null,[LoadedTemplates().TryGetValue(name, {get:() => o, set:(v) => {
    o=v;
  }}), o]);
  if(m[0])return m[1];
  else {
    const d=new Dictionary("New_5");
    LoadedTemplates().set_Item(name, d);
    return d;
  }
}
function FakeRootSingle(el){
  let _2;
  el.removeAttribute("ws-template");
  const m=el.getAttribute("ws-replace");
  if(m==null)_2=null;
  else {
    el.removeAttribute("ws-replace");
    const m_1=el.parentNode;
    if(Equals(m_1, null))_2=null;
    else {
      const n=globalThis.document.createElement(el.tagName);
      _2=(n.setAttribute("ws-replace", m),void m_1.replaceChild(n, el));
    }
  }
  const fakeroot=globalThis.document.createElement("div");
  fakeroot.appendChild(el);
  return fakeroot;
}
function FakeRoot(parent){
  const fakeroot=globalThis.document.createElement("div");
  while(parent.hasChildNodes())
    fakeroot.appendChild(parent.firstChild);
  return fakeroot;
}
function FakeRootFromHTMLTemplate(parent){
  const fakeroot=globalThis.document.createElement("div");
  const content=parent.content;
  for(let i=0, _2=content.childNodes.length-1;i<=_2;i++)fakeroot.appendChild(content.childNodes[i].cloneNode(true));
  return fakeroot;
}
function PrepareTemplateStrict(baseName, name, fakeroot, prepareLocalTemplate){
  const processedHTML5Templates=new HashSet("New_3");
  function recF(recI, _2){
    while(true)
      switch(recI){
        case 0:
          if(_2!==null){
            const next=_2.nextSibling;
            if(Equals(_2.nodeType, Node.TEXT_NODE))convertTextNode(_2);
            else Equals(_2.nodeType, Node.ELEMENT_NODE)?convertElement(_2):null;
            _2=next;
          }
          else return null;
          break;
        case 1:
          let _3;
          let _4;
          const name_2=string(_2.nodeName, Some(3), null).toLowerCase();
          const m=name_2.indexOf(".");
          const p=m===-1?[baseName, name_2]:[string(name_2, null, Some(m-1)), string(name_2, Some(m+1), null)];
          const instName=p[1];
          const instBaseName=p[0];
          if(instBaseName!=""&&!LoadedTemplates().ContainsKey(instBaseName))return failNotLoaded(instName);
          else {
            if(instBaseName==""&&prepareLocalTemplate!=null)prepareLocalTemplate.$0(instName);
            else null;
            const d=LoadedTemplates().Item(instBaseName);
            if(!d.ContainsKey(instName))return failNotLoaded(instName);
            else {
              const t=d.Item(instName);
              const instance=t.cloneNode(true);
              const usedHoles=new HashSet("New_3");
              const mappings=new Dictionary("New_5");
              const attrs=_2.attributes;
              for(let i=0, _7=attrs.length-1;i<=_7;i++){
                const name_3=attrs.item(i).name.toLowerCase();
                const m_1=attrs.item(i).nodeValue;
                let _5=m_1==""?name_3:m_1.toLowerCase();
                mappings.set_Item(name_3, _5);
                if(!usedHoles.SAdd(name_3))console.warn("Hole mapped twice", name_3);
              }
              for(let i_1=0, _8=_2.childNodes.length-1;i_1<=_8;i_1++){
                const n=_2.childNodes[i_1];
                if(Equals(n.nodeType, Node.ELEMENT_NODE))if(!usedHoles.SAdd(n.nodeName.toLowerCase()))console.warn("Hole filled twice", instName);
              }
              const singleTextFill=_2.childNodes.length===1&&Equals(_2.firstChild.nodeType, Node.TEXT_NODE);
              if(singleTextFill){
                const x=fillTextHole(instance, _2.firstChild.textContent, instName);
                const f=((usedHoles_1) =>(i_2) => usedHoles_1.SAdd(i_2))(usedHoles);
                let _6=((a) =>(o) => {
                  if(o!=null)a(o.$0);
                })((x_1) => {
                  f(x_1);
                });
                _3=_6(x);
              }
              else _3=null;
              removeHolesExcept(instance, usedHoles);
              if(!singleTextFill){
                for(let i_2=0, _9=_2.childNodes.length-1;i_2<=_9;i_2++){
                  const n_1=_2.childNodes[i_2];
                  if(Equals(n_1.nodeType, Node.ELEMENT_NODE))if(n_1.hasAttributes())fillInstanceAttrs(instance, n_1);
                  else fillDocHole(instance, n_1);
                }
                _4=void 0;
              }
              else _4=null;
              mapHoles(instance, mappings);
              fill(instance, _2.parentNode, _2);
              _2.parentNode.removeChild(_2);
              return;
            }
          }
          break;
      }
  }
  function fillDocHole(instance, fillWith){
    const name_2=fillWith.nodeName.toLowerCase();
    const fillHole=(p, n) => {
      let _2;
      if(name_2=="title"&&fillWith.hasChildNodes()){
        const parsed=ParseHTMLIntoFakeRoot(fillWith.textContent);
        fillWith.removeChild(fillWith.firstChild);
        while(parsed.hasChildNodes())
          fillWith.appendChild(parsed.firstChild);
        _2=void 0;
      }
      else _2=null;
      convertElement(fillWith);
      return fill(fillWith, p, n);
    };
    foreachNotPreserved(instance, "[ws-attr-holes]", (e) => {
      const holeAttrs=SplitChars(e.getAttribute("ws-attr-holes"), [" "], 1);
      for(let i=0, _3=holeAttrs.length-1;i<=_3;i++){
        const attrName=get(holeAttrs, i);
        let this_1=new RegExp("\\${"+name_2+"}", "ig");
        let str=e.getAttribute(attrName);
        let newSubStr=fillWith.textContent;
        let _2=str.replace(this_1, newSubStr);
        e.setAttribute(attrName, _2);
      }
    });
    const m=instance.querySelector("[ws-hole="+name_2+"]");
    if(Equals(m, null)){
      const m_1=instance.querySelector("[ws-replace="+name_2+"]");
      if(Equals(m_1, null)){
        const m_2=instance.querySelector("slot[name="+name_2+"]");
        return instance.tagName.toLowerCase()=="template"?(fillHole(m_2.parentNode, m_2),void m_2.parentNode.removeChild(m_2)):null;
      }
      else {
        fillHole(m_1.parentNode, m_1);
        m_1.parentNode.removeChild(m_1);
        return;
      }
    }
    else {
      while(m.hasChildNodes())
        m.removeChild(m.lastChild);
      m.removeAttribute("ws-hole");
      return(((a) => {
        const _2=a;
        return(_3) => fillHole(_2, _3);
      })(m))(null);
    }
  }
  function convertElement(el){
    if(!el.hasAttribute("ws-preserve"))if(StartsWith(el.nodeName.toLowerCase(), "ws-"))convertInstantiation(el);
    else {
      convertAttrs(el);
      convertNodeAndSiblings(el.firstChild);
    }
  }
  function convertNodeAndSiblings(n){
    return recF(0, n);
  }
  function convertInstantiation(el){
    return recF(1, el);
  }
  function convertNestedTemplates(el){
    while(true)
      {
        const m=el.querySelector("[ws-template]");
        if(Equals(m, null)){
          const m_1=el.querySelector("[ws-children-template]");
          if(Equals(m_1, null)){
            const idTemplates=el.querySelectorAll("template[id]");
            for(let i=1, _2=idTemplates.length-1;i<=_2;i++){
              const n=idTemplates[i];
              if(processedHTML5Templates.Contains(n)){ }
              else {
                PrepareTemplateStrict(baseName, Some(n.getAttribute("id")), n, null);
                processedHTML5Templates.SAdd(n);
              }
            }
            const nameTemplates=el.querySelectorAll("template[name]");
            for(let i_1=1, _3=nameTemplates.length-1;i_1<=_3;i_1++){
              const n_1=nameTemplates[i_1];
              if(processedHTML5Templates.Contains(n_1)){ }
              else {
                PrepareTemplateStrict(baseName, Some(n_1.getAttribute("name")), n_1, null);
                processedHTML5Templates.SAdd(n_1);
              }
            }
            return null;
          }
          else {
            const name_2=m_1.getAttribute("ws-children-template");
            m_1.removeAttribute("ws-children-template");
            PrepareTemplateStrict(baseName, Some(name_2), m_1, null);
            el=el;
          }
        }
        else {
          const name_3=m.getAttribute("ws-template");
          (PrepareSingleTemplate(baseName, Some(name_3), m))(null);
          el=el;
        }
      }
  }
  const name_1=(name==null?"":name.$0).toLowerCase();
  LoadedTemplateFile(baseName).set_Item(name_1, fakeroot);
  if(fakeroot.hasChildNodes()){
    convertNestedTemplates(fakeroot);
    convertNodeAndSiblings(fakeroot.firstChild);
  }
}
function foreachNotPreserved(root, selector, f){
  IterSelector(root, selector, (p) => {
    if(p.closest("[ws-preserve]")==null)f(p);
  });
}
function PrepareSingleTemplate(baseName, name, el){
  const root=FakeRootSingle(el);
  return(p) => {
    PrepareTemplateStrict(baseName, name, root, p);
  };
}
function TextHoleRE(){
  return _c_3.TextHoleRE;
}
function Sample_1(){
  return _c_1.Sample;
}
function Join(data, context){
  _1.select(context).append("svg").attr("height", 500).selectAll("circle").data(data).enter().append("circle").attr("cx", (x) => x*5).attr("cy", (x) => 50+x*x/40).attr("r", (_2, _3) => 7+5*Math.sin(3*(+_3/+length(data))*Math.PI));
}
function Data(){
  return _c_1.Data;
}
function Sample_2(){
  return _c_2.Sample;
}
function Render(ctx){
  const width=960;
  const height=500;
  const projection=_1.geoOrthographic().scale(248).clipAngle(90);
  const title=_1.select(ctx).append("h1");
  const c=_1.select(ctx).append("canvas").attr("width", width).attr("height", height).node().getContext("2d");
  const path=_1.geoPath().projection(projection).context(c);
  const f=() => {
    const globe={type:"sphere"};
    const x=D3_JsonAsync("WorldTour/world-110m.json");
    const f_1=(a) => {
      const x_1=D3_TsvAsync("WorldTour/world-country-names.tsv");
      const f_2=(a_1) => {
        const landFeature=topojson().feature(a, a.objects.land);
        const countries=topojson().feature(a, a.objects.countries).features;
        const borders=topojson().mesh(a, a.objects.countries, (t) => t[0]!==t[1]);
        const countries_1=sortBy((c_1) => c_1.name, filter((d) => exists((n) => d.id==n.id&&(d.name=n.name,true), a_1), countries));
        function transition(i){
          _1.transition().duration(1250).each(CreateFuncWithArgs(() => {
            title.text(get(countries_1, i).name);
          })).tween("rotate", () => {
            let a_2=get(countries_1, i);
            const p=_1.geoCentroid(a_2);
            let arg1=projection.rotate();
            const r=_1.interpolate(arg1, [-p[0], -p[1], 0]);
            return(t) => {
              const p_1=r(t);
              projection.rotate([p_1[0], p_1[1], p_1[2]]);
              c.clearRect(0, 0, width, height);
              c.fillStyle="#bbb";
              c.beginPath();
              path(landFeature);
              c.fill();
              c.fillStyle="#f00";
              c.beginPath();
              path(get(countries_1, i));
              c.fill();
              c.strokeStyle="#fff";
              c.lineWidth=0.5;
              c.beginPath();
              path(borders);
              c.stroke();
              c.strokeStyle="#000";
              c.lineWidth=2;
              c.beginPath();
              path(globe);
              return c.stroke();
            };
          }).transition().on("end", () => transition(i+1));
        }
        transition(0);
        return Return(null);
      };
      return Bind(x_1, (_4, _5) =>(f_2(_4))(_5));
    };
    return Bind(x, (_4, _5) =>(f_1(_4))(_5));
  };
  let _2=Delay((_4, _5) =>(f(_4))(_5));
  let _3=_2;
  Start(_3, null);
}
function topojson(){
  return _c_2.topojson;
}
function Updates(dyn){
  return MapTreeReduce((x) => x.NChanged, Const(), Map2Unit, dyn.DynNodes);
}
function AppendTree(a, b){
  if(a===null)return b;
  else if(b===null)return a;
  else {
    const x=Attr.A2(a, b);
    SetFlags(x, Flags(a)|Flags(b));
    return x;
  }
}
function EmptyAttr(){
  return _c_4.EmptyAttr;
}
function Insert(elem, tree){
  const nodes=[];
  const oar=[];
  function loop(node){
    while(true)
      {
        if(!(node===null)){
          if(node!=null&&node.$==1)return nodes.push(node.$0);
          else if(node!=null&&node.$==2){
            const b=node.$1;
            const a=node.$0;
            loop(a);
            node=b;
          }
          else return node!=null&&node.$==3?node.$0(elem):node!=null&&node.$==4?oar.push(node.$0):null;
        }
        else return null;
      }
  }
  loop(tree);
  const arr=nodes.slice(0);
  let _2=New(elem, Flags(tree), arr, oar.length===0?null:Some((el) => {
    iter_1((f) => {
      f(el);
    }, oar);
  }));
  return _2;
}
function SetFlags(a, f){
  a.flags=f;
}
function Flags(a){
  return a!==null&&a.hasOwnProperty("flags")?a.flags:0;
}
function HasExitAnim(attr_1){
  const flag=2;
  return(attr_1.DynFlags&flag)===flag;
}
function GetExitAnim(dyn){
  return GetAnim(dyn, (_2, _3) => _2.NGetExitAnim(_3));
}
function HasEnterAnim(attr_1){
  const flag=1;
  return(attr_1.DynFlags&flag)===flag;
}
function GetEnterAnim(dyn){
  return GetAnim(dyn, (_2, _3) => _2.NGetEnterAnim(_3));
}
function HasChangeAnim(attr_1){
  const flag=4;
  return(attr_1.DynFlags&flag)===flag;
}
function GetChangeAnim(dyn){
  return GetAnim(dyn, (_2, _3) => _2.NGetChangeAnim(_3));
}
function GetAnim(dyn, f){
  return Concat(map((n) => f(n, dyn.DynElem), dyn.DynNodes));
}
function Sync(elem, dyn){
  iter((d) => {
    d.NSync(elem);
  }, dyn.DynNodes);
}
function AddClass(element, cl){
  const c=getClass(element);
  if(c=="")setClass(element, cl);
  else!clsRE(cl).test(c)?setClass(element, c+" "+cl):void 0;
}
function RemoveClass(element, cl){
  let this_1=clsRE(cl);
  let str=getClass(element);
  let replaceFn=(_3, _4, _5) => _4==""||_5==""?"":" ";
  let _2=str.replace(this_1, replaceFn);
  setClass(element, _2);
}
function getClass(element){
  return element instanceof SVGElement?element.getAttribute("class"):element.className;
}
function setClass(element, value){
  if(element instanceof SVGElement)element.setAttribute("class", value);
  else element.className=value;
}
function clsRE(cls){
  return new RegExp("(\\s+|^)"+cls+"(?:\\s+"+cls+")*(\\s+|$)", "g");
}
function ParseHTMLIntoFakeRoot(elem){
  const root=globalThis.document.createElement("div");
  if(!rhtml().test(elem)){
    root.appendChild(globalThis.document.createTextNode(elem));
    return root;
  }
  else {
    const m=rtagName().exec(elem);
    const tag=Equals(m, null)?"":get(m, 1).toLowerCase();
    const w=(wrapMap())[tag];
    const p=w?w:defaultWrap();
    root.innerHTML=p[1]+elem.replace(rxhtmlTag(), "<$1></$2>")+p[2];
    function unwrap(elt, a){
      while(true)
        {
          if(a===0)return elt;
          else {
            const i=a;
            elt=elt.lastChild;
            a=i-1;
          }
        }
    }
    return(((a) => {
      const _2=a;
      return(_3) => unwrap(_2, _3);
    })(root))(p[0]);
  }
}
function rhtml(){
  return _c_5.rhtml;
}
function wrapMap(){
  return _c_5.wrapMap;
}
function defaultWrap(){
  return _c_5.defaultWrap;
}
function rxhtmlTag(){
  return _c_5.rxhtmlTag;
}
function rtagName(){
  return _c_5.rtagName;
}
function IterSelector(el, selector, f){
  const l=el.querySelectorAll(selector);
  for(let i=0, _2=l.length-1;i<=_2;i++)f(l[i]);
}
function InsertAt(parent, pos, node){
  let _2;
  if(node.parentNode===parent){
    const m=node.nextSibling;
    let _3=Equals(m, null)?null:m;
    _2=pos===_3;
  }
  else _2=false;
  if(!_2)parent.insertBefore(node, pos);
}
function RemoveNode(parent, el){
  if(el.parentNode===parent)parent.removeChild(el);
}
function Const(x){
  const o={s:Forever(x)};
  return() => o;
}
function Map2Unit(a, a_1){
  return CreateLazy(() => Map2Unit_1(a(), a_1()));
}
function CreateLazy(observe){
  const lv={c:null, o:observe};
  return() => {
    let c;
    c=lv.c;
    if(c===null){
      c=lv.o();
      lv.c=c;
      const _2=c.s;
      if(_2!=null&&_2.$==0)lv.o=null;
      else WhenObsoleteRun(c, () => {
        lv.c=null;
      });
      return c;
    }
    else return c;
  };
}
function Sink(act, a){
  function loop(){
    WhenRun(a(), act, () => {
      scheduler().Fork(loop);
    });
  }
  scheduler().Fork(loop);
}
function Map(fn, a){
  return CreateLazy(() => Map_1(fn, a()));
}
function TextNodeDoc(Item){
  return{$:5, $0:Item};
}
function AppendDoc(Item1, Item2){
  return{
    $:0, 
    $0:Item1, 
    $1:Item2
  };
}
function ElemDoc(Item){
  return{$:1, $0:Item};
}
class View { }
function ofList(xs){
  let l;
  const q=[];
  l=xs;
  while(!(l.$==0))
    {
      q.push(head(l));
      l=tail(l);
    }
  return q;
}
function exists(f, x){
  let e, i;
  e=false;
  i=0;
  const l=length(x);
  while(!e&&i<l)
    if(f(x[i]))e=true;
    else i=i+1;
  return e;
}
function tryPick(f, arr){
  let res, i;
  res=null;
  i=0;
  while(i<arr.length&&res==null)
    {
      const m=f(arr[i]);
      if(m!=null&&m.$==1)res=m;
      i=i+1;
    }
  return res;
}
function tryFindIndex(f, arr){
  let res, i;
  res=null;
  i=0;
  while(i<arr.length&&res==null)
    {
      f(arr[i])?res=Some(i):void 0;
      i=i+1;
    }
  return res;
}
function ofSeq_1(xs){
  if(xs instanceof Array)return xs.slice();
  else if(xs instanceof FSharpList)return ofList(xs);
  else {
    const q=[];
    const o=Get(xs);
    try {
      while(o.MoveNext())
        q.push(o.Current);
      return q;
    }
    finally {
      const _2=o;
      if(typeof _2=="object"&&isIDisposable(_2))o.Dispose();
    }
  }
}
function filter(f, arr){
  const r=[];
  for(let i=0, _2=arr.length-1;i<=_2;i++)if(f(arr[i]))r.push(arr[i]);
  return r;
}
function sortBy(f, arr){
  return map((t) => t[0], mapi_1((_2, _3) =>[_3, [f(_3), _2]], arr).sort((_2, _3) => Compare(_2[1], _3[1])));
}
function map(f, arr){
  const r=new Array(arr.length);
  for(let i=0, _2=arr.length-1;i<=_2;i++)r[i]=f(arr[i]);
  return r;
}
function iter(f, arr){
  for(let i=0, _2=arr.length-1;i<=_2;i++)f(arr[i]);
}
function mapi_1(f, arr){
  const y=new Array(arr.length);
  for(let i=0, _2=arr.length-1;i<=_2;i++)y[i]=f(i, arr[i]);
  return y;
}
function concat(xs){
  return Array.prototype.concat.apply([], ofSeq_1(xs));
}
function pick(f, arr){
  const m=tryPick(f, arr);
  return m==null?FailWith("KeyNotFoundException"):m.$0;
}
function foldBack(f, arr, zero){
  let acc;
  acc=zero;
  const len=arr.length;
  for(let i=1, _2=len;i<=_2;i++)acc=f(arr[len-i], acc);
  return acc;
}
function choose(f, arr){
  const q=[];
  for(let i=0, _2=arr.length-1;i<=_2;i++){
    const m=f(arr[i]);
    if(m==null){ }
    else q.push(m.$0);
  }
  return q;
}
function create(size, value){
  const r=new Array(size);
  for(let i=0, _2=size-1;i<=_2;i++)r[i]=value;
  return r;
}
function init(size, f){
  if(size<0)FailWith("Negative size given.");
  else null;
  const r=new Array(size);
  for(let i=0, _2=size-1;i<=_2;i++)r[i]=f(i);
  return r;
}
function forall(f, x){
  let a, i;
  a=true;
  i=0;
  const l=length(x);
  while(a&&i<l)
    if(f(x[i]))i=i+1;
    else a=false;
  return a;
}
class Dictionary extends Object_1 {
  equals;
  hash;
  count;
  data;
  set_Item(k, v){
    this.set(k, v);
  }
  ContainsKey(k){
    const d=this.data[this.hash(k)];
    return d==null?false:exists((a) => this.equals.apply(null, [(KeyValue(a))[0], k]), d);
  }
  TryGetValue(k, res){
    const d=this.data[this.hash(k)];
    if(d==null)return false;
    else {
      const v=tryPick((a) => {
        const a_1=KeyValue(a);
        return this.equals.apply(null, [a_1[0], k])?Some(a_1[1]):null;
      }, d);
      return v!=null&&v.$==1&&(res.set(v.$0),true);
    }
  }
  RemoveKey(k){
    return this.remove(k);
  }
  get Keys(){
    return new KeyCollection(this);
  }
  set(k, v){
    const h=this.hash(k);
    const d=this.data[h];
    if(d==null){
      this.count=this.count+1;
      this.data[h]=new Array({K:k, V:v});
    }
    else {
      const m=tryFindIndex((a) => this.equals.apply(null, [(KeyValue(a))[0], k]), d);
      if(m==null){
        this.count=this.count+1;
        d.push({K:k, V:v});
      }
      else d[m.$0]={K:k, V:v};
    }
  }
  remove(k){
    const h=this.hash(k);
    const d=this.data[h];
    if(d==null)return false;
    else {
      const r=filter((a) =>!this.equals.apply(null, [(KeyValue(a))[0], k]), d);
      return length(r)<d.length&&(this.count=this.count-1,this.data[h]=r,true);
    }
  }
  Item(k){
    return this.get(k);
  }
  GetEnumerator(){
    return Get0(concat(GetFieldValues(this.data)));
  }
  get(k){
    const d=this.data[this.hash(k)];
    return d==null?notPresent():pick((a) => {
      const a_1=KeyValue(a);
      return this.equals.apply(null, [a_1[0], k])?Some(a_1[1]):null;
    }, d);
  }
  constructor(i, _2, _3, _4){
    if(i=="New_5"){
      i="New_6";
      _2=[];
      _3=Equals;
      _4=Hash;
    }
    if(i=="New_6"){
      const init_2=_2;
      const equals=_3;
      const hash=_4;
      super();
      this.equals=equals;
      this.hash=hash;
      this.count=0;
      this.data=[];
      const e=Get(init_2);
      try {
        while(e.MoveNext())
          {
            const x=e.Current;
            this.set(x.K, x.V);
          }
      }
      finally {
        const _5=e;
        if(typeof _5=="object"&&isIDisposable(_5))e.Dispose();
      }
    }
  }
}
function delay(f){
  return{GetEnumerator:() => Get(f())};
}
function collect(f, s){
  return concat_1(map_1(f, s));
}
function head_1(s){
  const e=Get(s);
  try {
    return e.MoveNext()?e.Current:insufficient();
  }
  finally {
    const _2=e;
    if(typeof _2=="object"&&isIDisposable(_2))e.Dispose();
  }
}
function concat_1(ss){
  return{GetEnumerator:() => {
    const outerE=Get(ss);
    function next(st){
      while(true)
        {
          const m=st.s;
          if(Equals(m, null)){
            if(outerE.MoveNext()){
              st.s=Get(outerE.Current);
              st=st;
            }
            else {
              outerE.Dispose();
              return false;
            }
          }
          else if(m.MoveNext()){
            st.c=m.Current;
            return true;
          }
          else {
            st.Dispose();
            st.s=null;
            st=st;
          }
        }
    }
    return new T(null, null, next, (st) => {
      const x=st.s;
      if(!Equals(x, null))x.Dispose();
      const x_1=outerE;
      if(!Equals(x_1, null))x_1.Dispose();
    });
  }};
}
function map_1(f, s){
  return{GetEnumerator:() => {
    const en=Get(s);
    return new T(null, null, (e) => en.MoveNext()&&(e.c=f(en.Current),true), () => {
      en.Dispose();
    });
  }};
}
function initInfinite(f){
  return{GetEnumerator:() => new T(0, null, (e) => {
    e.c=f(e.s);
    e.s=e.s+1;
    return true;
  }, void 0)};
}
function takeWhile(f, s){
  return delay(() => enumUsing(Get(s), (e) => enumWhile(() => e.MoveNext()&&f(e.Current), delay(() =>[e.Current]))));
}
function fold(f, x, s){
  let r;
  r=x;
  const e=Get(s);
  try {
    while(e.MoveNext())
      r=f(r, e.Current);
    return r;
  }
  finally {
    const _2=e;
    if(typeof _2=="object"&&isIDisposable(_2))e.Dispose();
  }
}
function iter_1(p, s){
  const e=Get(s);
  try {
    while(e.MoveNext())
      p(e.Current);
  }
  finally {
    const _2=e;
    if(typeof _2=="object"&&isIDisposable(_2))e.Dispose();
  }
}
function init_1(n, f){
  return take(n, initInfinite(f));
}
function take(n, s){
  n<0?nonNegative():void 0;
  return{GetEnumerator:() => {
    const e=[Get(s)];
    return new T(0, null, (o) => {
      o.s=o.s+1;
      if(o.s>n)return false;
      else {
        const en=e[0];
        return Equals(en, null)?insufficient():en.MoveNext()?(o.c=en.Current,o.s===n?(en.Dispose(),e[0]=null):void 0,true):(en.Dispose(),e[0]=null,insufficient());
      }
    }, () => {
      const x=e[0];
      if(!Equals(x, null))x.Dispose();
    });
  }};
}
function forall_1(p, s){
  return!exists_1((x) =>!p(x), s);
}
function max(s){
  const e=Get(s);
  try {
    let m;
    if(!e.MoveNext())seqEmpty();
    else null;
    m=e.Current;
    while(e.MoveNext())
      {
        const x=e.Current;
        if(Compare(x, m)===1)m=x;
      }
    return m;
  }
  finally {
    const _2=e;
    if(typeof _2=="object"&&isIDisposable(_2))e.Dispose();
  }
}
function exists_1(p, s){
  const e=Get(s);
  try {
    let r;
    r=false;
    while(!r&&e.MoveNext())
      r=p(e.Current);
    return r;
  }
  finally {
    const _2=e;
    if(typeof _2=="object"&&isIDisposable(_2))e.Dispose();
  }
}
function seqEmpty(){
  return FailWith("The input sequence was empty.");
}
let _c_1=Lazy((_i) => class $StartupCode_Circles {
  static {
    _c_1=_i(this);
  }
  static Sample;
  static Data;
  static {
    this.Data=ofSeq_1(step(10, 10, 100));
    this.Sample=Build().Id("Circles").FileName("Circles.fs").Keywords(ofArray(["basic"])).Render((c) => {
      Join(Data(), c);
    }).Create();
  }
});
let _c_2=Lazy((_i) => class $StartupCode_WorldTour {
  static {
    _c_2=_i(this);
  }
  static Sample;
  static topojson;
  static {
    this.topojson=globalThis.topojson;
    this.Sample=Build().Id("WorldTour").FileName("WorldTour.fs").Render((c) => {
      Render(c);
    }).Create();
  }
});
function Map2Unit_1(sn1, sn2){
  const _2=sn1.s;
  const _3=sn2.s;
  if(_2!=null&&_2.$==0)return _3!=null&&_3.$==0?{s:Forever(null)}:sn2;
  else if(_3!=null&&_3.$==0)return sn1;
  else {
    const res={s:Waiting([], [])};
    const cont=() => {
      const m=res.s;
      if(!(m!=null&&m.$==0||m!=null&&m.$==2)){
        const _4=ValueAndForever(sn1);
        const _5=ValueAndForever(sn2);
        if(_4!=null&&_4.$==1)if(_5!=null&&_5.$==1)if(_4.$0[1]&&_5.$0[1])MarkForever(res, null);
        else MarkReady(res, null);
      }
    };
    When(sn1, cont, res);
    When(sn2, cont, res);
    return res;
  }
}
function WhenObsoleteRun(snap, obs){
  const m=snap.s;
  if(m==null)obs();
  else m!=null&&m.$==2?(m.$0,m.$1.push(obs)):m!=null&&m.$==3?(m.$0,m.$1.push(obs)):m.$0;
}
function ValueAndForever(snap){
  const m=snap.s;
  return m!=null&&m.$==0?Some([m.$0, true]):m!=null&&m.$==2?Some([m.$0, false]):null;
}
function MarkForever(sn, v){
  const m=sn.s;
  if(m!=null&&m.$==3){
    const q=m.$0;
    sn.s=Forever(v);
    for(let i=0, _2=length(q)-1;i<=_2;i++)(get(q, i))(v);
  }
  else void 0;
}
function MarkReady(sn, v){
  const m=sn.s;
  if(m!=null&&m.$==3){
    const q2=m.$1;
    const q1=m.$0;
    sn.s=Ready(v, q2);
    for(let i=0, _2=length(q1)-1;i<=_2;i++)(get(q1, i))(v);
  }
  else void 0;
}
function When(snap, avail, obs){
  const m=snap.s;
  if(m==null)Obsolete(obs);
  else if(m!=null&&m.$==2){
    const v=m.$0;
    EnqueueSafe(m.$1, obs);
    avail(v);
  }
  else if(m!=null&&m.$==3){
    const q2=m.$1;
    m.$0.push(avail);
    EnqueueSafe(q2, obs);
  }
  else avail(m.$0);
}
function EnqueueSafe(q, x){
  q.push(x);
  if(q.length%20===0){
    const qcopy=q.slice(0);
    Clear_1(q);
    for(let i=0, _2=length(qcopy)-1;i<=_2;i++){
      const o=get(qcopy, i);
      if(typeof o=="object")(((sn) => {
        if(sn.s)q.push(sn);
      })(o));
      else(((f) => {
        q.push(f);
      })(o));
    }
  }
  else void 0;
}
function Copy(sn){
  const m=sn.s;
  if(m==null)return sn;
  else if(m!=null&&m.$==2){
    const res={s:Ready(m.$0, [])};
    WhenObsolete(sn, res);
    return res;
  }
  else if(m!=null&&m.$==3){
    const res_1={s:Waiting([], [])};
    When(sn, (v) => {
      MarkDone(res_1, sn, v);
    }, res_1);
    return res_1;
  }
  else return sn;
}
function WhenRun(snap, avail, obs){
  const m=snap.s;
  if(m==null)obs();
  else if(m!=null&&m.$==2){
    const v=m.$0;
    m.$1.push(obs);
    avail(v);
  }
  else if(m!=null&&m.$==3){
    const q2=m.$1;
    m.$0.push(avail);
    q2.push(obs);
  }
  else avail(m.$0);
}
function WhenObsolete(snap, obs){
  const m=snap.s;
  if(m==null)Obsolete(obs);
  else m!=null&&m.$==2?(m.$0,EnqueueSafe(m.$1, obs)):m!=null&&m.$==3?(m.$0,EnqueueSafe(m.$1, obs)):m.$0;
}
function MarkDone(res, sn, v){
  const _2=sn.s;
  if(_2!=null&&_2.$==0)MarkForever(res, v);
  else MarkReady(res, v);
}
function Map_1(fn, sn){
  const m=sn.s;
  if(m!=null&&m.$==0)return{s:Forever(fn(m.$0))};
  else {
    const res={s:Waiting([], [])};
    When(sn, (a) => {
      MarkDone(res, sn, fn(a));
    }, res);
    return res;
  }
}
class DocElemNode {
  Attr;
  Children;
  Delimiters;
  El;
  ElKey;
  Render;
  Equals(o){
    return this.ElKey===o.ElKey;
  }
  GetHashCode(){
    return this.ElKey;
  }
  static New(Attr_1, Children_1, Delimiters, El, ElKey, Render_1){
    const _2={
      Attr:Attr_1, 
      Children:Children_1, 
      El:El, 
      ElKey:ElKey
    };
    let _3=(SetOptional(_2, "Delimiters", Delimiters),SetOptional(_2, "Render", Render_1),_2);
    return Create_1(DocElemNode, _3);
  }
}
function Get(x){
  return x instanceof Array?ArrayEnumerator(x):Equals(typeof x, "string")?StringEnumerator(x):x.GetEnumerator();
}
function ArrayEnumerator(s){
  return new T(0, null, (e) => {
    const i=e.s;
    return i<length(s)&&(e.c=get(s, i),e.s=i+1,true);
  }, void 0);
}
function StringEnumerator(s){
  return new T(0, null, (e) => {
    const i=e.s;
    return i<s.length&&(e.c=s[i],e.s=i+1,true);
  }, void 0);
}
function Get0(x){
  return x instanceof Array?ArrayEnumerator(x):Equals(typeof x, "string")?StringEnumerator(x):"GetEnumerator0"in x?x.GetEnumerator0():x.GetEnumerator();
}
let _c_3=Lazy((_i) => class $StartupCode_Templates {
  static {
    _c_3=_i(this);
  }
  static RenderedFullDocTemplate;
  static TextHoleRE;
  static GlobalHoles;
  static LocalTemplatesLoaded;
  static LoadedTemplates;
  static {
    this.LoadedTemplates=new Dictionary("New_5");
    this.LocalTemplatesLoaded=false;
    this.GlobalHoles=new Dictionary("New_5");
    this.TextHoleRE="\\${([^}]+)}";
    this.RenderedFullDocTemplate=null;
  }
});
class HashSet extends Object_1 {
  equals;
  hash;
  data;
  count;
  SAdd(item){
    return this.add(item);
  }
  Contains(item){
    const arr=this.data[this.hash(item)];
    return arr==null?false:this.arrContains(item, arr);
  }
  add(item){
    const h=this.hash(item);
    const arr=this.data[h];
    return arr==null?(this.data[h]=[item],this.count=this.count+1,true):this.arrContains(item, arr)?false:(arr.push(item),this.count=this.count+1,true);
  }
  arrContains(item, arr){
    let c, i;
    c=true;
    i=0;
    const l=arr.length;
    while(c&&i<l)
      if(this.equals.apply(null, [arr[i], item]))c=false;
      else i=i+1;
    return!c;
  }
  GetEnumerator(){
    return Get(concat_3(this.data));
  }
  ExceptWith(xs){
    const e=Get(xs);
    try {
      while(e.MoveNext())
        this.Remove(e.Current);
    }
    finally {
      const _2=e;
      if(typeof _2=="object"&&isIDisposable(_2))e.Dispose();
    }
  }
  get Count(){
    return this.count;
  }
  IntersectWith(xs){
    const other=new HashSet("New_4", xs, this.equals, this.hash);
    const all=concat_3(this.data);
    for(let i=0, _2=all.length-1;i<=_2;i++){
      const item=all[i];
      if(!other.Contains(item))this.Remove(item);
    }
  }
  Remove(item){
    const arr=this.data[this.hash(item)];
    return arr==null?false:this.arrRemove(item, arr)&&(this.count=this.count-1,true);
  }
  CopyTo(arr, index){
    const all=concat_3(this.data);
    for(let i=0, _2=all.length-1;i<=_2;i++)set(arr, i+index, all[i]);
  }
  arrRemove(item, arr){
    let c, i;
    c=true;
    i=0;
    const l=arr.length;
    while(c&&i<l)
      if(this.equals.apply(null, [arr[i], item])){
        arr.splice(i, 1);
        c=false;
      }
      else i=i+1;
    return!c;
  }
  constructor(i, _2, _3, _4){
    if(i=="New_3"){
      i="New_4";
      _2=[];
      _3=Equals;
      _4=Hash;
    }
    let init_2;
    if(i=="New_2"){
      init_2=_2;
      i="New_4";
      _2=init_2;
      _3=Equals;
      _4=Hash;
    }
    if(i=="New_4"){
      const init_3=_2;
      const equals=_3;
      const hash=_4;
      super();
      this.equals=equals;
      this.hash=hash;
      this.data=[];
      this.count=0;
      const e=Get(init_3);
      try {
        while(e.MoveNext())
          this.add(e.Current);
      }
      finally {
        const _5=e;
        if(typeof _5=="object"&&isIDisposable(_5))e.Dispose();
      }
    }
  }
}
function Some(Value){
  return{$:1, $0:Value};
}
class Exception extends Object_1 { }
class Builder {
  BFileName;
  BId;
  BKeywords;
  BRender;
  BTitle;
  Create(){
    const id=req("Id", op_PlusPlus(this.BId, this.BTitle));
    const x=op_PlusPlus(this.BTitle, this.BId);
    const title=x==null?"Sample":x.$0;
    return Sample.New(req("FileName", this.BFileName), id, this.BKeywords, req("Render", this.BRender), title);
  }
  Render(x){
    this.BRender=Some(x);
    return this;
  }
  Keywords(x){
    this.BKeywords=x;
    return this;
  }
  FileName(x){
    this.BFileName=Some(x);
    return this;
  }
  Id(x){
    this.BId=Some(x);
    return this;
  }
  static New(BFileName, BId, BKeywords, BRender, BTitle){
    return Create_1(Builder, {
      BFileName:BFileName, 
      BId:BId, 
      BKeywords:BKeywords, 
      BRender:BRender, 
      BTitle:BTitle
    });
  }
}
class T extends Object_1 {
  s;
  c;
  n;
  d;
  e;
  MoveNext(){
    const m=this.n(this);
    this.e=m?1:2;
    return m;
  }
  get Current(){
    return this.e===1?this.c:this.e===0?FailWith("Enumeration has not started. Call MoveNext."):FailWith("Enumeration already finished.");
  }
  Dispose(){
    if(this.d)this.d(this);
  }
  constructor(s, c, n, d){
    super();
    this.s=s;
    this.c=c;
    this.n=n;
    this.d=d;
    this.e=0;
  }
}
class Elt extends Doc {
  docNode_1;
  updates_1;
  elt;
  rvUpdates;
  static New(el, attr_1, children){
    const node=CreateElemNode(el, attr_1, children.docNode);
    const rvUpdates=Updates_1.Create(children.updates);
    return new Elt(ElemDoc(node), Map2Unit(Updates(node.Attr), rvUpdates.v), el, rvUpdates);
  }
  constructor(docNode, updates, elt, rvUpdates){
    super(docNode, updates);
    this.docNode_1=docNode;
    this.updates_1=updates;
    this.elt=elt;
    this.rvUpdates=rvUpdates;
  }
}
function Forever(Item){
  return{$:0, $0:Item};
}
function Waiting(Item1, Item2){
  return{
    $:3, 
    $0:Item1, 
    $1:Item2
  };
}
function Ready(Item1, Item2){
  return{
    $:2, 
    $0:Item1, 
    $1:Item2
  };
}
function New(DynElem, DynFlags, DynNodes, OnAfterRender){
  const _2={
    DynElem:DynElem, 
    DynFlags:DynFlags, 
    DynNodes:DynNodes
  };
  SetOptional(_2, "OnAfterRender", OnAfterRender);
  return _2;
}
function TryParse(s, r){
  return TryParse_2(s, -2147483648, 2147483647, r);
}
function Obsolete(sn){
  let _2;
  const m=sn.s;
  if(m==null||(m!=null&&m.$==2?(_2=m.$1,false):m!=null&&m.$==3?(_2=m.$1,false):true))void 0;
  else {
    sn.s=null;
    for(let i=0, _3=length(_2)-1;i<=_3;i++){
      const o=get(_2, i);
      if(typeof o=="object")(((sn_1) => {
        Obsolete(sn_1);
      })(o));
      else o();
    }
  }
}
class TemplateHole extends Object_1 { }
function notPresent(){
  throw new KeyNotFoundException("New");
}
function convertTextNode(n){
  let m, li;
  m=null;
  li=0;
  const s=n.textContent;
  const strRE=new RegExp(TextHoleRE(), "g");
  while(m=strRE.exec(s),m!==null)
    {
      n.parentNode.insertBefore(globalThis.document.createTextNode(string(s, Some(li), Some(strRE.lastIndex-get(m, 0).length-1))), n);
      li=strRE.lastIndex;
      const hole=globalThis.document.createElement("span");
      hole.setAttribute("ws-replace", get(m, 1).toLowerCase());
      n.parentNode.insertBefore(hole, n);
    }
  strRE.lastIndex=0;
  n.textContent=string(s, Some(li), null);
}
function failNotLoaded(name){
  console.warn("Instantiating non-loaded template", name);
}
function fillTextHole(instance, fillWith, templateName){
  const m=instance.querySelector("[ws-replace]");
  return Equals(m, null)?(console.warn("Filling non-existent text hole", templateName),null):(m.parentNode.replaceChild(globalThis.document.createTextNode(fillWith), m),Some(m.getAttribute("ws-replace")));
}
function removeHolesExcept(instance, dontRemove){
  const run=(attrName) => {
    foreachNotPreserved(instance, "["+attrName+"]", (e) => {
      if(!dontRemove.Contains(e.getAttribute(attrName)))e.removeAttribute(attrName);
    });
  };
  run("ws-attr");
  run("ws-onafterrender");
  run("ws-var");
  foreachNotPreserved(instance, "[ws-hole]", (e) => {
    if(!dontRemove.Contains(e.getAttribute("ws-hole"))){
      e.removeAttribute("ws-hole");
      while(e.hasChildNodes())
        e.removeChild(e.lastChild);
    }
  });
  foreachNotPreserved(instance, "[ws-replace]", (e) => {
    if(!dontRemove.Contains(e.getAttribute("ws-replace")))e.parentNode.removeChild(e);
  });
  foreachNotPreserved(instance, "[ws-on]", (e) => {
    e.setAttribute("ws-on", concat_2(" ", filter((x) => dontRemove.Contains(get(SplitChars(x, [":"], 1), 1)), SplitChars(e.getAttribute("ws-on"), [" "], 1))));
  });
  foreachNotPreserved(instance, "[ws-attr-holes]", (e) => {
    const holeAttrs=SplitChars(e.getAttribute("ws-attr-holes"), [" "], 1);
    for(let i=0, _3=holeAttrs.length-1;i<=_3;i++){
      const attrName=get(holeAttrs, i);
      let this_1=new RegExp(TextHoleRE(), "g");
      let str=e.getAttribute(attrName);
      let replaceFn=(_4, _5) => dontRemove.Contains(_5)?_4:"";
      let _2=str.replace(this_1, replaceFn);
      e.setAttribute(attrName, _2);
    }
  });
}
function fillInstanceAttrs(instance, fillWith){
  convertAttrs(fillWith);
  const name=fillWith.nodeName.toLowerCase();
  const m=instance.querySelector("[ws-attr="+name+"]");
  if(Equals(m, null))console.warn("Filling non-existent attr hole", name);
  else {
    m.removeAttribute("ws-attr");
    for(let i=0, _2=fillWith.attributes.length-1;i<=_2;i++){
      const a=fillWith.attributes.item(i);
      if(a.name=="class"&&m.hasAttribute("class"))m.setAttribute("class", m.getAttribute("class")+" "+a.nodeValue);
      else m.setAttribute(a.name, a.nodeValue);
    }
  }
}
function mapHoles(t, mappings){
  const run=(attrName) => {
    foreachNotPreserved(t, "["+attrName+"]", (e) => {
      let o;
      const m=(o=null,[mappings.TryGetValue(e.getAttribute(attrName).toLowerCase(), {get:() => o, set:(v) => {
        o=v;
      }}), o]);
      if(m[0])e.setAttribute(attrName, m[1]);
    });
  };
  run("ws-hole");
  run("ws-replace");
  run("ws-attr");
  run("ws-onafterrender");
  run("ws-var");
  foreachNotPreserved(t, "[ws-on]", (e) => {
    e.setAttribute("ws-on", concat_2(" ", map((x) => {
      let o;
      const a=SplitChars(x, [":"], 1);
      const m=(o=null,[mappings.TryGetValue(get(a, 1), {get:() => o, set:(v) => {
        o=v;
      }}), o]);
      return m[0]?get(a, 0)+":"+m[1]:x;
    }, SplitChars(e.getAttribute("ws-on"), [" "], 1))));
  });
  foreachNotPreserved(t, "[ws-attr-holes]", (e) => {
    const holeAttrs=SplitChars(e.getAttribute("ws-attr-holes"), [" "], 1);
    for(let i=0, _2=holeAttrs.length-1;i<=_2;i++)((() => {
      const attrName=get(holeAttrs, i);
      return e.setAttribute(attrName, fold((_3, _4) => {
        const a=KeyValue(_4);
        return _3.replace(new RegExp("\\${"+a[0]+"}", "ig"), "${"+a[1]+"}");
      }, e.getAttribute(attrName), mappings));
    })());
  });
}
function fill(fillWith, p, n){
  while(true)
    {
      if(fillWith.hasChildNodes())n=p.insertBefore(fillWith.lastChild, n);
      else return null;
    }
}
function convertAttrs(el){
  const attrs=el.attributes;
  const toRemove=[];
  const events=[];
  const holedAttrs=[];
  for(let i=0, _3=attrs.length-1;i<=_3;i++){
    const a=attrs.item(i);
    if(StartsWith(a.nodeName, "ws-on")&&a.nodeName!="ws-onafterrender"&&a.nodeName!="ws-on"){
      toRemove.push(a.nodeName);
      events.push(string(a.nodeName, Some("ws-on".length), null)+":"+a.nodeValue.toLowerCase());
    }
    else if(!StartsWith(a.nodeName, "ws-")&&(new RegExp(TextHoleRE())).test(a.nodeValue)){
      let this_1=new RegExp(TextHoleRE(), "g");
      let str=a.nodeValue;
      let replaceFn=(_4, _5) =>"${"+_5.toLowerCase()+"}";
      let _2=str.replace(this_1, replaceFn);
      a.nodeValue=_2;
      holedAttrs.push(a.nodeName);
    }
    else void 0;
  }
  if(!(events.length==0))el.setAttribute("ws-on", concat_2(" ", events));
  if(!(holedAttrs.length==0))el.setAttribute("ws-attr-holes", concat_2(" ", holedAttrs));
  const lowercaseAttr=(name) => {
    const m=el.getAttribute(name);
    if(m==null){ }
    else el.setAttribute(name, m.toLowerCase());
  };
  lowercaseAttr("ws-hole");
  lowercaseAttr("ws-replace");
  lowercaseAttr("ws-attr");
  lowercaseAttr("ws-onafterrender");
  lowercaseAttr("ws-var");
  iter((a_1) => {
    el.removeAttribute(a_1);
  }, toRemove);
}
function string(source, start, finish){
  if(start==null){
    if(finish!=null&&finish.$==1){
      const f=finish.$0;
      return f<0?"":source.slice(0, f+1);
    }
    else return"";
  }
  else if(finish==null)return source.slice(start.$0);
  else {
    const f_1=finish.$0;
    const s=start.$0;
    return f_1<0?"":source.slice(s, f_1+1);
  }
}
function insufficient(){
  return FailWith("The input sequence has an insufficient number of elements.");
}
function arrContains(item, arr){
  let c, i;
  c=true;
  i=0;
  const l=length(arr);
  while(c&&i<l)
    if(Equals(arr[i], item))c=false;
    else i=i+1;
  return!c;
}
function nonNegative(){
  return FailWith("The input must be non-negative.");
}
class KeyCollection extends Object_1 {
  d;
  GetEnumerator(){
    return Get(map_1((kvp) => kvp.K, this.d));
  }
  constructor(d){
    super();
    this.d=d;
  }
}
function D3_JsonAsync(url){
  return Defer((c) => {
    _1.json(url).then((data) => c([null, data]), (err) => c([err, null]));
  });
}
function D3_TsvAsync(url){
  return Defer((c) => {
    _1.tsv(url).then((data) => c([null, data]), (err) => c([err, null]));
  });
}
function Defer(job){
  return FromContinuations((ok, no) => job((t) => {
    if(t[0])no(new Error("Defer: failed"));
    else ok(t[1]);
  }));
}
function CreateElemNode(el, attr_1, children){
  LinkElement(el, children);
  const attr_2=Insert(el, attr_1);
  return DocElemNode.New(attr_2, children, null, el, Int(), GetOptional(attr_2.OnAfterRender));
}
function LinkPrevElement(el, children){
  InsertDoc(el.parentNode, children, el);
}
function CreateDelimitedRunState(ldelim, rdelim, doc){
  return New_1(get_Empty(), CreateDelimitedElemNode(ldelim, rdelim, EmptyAttr(), doc));
}
function PerformAnimatedUpdate(childrenOnly, st, doc){
  if(get_UseAnimations()){
    const f=() => {
      const cur=FindAll(doc);
      const change=ComputeChangeAnim(st, cur);
      const enter=ComputeEnterAnim(st, cur);
      const x=Play(Append(change, ComputeExitAnim(st, cur)));
      const f_1=() => {
        const x_1=SyncElemNodesNextFrame(childrenOnly, st);
        const f_2=() => {
          const x_2=Play(enter);
          const f_3=() => {
            st.PreviousNodes=cur;
            return Return(null);
          };
          return Bind(x_2, (_2, _3) =>(f_3(_2))(_3));
        };
        return Bind(x_1, (_2, _3) =>(f_2(_2))(_3));
      };
      return Bind(x, (_2, _3) =>(f_1(_2))(_3));
    };
    return Delay((_2, _3) =>(f(_2))(_3));
  }
  else return SyncElemNodesNextFrame(childrenOnly, st);
}
function PerformSyncUpdate(childrenOnly, st, doc){
  const cur=FindAll(doc);
  SyncElemNode(childrenOnly, st.Top);
  st.PreviousNodes=cur;
}
function LinkElement(el, children){
  InsertDoc(el, children, null);
}
function InsertDoc(parent, doc, pos){
  while(true)
    {
      if(doc!=null&&doc.$==1){
        const e=doc.$0;
        return InsertNode(parent, e.El, pos);
      }
      else if(doc!=null&&doc.$==2){
        const d=doc.$0;
        d.Dirty=false;
        doc=d.Current;
      }
      else if(doc==null)return pos;
      else if(doc!=null&&doc.$==4){
        const t=doc.$0;
        return InsertNode(parent, t.Text, pos);
      }
      else if(doc!=null&&doc.$==5){
        const t_1=doc.$0;
        return InsertNode(parent, t_1, pos);
      }
      else if(doc!=null&&doc.$==6)return foldBack((_2, _3) =>((((parent_1) =>(el) =>(pos_1) => el==null||el.constructor===Object?InsertDoc(parent_1, el, pos_1):InsertNode(parent_1, el, pos_1))(parent))(_2))(_3), doc.$0.Els, pos);
      else {
        const b=doc.$1;
        const a=doc.$0;
        doc=a;
        pos=InsertDoc(parent, b, pos);
      }
    }
}
function CreateDelimitedElemNode(ldelim, rdelim, attr_1, children){
  const el=ldelim.parentNode;
  LinkPrevElement(rdelim, children);
  const attr_2=Insert(el, attr_1);
  return DocElemNode.New(attr_2, children, Some([ldelim, rdelim]), el, Int(), GetOptional(attr_2.OnAfterRender));
}
function SyncElemNodesNextFrame(childrenOnly, st){
  if(BatchUpdatesEnabled()){
    const c=(ok) => {
      requestAnimationFrame(() => {
        SyncElemNode(childrenOnly, st.Top);
        ok();
      });
    };
    return FromContinuations((_2, _3, _4) => c.apply(null, [_2, _3, _4]));
  }
  else {
    SyncElemNode(childrenOnly, st.Top);
    return Return(null);
  }
}
function ComputeExitAnim(st, cur){
  return Concat(map((n) => GetExitAnim(n.Attr), ToArray(Except(cur, Filter((n) => HasExitAnim(n.Attr), st.PreviousNodes)))));
}
function ComputeEnterAnim(st, cur){
  return Concat(map((n) => GetEnterAnim(n.Attr), ToArray(Except(st.PreviousNodes, Filter((n) => HasEnterAnim(n.Attr), cur)))));
}
function ComputeChangeAnim(st, cur){
  const f=(n) => HasChangeAnim(n.Attr);
  const relevant=(a) => Filter(f, a);
  return Concat(map((n) => GetChangeAnim(n.Attr), ToArray(Intersect(relevant(st.PreviousNodes), relevant(cur)))));
}
function SyncElemNode(childrenOnly, el){
  !childrenOnly?SyncElement(el):void 0;
  Sync_1(el.Children);
  AfterRender(el);
}
function InsertNode(parent, node, pos){
  InsertAt(parent, pos, node);
  return node;
}
function SyncElement(el){
  function hasDirtyChildren(el_1){
    function dirty(doc){
      while(true)
        {
          if(doc!=null&&doc.$==0){
            const b=doc.$1;
            const a=doc.$0;
            if(dirty(a))return true;
            else doc=b;
          }
          else if(doc!=null&&doc.$==2){
            const d=doc.$0;
            if(d.Dirty)return true;
            else doc=d.Current;
          }
          else if(doc!=null&&doc.$==6){
            const t=doc.$0;
            return t.Dirty||exists(hasDirtyChildren, t.Holes);
          }
          else return false;
        }
    }
    return dirty(el_1.Children);
  }
  Sync(el.El, el.Attr);
  if(hasDirtyChildren(el))DoSyncElement(el);
}
function Sync_1(doc){
  while(true)
    {
      if(doc!=null&&doc.$==1)return SyncElemNode(false, doc.$0);
      else if(doc!=null&&doc.$==2){
        const n=doc.$0;
        doc=n.Current;
      }
      else if(doc==null)return null;
      else if(doc!=null&&doc.$==5)return null;
      else if(doc!=null&&doc.$==4){
        const d=doc.$0;
        return d.Dirty?(d.Text.nodeValue=d.Value,d.Dirty=false):null;
      }
      else if(doc!=null&&doc.$==6){
        const t=doc.$0;
        iter((h) => {
          SyncElemNode(false, h);
        }, t.Holes);
        iter((t_1) => {
          Sync(t_1[0], t_1[1]);
        }, t.Attrs);
        return AfterRender(t);
      }
      else {
        const b=doc.$1;
        const a=doc.$0;
        Sync_1(a);
        doc=b;
      }
    }
}
function AfterRender(el){
  const m=GetOptional(el.Render);
  if(m!=null&&m.$==1){
    m.$0(el.El);
    SetOptional(el, "Render", null);
  }
}
function DoSyncElement(el){
  const m=GetOptional(el.Delimiters);
  const parent=m==null?el.El:m.$0[1].parentNode;
  function ins(doc, pos){
    while(true)
      {
        if(doc!=null&&doc.$==1)return doc.$0.El;
        else if(doc!=null&&doc.$==2){
          const d=doc.$0;
          if(d.Dirty){
            d.Dirty=false;
            return InsertDoc(parent, d.Current, pos);
          }
          else doc=d.Current;
        }
        else if(doc==null)return pos;
        else if(doc!=null&&doc.$==4)return doc.$0.Text;
        else if(doc!=null&&doc.$==5)return doc.$0;
        else if(doc!=null&&doc.$==6){
          const t=doc.$0;
          if(t.Dirty)t.Dirty=false;
          return foldBack((_4, _5) => _4==null||_4.constructor===Object?ins(_4, _5):_4, t.Els, pos);
        }
        else {
          const b=doc.$1;
          const a=doc.$0;
          doc=a;
          pos=ins(b, pos);
        }
      }
  }
  Iter((e) => {
    RemoveNode(parent, e);
  }, Except_2(DocChildren(el), Children(parent, GetOptional(el.Delimiters))));
  let _2=el.Children;
  const m_1=GetOptional(el.Delimiters);
  let _3=m_1!=null&&m_1.$==1?m_1.$0[1]:null;
  ins(_2, _3);
}
class Updates_1 {
  c;
  s;
  v;
  static Create(v){
    let var_1;
    var_1=null;
    var_1=Updates_1.New(v, null, () => {
      let c;
      c=var_1.s;
      return c===null?(c=Copy(var_1.c()),var_1.s=c,WhenObsoleteRun(c, () => {
        var_1.s=null;
      }),c):c;
    });
    return var_1;
  }
  static New(Current, Snap, VarView){
    return Create_1(Updates_1, {
      c:Current, 
      s:Snap, 
      v:VarView
    });
  }
}
function concat_2(separator, strings){
  return ofSeq_1(strings).join(separator);
}
function SplitChars(s, sep, opts){
  return Split(s, new RegExp("["+RegexEscape(sep.join(""))+"]"), opts);
}
function StartsWith(t, s){
  return t.substring(0, s.length)==s;
}
function Split(s, pat, opts){
  return opts===1?filter((x) => x!=="", SplitWith(s, pat)):SplitWith(s, pat);
}
function RegexEscape(s){
  return s.replace(new RegExp("[-\\/\\\\^$*+?.()|[\\]{}]", "g"), "\\$&");
}
function SplitWith(str, pat){
  return str.split(pat);
}
function forall_2(f, s){
  return forall_1(f, protect(s));
}
function protect(s){
  return s==null?"":s;
}
function get_UseAnimations(){
  return UseAnimations();
}
function Play(anim){
  const f=() => {
    const x=Run(() => { }, Actions(anim));
    const f_1=() => {
      Finalize(anim);
      return Return(null);
    };
    return Bind(x, (_2, _3) =>(f_1(_2))(_3));
  };
  return Delay((_2, _3) =>(f(_2))(_3));
}
function Append(a, a_1){
  return Anim(Append_1(a.$0, a_1.$0));
}
function Run(k, anim){
  const dur=anim.Duration;
  if(dur===0)return Zero();
  else {
    const c=(ok) => {
      function loop(start){
        return(now) => {
          const t=now-start;
          anim.Compute(t);
          k();
          return t<=dur?void requestAnimationFrame((t_1) => {
            (loop(start))(t_1);
          }):ok();
        };
      }
      requestAnimationFrame((t) => {
        (loop(t))(t);
      });
    };
    return FromContinuations((_2, _3, _4) => c.apply(null, [_2, _3, _4]));
  }
}
function Anim(Item){
  return{$:0, $0:Item};
}
function Concat(xs){
  return Anim(Concat_1(map_1(List, xs)));
}
function BatchUpdatesEnabled(){
  return _c_6.BatchUpdatesEnabled;
}
function StartProcessor(procAsync){
  const st=[0];
  function work(){
    const f=() => {
      const f_1=() => {
        const m=st[0];
        return Equals(m, 1)?(st[0]=0,Zero()):Equals(m, 2)?(st[0]=1,work()):Zero();
      };
      return Bind(procAsync, (_2, _3) =>(f_1(_2))(_3));
    };
    return Delay((_2, _3) =>(f(_2))(_3));
  }
  return() => {
    const m=st[0];
    if(Equals(m, 0)){
      st[0]=1;
      Start(work(), null);
    }
    else Equals(m, 1)?st[0]=2:void 0;
  };
}
function enumUsing(x, f){
  return{GetEnumerator:() => {
    let enum_1;
    try {
      enum_1=Get(f(x));
    }
    catch(e){
      let c;
      c=x;
      c.Dispose();
      throw e;
    }
    return new T(null, null, (e_1) => enum_1.MoveNext()&&(e_1.c=enum_1.Current,true), () => {
      let c_1;
      enum_1.Dispose();
      c_1=x;
      c_1.Dispose();
    });
  }};
}
function enumWhile(f, s){
  return{GetEnumerator:() => {
    function next(en){
      while(true)
        {
          const m=en.s;
          if(Equals(m, null)){
            if(f()){
              en.s=Get(s);
              en=en;
            }
            else return false;
          }
          else if(m.MoveNext()){
            en.c=m.Current;
            return true;
          }
          else {
            m.Dispose();
            en.s=null;
            en=en;
          }
        }
    }
    return new T(null, null, next, (en) => {
      const x=en.s;
      if(!Equals(x, null))x.Dispose();
    });
  }};
}
function Delay(mk){
  return(c) => {
    try {
      mk(null, c);
    }
    catch(e){
      c.k(No(e));
    }
  };
}
function Bind(r, f){
  return checkCancel((c) => {
    r(New_2((a) => {
      if(a.$==0){
        const x=a.$0;
        scheduler().Fork(() => {
          try {
            f(x, c);
          }
          catch(e){
            c.k(No(e));
          }
        });
      }
      else scheduler().Fork(() => {
        c.k(a);
      });
    }, c.ct));
  });
}
function Return(x){
  return(c) => {
    c.k(Ok(x));
  };
}
function Start(c, ctOpt){
  const d=(defCTS())[0];
  const ct=ctOpt==null?d:ctOpt.$0;
  scheduler().Fork(() => {
    if(!ct.c)c(New_2((a) => {
      if(a.$==1)UncaughtAsyncError(a.$0);
    }, ct));
  });
}
function checkCancel(r){
  return(c) => {
    if(c.ct.c)cancel(c);
    else r(c);
  };
}
function defCTS(){
  return _c_8.defCTS;
}
function UncaughtAsyncError(e){
  console.log("WebSharper: Uncaught asynchronous exception", e);
}
function Zero(){
  return _c_8.Zero;
}
function cancel(c){
  c.k(Cc(new OperationCanceledException("New", c.ct)));
}
function scheduler(){
  return _c_8.scheduler;
}
function FromContinuations(subscribe){
  return(c) => {
    const continued=[false];
    const once=(cont) => {
      if(continued[0])FailWith("A continuation provided by Async.FromContinuations was invoked multiple times");
      else {
        continued[0]=true;
        scheduler().Fork(cont);
      }
    };
    subscribe((a) => {
      once(() => {
        c.k(Ok(a));
      });
    }, (e) => {
      once(() => {
        c.k(No(e));
      });
    }, (e) => {
      once(() => {
        c.k(Cc(e));
      });
    });
  };
}
function Int(){
  set_counter(counter()+1);
  return counter();
}
function set_counter(_2){
  _c_9.counter=_2;
}
function counter(){
  return _c_9.counter;
}
let _c_4=Lazy((_i) => class Client {
  static {
    _c_4=_i(this);
  }
  static FloatApplyChecked;
  static FloatGetChecked;
  static FloatSetChecked;
  static FloatApplyUnchecked;
  static FloatGetUnchecked;
  static FloatSetUnchecked;
  static IntApplyChecked;
  static IntGetChecked;
  static IntSetChecked;
  static IntApplyUnchecked;
  static IntGetUnchecked;
  static IntSetUnchecked;
  static FileApplyUnchecked;
  static FileGetUnchecked;
  static FileSetUnchecked;
  static DateTimeApplyUnchecked;
  static DateTimeGetUnchecked;
  static DateTimeSetUnchecked;
  static StringListApply;
  static StringListGet;
  static StringListSet;
  static StringApply;
  static StringGet;
  static StringSet;
  static BoolCheckedApply;
  static EmptyAttr;
  static {
    this.EmptyAttr=null;
    this.BoolCheckedApply=(var_1) =>[(el) => {
      el.addEventListener("change", () => var_1.Get()!=el.checked?var_1.Set(el.checked):null);
    }, (_2) =>(_3) => _3!=null&&_3.$==1?void(_2.checked=_3.$0):null, Map((V) => Some(V), var_1.View)];
    this.StringSet=(el) =>(s_8) => {
      el.value=s_8;
    };
    this.StringGet=(el) => Some(el.value);
    const g=StringGet();
    const s=StringSet();
    this.StringApply=(v) => ApplyValue(g, s, v);
    this.StringListSet=(el) =>(s_8) => {
      const options_=el.options;
      for(let i=0, _2=options_.length-1;i<=_2;i++)((() => {
        const option=options_.item(i);
        option.selected=arrContains(option.value, s_8);
      })());
    };
    this.StringListGet=(el) => {
      const selectedOptions=el.selectedOptions;
      return Some(ofSeq_1(delay(() => collect((i) =>[selectedOptions.item(i).value], range(0, selectedOptions.length-1)))));
    };
    const g_1=StringListGet();
    const s_1=StringListSet();
    this.StringListApply=(v) => ApplyValue(g_1, s_1, v);
    this.DateTimeSetUnchecked=(el) =>(i) => {
      el.value=(new Date(i)).toLocaleString();
    };
    this.DateTimeGetUnchecked=(el) => {
      let o, m;
      const s_8=el.value;
      if(isBlank(s_8))return Some(-8640000000000000);
      else {
        o=0;
        const m_1=TryParse_1(s_8);
        let _2=m_1!=null&&m_1.$==1&&(o=m_1.$0,true);
        m=[_2, o];
        return m[0]?Some(m[1]):null;
      }
    };
    const g_2=DateTimeGetUnchecked();
    const s_2=DateTimeSetUnchecked();
    this.DateTimeApplyUnchecked=(v) => ApplyValue(g_2, s_2, v);
    this.FileSetUnchecked=() =>() => null;
    this.FileGetUnchecked=(el) => {
      const files=el.files;
      return Some(ofSeq_1(delay(() => map_1((i) => files.item(i), range(0, files.length-1)))));
    };
    const g_3=FileGetUnchecked();
    const s_3=FileSetUnchecked();
    this.FileApplyUnchecked=(v) => FileApplyValue(g_3, s_3, v);
    this.IntSetUnchecked=(el) =>(i) => {
      el.value=String(i);
    };
    this.IntGetUnchecked=(el) => {
      const s_8=el.value;
      if(isBlank(s_8))return Some(0);
      else {
        const pd=+s_8;
        return pd!==pd>>0?null:Some(pd);
      }
    };
    const g_4=IntGetUnchecked();
    const s_4=IntSetUnchecked();
    this.IntApplyUnchecked=(v) => ApplyValue(g_4, s_4, v);
    this.IntSetChecked=(el) =>(i) => {
      const i_1=i.Input;
      return el.value!=i_1?void(el.value=i_1):null;
    };
    this.IntGetChecked=(el) => {
      let _2, o;
      const s_8=el.value;
      if(isBlank(s_8))_2=(el.checkValidity?el.checkValidity():true)?CheckedInput.Blank(s_8):CheckedInput.Invalid(s_8);
      else {
        const m=(o=0,[TryParse(s_8, {get:() => o, set:(v) => {
          o=v;
        }}), o]);
        _2=m[0]?CheckedInput.Valid(m[1], s_8):CheckedInput.Invalid(s_8);
      }
      return Some(_2);
    };
    const g_5=IntGetChecked();
    const s_5=IntSetChecked();
    this.IntApplyChecked=(v) => ApplyValue(g_5, s_5, v);
    this.FloatSetUnchecked=(el) =>(i) => {
      el.value=String(i);
    };
    this.FloatGetUnchecked=(el) => {
      const s_8=el.value;
      if(isBlank(s_8))return Some(0);
      else {
        const pd=+s_8;
        return isNaN(pd)?null:Some(pd);
      }
    };
    const g_6=FloatGetUnchecked();
    const s_6=FloatSetUnchecked();
    this.FloatApplyUnchecked=(v) => ApplyValue(g_6, s_6, v);
    this.FloatSetChecked=(el) =>(i) => {
      const i_1=i.Input;
      return el.value!=i_1?void(el.value=i_1):null;
    };
    this.FloatGetChecked=(el) => {
      let _2;
      const s_8=el.value;
      if(isBlank(s_8))_2=(el.checkValidity?el.checkValidity():true)?CheckedInput.Blank(s_8):CheckedInput.Invalid(s_8);
      else {
        const i=+s_8;
        _2=isNaN(i)?CheckedInput.Invalid(s_8):CheckedInput.Valid(i, s_8);
      }
      return Some(_2);
    };
    const g_7=FloatGetChecked();
    const s_7=FloatSetChecked();
    this.FloatApplyChecked=(v) => ApplyValue(g_7, s_7, v);
  }
});
let _c_5=Lazy((_i) => class $StartupCode_DomUtility {
  static {
    _c_5=_i(this);
  }
  static defaultWrap;
  static wrapMap;
  static rhtml;
  static rtagName;
  static rxhtmlTag;
  static {
    this.rxhtmlTag=new RegExp("<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\\w:]+)[^>]*)\\/>", "gi");
    this.rtagName=new RegExp("<([\\w:]+)");
    this.rhtml=new RegExp("<|&#?\\w+;");
    const table=[1, "<table>", "</table>"];
    let _2=Object.fromEntries([["option", [1, "<select multiple='multiple'>", "</select>"]], ["legend", [1, "<fieldset>", "</fieldset>"]], ["area", [1, "<map>", "</map>"]], ["param", [1, "<object>", "</object>"]], ["thead", table], ["tbody", table], ["tfoot", table], ["tr", [2, "<table><tbody>", "</tbody></table>"]], ["col", [2, "<table><colgroup>", "</colgoup></table>"]], ["td", [3, "<table><tbody><tr>", "</tr></tbody></table>"]]]);
    this.wrapMap=_2;
    this.defaultWrap=[0, "", ""];
  }
});
function New_1(PreviousNodes, Top){
  return{PreviousNodes:PreviousNodes, Top:Top};
}
function get_Empty(){
  return NodeSet(new HashSet("New_3"));
}
function FindAll(doc){
  const q=[];
  function recF(recI, _2){
    while(true)
      switch(recI){
        case 0:
          if(_2!=null&&_2.$==0){
            const b=_2.$1;
            const a=_2.$0;
            recF(0, a);
            _2=b;
          }
          else if(_2!=null&&_2.$==1){
            const el=_2.$0;
            _2=el;
            recI=1;
          }
          else if(_2!=null&&_2.$==2){
            const em=_2.$0;
            _2=em.Current;
          }
          else if(_2!=null&&_2.$==6){
            const x=_2.$0.Holes;
            return(((a_1) =>(a_2) => {
              iter(a_1, a_2);
            })(loopEN))(x);
          }
          else return null;
          break;
        case 1:
          q.push(_2);
          _2=_2.Children;
          recI=0;
          break;
      }
  }
  function loop(node){
    return recF(0, node);
  }
  function loopEN(el){
    return recF(1, el);
  }
  loop(doc);
  return NodeSet(new HashSet("New_2", q));
}
function NodeSet(Item){
  return{$:0, $0:Item};
}
function Filter(f, a){
  return NodeSet(Filter_1(f, a.$0));
}
function Except(a, a_1){
  return NodeSet(Except_1(a.$0, a_1.$0));
}
function ToArray(a){
  return ToArray_2(a.$0);
}
function Intersect(a, a_1){
  return NodeSet(Intersect_1(a.$0, a_1.$0));
}
function UseAnimations(){
  return _c_7.UseAnimations;
}
function Actions(a){
  return ConcatActions(choose((a_1) => a_1.$==1?Some(a_1.$0):null, ToArray_1(a.$0)));
}
function Finalize(a){
  iter((a_1) => {
    if(a_1.$==0)a_1.$0();
  }, ToArray_1(a.$0));
}
function ConcatActions(xs){
  const xs_1=ofSeqNonCopying(xs);
  const m=length(xs_1);
  if(m===0)return Const_1();
  else if(m===1)return get(xs_1, 0);
  else {
    const dur=max(map_1((anim) => anim.Duration, xs_1));
    const xs_2=map((x) => Prolong(dur, x), xs_1);
    return Def(dur, (t) => {
      iter((anim) => {
        anim.Compute(t);
      }, xs_2);
    });
  }
}
function List(a){
  return a.$0;
}
function Const_1(v){
  return Def(0, () => v);
}
function Def(d, f){
  return{Compute:f, Duration:d};
}
function Prolong(nextDuration, anim){
  const comp=anim.Compute;
  const dur=anim.Duration;
  const last=Create(() => anim.Compute(anim.Duration));
  return{Compute:(t) => t>=dur?last.f():comp(t), Duration:nextDuration};
}
let _c_6=Lazy((_i) => class Proxy {
  static {
    _c_6=_i(this);
  }
  static BatchUpdatesEnabled;
  static {
    this.BatchUpdatesEnabled=true;
  }
});
function New_2(k, ct){
  return{k:k, ct:ct};
}
function No(Item){
  return{$:1, $0:Item};
}
function Ok(Item){
  return{$:0, $0:Item};
}
function Cc(Item){
  return{$:2, $0:Item};
}
function New_3(IsCancellationRequested, Registrations){
  return{c:IsCancellationRequested, r:Registrations};
}
function concat_3(o){
  let r=[];
  let k;
  for(var k_1 in o)r.push.apply(r, o[k_1]);
  return r;
}
class Var extends Object_1 { }
function ApplyValue(get_1, set_1, var_1){
  let expectedValue;
  expectedValue=null;
  return[(el) => {
    const onChange=() => {
      var_1.UpdateMaybe((v) => {
        let _2;
        expectedValue=get_1(el);
        return expectedValue!=null&&expectedValue.$==1&&(!Equals(expectedValue.$0, v)&&(_2=[expectedValue, expectedValue.$0],true))?_2[0]:null;
      });
    };
    el.addEventListener("change", onChange);
    el.addEventListener("input", onChange);
    el.addEventListener("keypress", onChange);
  }, (x) => {
    const _2=set_1(x);
    return(_3) => _3==null?null:_2(_3.$0);
  }, Map((v) => {
    let _2;
    return expectedValue!=null&&expectedValue.$==1&&(Equals(expectedValue.$0, v)&&(_2=expectedValue.$0,true))?null:Some(v);
  }, var_1.View)];
}
function StringSet(){
  return _c_4.StringSet;
}
function StringGet(){
  return _c_4.StringGet;
}
function StringListSet(){
  return _c_4.StringListSet;
}
function StringListGet(){
  return _c_4.StringListGet;
}
function DateTimeSetUnchecked(){
  return _c_4.DateTimeSetUnchecked;
}
function DateTimeGetUnchecked(){
  return _c_4.DateTimeGetUnchecked;
}
function FileApplyValue(get_1, set_1, var_1){
  let expectedValue;
  expectedValue=null;
  return[(el) => {
    el.addEventListener("change", () => {
      var_1.UpdateMaybe((v) => {
        let _2;
        expectedValue=get_1(el);
        return expectedValue!=null&&expectedValue.$==1&&(expectedValue.$0!==v&&(_2=[expectedValue, expectedValue.$0],true))?_2[0]:null;
      });
    });
  }, (x) => {
    const _2=set_1(x);
    return(_3) => _3==null?null:_2(_3.$0);
  }, Map((v) => {
    let _2;
    return expectedValue!=null&&expectedValue.$==1&&(Equals(expectedValue.$0, v)&&(_2=expectedValue.$0,true))?null:Some(v);
  }, var_1.View)];
}
function FileSetUnchecked(){
  return _c_4.FileSetUnchecked;
}
function FileGetUnchecked(){
  return _c_4.FileGetUnchecked;
}
function IntSetUnchecked(){
  return _c_4.IntSetUnchecked;
}
function IntGetUnchecked(){
  return _c_4.IntGetUnchecked;
}
function IntSetChecked(){
  return _c_4.IntSetChecked;
}
function IntGetChecked(){
  return _c_4.IntGetChecked;
}
function FloatSetUnchecked(){
  return _c_4.FloatSetUnchecked;
}
function FloatGetUnchecked(){
  return _c_4.FloatGetUnchecked;
}
function FloatSetChecked(){
  return _c_4.FloatSetChecked;
}
function FloatGetChecked(){
  return _c_4.FloatGetChecked;
}
function isBlank(s){
  return forall_2(IsWhiteSpace, s);
}
class CheckedInput {
  get Input(){
    return this.$==1?this.$0:this.$==2?this.$0:this.$1;
  }
  static Blank(inputText){
    return Create_1(CheckedInput, {$:2, $0:inputText});
  }
  static Invalid(inputText){
    return Create_1(CheckedInput, {$:1, $0:inputText});
  }
  static Valid(value, inputText){
    return Create_1(CheckedInput, {
      $:0, 
      $0:value, 
      $1:inputText
    });
  }
  $;
  $0;
  $1;
}
function Clear_1(a){
  a.splice(0, length(a));
}
class KeyNotFoundException extends Error {
  constructor(i, _2){
    if(i=="New"){
      i="New_1";
      _2="The given key was not present in the dictionary.";
    }
    if(i=="New_1"){
      const message=_2;
      super(message);
    }
  }
}
let _c_7=Lazy((_i) => class $StartupCode_Animation {
  static {
    _c_7=_i(this);
  }
  static UseAnimations;
  static CubicInOut;
  static {
    this.CubicInOut=Easing.Custom((t) => {
      const t2=t*t;
      return 3*t2-2*(t2*t);
    });
    this.UseAnimations=true;
  }
});
function Append_1(x, y){
  return x.$==0?y:y.$==0?x:{
    $:2, 
    $0:x, 
    $1:y
  };
}
function ToArray_1(xs){
  const out=[];
  function loop(xs_1){
    while(true)
      {
        if(xs_1.$==1)return out.push(xs_1.$0);
        else if(xs_1.$==2){
          const y=xs_1.$1;
          const x=xs_1.$0;
          loop(x);
          xs_1=y;
        }
        else return xs_1.$==3?iter((v) => {
          out.push(v);
        }, xs_1.$0):null;
      }
  }
  loop(xs);
  return out.slice(0);
}
function Concat_1(xs){
  const x=ofSeqNonCopying(xs);
  return TreeReduce(Empty(), Append_1, x);
}
function Empty(){
  return _c_10.Empty;
}
class Scheduler extends Object_1 {
  idle;
  robin;
  Fork(action){
    this.robin.push(action);
    this.idle?(this.idle=false,setTimeout(() => {
      this.tick();
    }, 0)):void 0;
  }
  tick(){
    let loop;
    const t=Date.now();
    loop=true;
    while(loop)
      if(this.robin.length===0){
        this.idle=true;
        loop=false;
      }
      else {
        (this.robin.shift())();
        Date.now()-t>40?(setTimeout(() => {
          this.tick();
        }, 0),loop=false):void 0;
      }
  }
  constructor(){
    super();
    this.idle=true;
    this.robin=[];
  }
}
let _c_8=Lazy((_i) => class $StartupCode_Concurrency {
  static {
    _c_8=_i(this);
  }
  static GetCT;
  static Zero;
  static defCTS;
  static scheduler;
  static noneCT;
  static {
    this.noneCT=New_3(false, []);
    this.scheduler=new Scheduler();
    this.defCTS=[new CancellationTokenSource()];
    this.Zero=Return();
    this.GetCT=(c) => {
      c.k(Ok(c.ct));
    };
  }
});
let _c_9=Lazy((_i) => class $StartupCode_Abbrev {
  static {
    _c_9=_i(this);
  }
  static counter;
  static {
    this.counter=0;
  }
});
function IsWhiteSpace(c){
  return c.match(new RegExp("\\s"))!==null;
}
function TryParse_1(s){
  const d=Date.parse(s);
  return isNaN(d)?null:Some(d);
}
function TryParse_2(s, min, max_1, r){
  const x=+s;
  const ok=x===x-x%1&&x>=min&&x<=max_1;
  if(ok)r.set(x);
  return ok;
}
class Easing extends Object_1 {
  transformTime;
  static Custom(f){
    return new Easing(f);
  }
  constructor(transformTime){
    super();
    this.transformTime=transformTime;
  }
}
function Filter_1(ok, set_1){
  return new HashSet("New_2", filter(ok, ToArray_2(set_1)));
}
function Except_1(excluded, included){
  const set_1=new HashSet("New_2", ToArray_2(included));
  set_1.ExceptWith(ToArray_2(excluded));
  return set_1;
}
function ToArray_2(set_1){
  const arr=create(set_1.Count, void 0);
  set_1.CopyTo(arr, 0);
  return arr;
}
function Intersect_1(a, b){
  const set_1=new HashSet("New_2", ToArray_2(a));
  set_1.IntersectWith(ToArray_2(b));
  return set_1;
}
class OperationCanceledException extends Error {
  ct;
  constructor(i, _2, _3, _4){
    let ct;
    if(i=="New"){
      ct=_2;
      i="New_1";
      _2="The operation was canceled.";
      _3=null;
      _4=ct;
    }
    if(i=="New_1"){
      const message=_2;
      const inner=_3;
      const ct_1=_4;
      super(message);
      this.inner=inner;
      this.ct=ct_1;
    }
  }
}
class CancellationTokenSource extends Object_1 {
  init;
  c;
  pending;
  r;
  constructor(){
    super();
    this.c=false;
    this.pending=null;
    this.r=[];
    this.init=1;
  }
}
function Children(elem, delims){
  let n;
  if(delims!=null&&delims.$==1){
    const rdelim=delims.$0[1];
    const ldelim=delims.$0[0];
    const a=[];
    n=ldelim.nextSibling;
    while(n!==rdelim)
      {
        a.push(n);
        n=n.nextSibling;
      }
    return DomNodes(a);
  }
  else {
    let _2=elem.childNodes.length;
    const o=elem.childNodes;
    let _3=init(_2, (i) => o[i]);
    return DomNodes(_3);
  }
}
function Except_2(a, a_1){
  const excluded=a.$0;
  return DomNodes(filter((n) => forall((k) =>!(n===k), excluded), a_1.$0));
}
function Iter(f, a){
  iter(f, a.$0);
}
function DocChildren(node){
  const q=[];
  function loop(doc){
    while(true)
      {
        if(doc!=null&&doc.$==2){
          const d=doc.$0;
          doc=d.Current;
        }
        else if(doc!=null&&doc.$==1)return q.push(doc.$0.El);
        else if(doc==null)return null;
        else if(doc!=null&&doc.$==5)return q.push(doc.$0);
        else if(doc!=null&&doc.$==4)return q.push(doc.$0.Text);
        else if(doc!=null&&doc.$==6){
          const x=doc.$0.Els;
          return(((a_1) =>(a_2) => {
            iter(a_1, a_2);
          })((a_1) => {
            if(a_1==null||a_1.constructor===Object)loop(a_1);
            else q.push(a_1);
          }))(x);
        }
        else {
          const b=doc.$1;
          const a=doc.$0;
          loop(a);
          doc=b;
        }
      }
  }
  loop(node.Children);
  return DomNodes(ofSeqNonCopying(q));
}
function DomNodes(Item){
  return{$:0, $0:Item};
}
function Create(f){
  return New_4(false, f, forceLazy);
}
function forceLazy(){
  const v=this.v();
  this.c=true;
  this.v=v;
  this.f=cachedLazy;
  return v;
}
function cachedLazy(){
  return this.v;
}
let _c_10=Lazy((_i) => class $StartupCode_AppendList {
  static {
    _c_10=_i(this);
  }
  static Empty;
  static {
    this.Empty={$:0};
  }
});
function New_4(created, evalOrVal, force){
  return{
    c:created, 
    v:evalOrVal, 
    f:force
  };
}
Main();

