//
// XElement.fs
//
// Author:
//       Ashley Towns <ashleyis@me.com>
//
// Copyright (c) 2013 Ashley Towns
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace UtilityBelt
open System
open System.Xml
open System.Xml.Linq

module XDocument = 
    let MapAttributes (xs: seq<XAttribute>) = 
        [ for attr in xs do yield (attr.Name.LocalName, attr.Value) ] |> Map.ofList
    
    let TryPickAttribute (xs: seq<XAttribute>) attr = 
        xs 
        |> Seq.map(fun x -> (x.Name.LocalName, x.Value))
        |> Seq.tryFind(fun (name, _) -> name = attr)
        |> Option.map snd

    let PickAttribute (xs: seq<XAttribute>) attr =  
        try
            xs 
            |> Seq.map(fun x -> (x.Name.LocalName, x.Value))
            |> Seq.find(fun (name, _) -> name = attr)
            |> snd
        with
        | :? Collections.Generic.KeyNotFoundException as ex -> failwithf "%A does not exist in %A" attr xs

    let PickElements (ele: XContainer) term = ele.Elements(term)
    let PickElement (ele: XContainer) term = ele.Element(term)
    let Elements (ele: XContainer) = ele.Elements()

    let inline xname name = XName.Get(name)
    let inline (.>|) (ele: XElement) attr = PickAttribute (ele.Attributes()) attr
    let inline (?>|) (ele: XElement) attr = TryPickAttribute (ele.Attributes()) attr
    let inline (.||) (ele: XContainer) term = PickElements ele (xname term)
    let inline (.|) (ele: XContainer) term = PickElement ele (xname term)
    
    let (|XAttribute|_|) name (ele: XElement) = 
        ele ?>| name
        
    let (|XElement|_|) name (ele: XContainer) = 
        let value = ele .| name
        if value = null then None else Some value
        
    let (|XElements|_|) name (ele: XContainer) = 
        let vals = ele .|| name
        if vals = null then None else Some vals
