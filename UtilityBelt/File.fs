//
// File.fs
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
open System.IO

module File =
    let (|FileExist|_|) item = 
        if File.Exists(item) then Some item
        else None

    let (|DirExist|_|) item =
        if Directory.Exists(item) then Some item
        else None
        
    let (|FileNotExist|_|) item = 
        if not (File.Exists(item)) then Some item
        else None

    let (|DirNotExist|_|) item =
        if not (Directory.Exists(item)) then Some item
        else None

    let parent (source: string) = Directory.GetParent(source.TrimEnd([|'/'|])).FullName
    let cp (source: string) (target: string) = File.Copy(source, target, false)
    let rm (source: string) = File.Delete(source)
    let mkdir (dir: string) = 
        if (Directory.Exists(parent dir)) then Directory.CreateDirectory(dir) |> ignore
        else failwithf "Cannot create %s, parent directory does not exist" dir
    let mkdir_p (dir: string) = Directory.CreateDirectory(dir) |> ignore

    let filesIn dir = Directory.GetFiles(dir) |> List.ofArray
    let filesInRec dir = Directory.GetFiles(dir, "*", SearchOption.AllDirectories) |> List.ofArray
    
    let filesWithPattern dir pattern = Directory.GetFiles(dir, pattern) |> List.ofArray
    let filesWithPatternRec dir pattern = 
        Directory.GetFiles(dir, pattern, SearchOption.AllDirectories) |> List.ofArray

    let dirsIn dir = Directory.GetDirectories(dir) |> List.ofArray
    let dirsInRec dir = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories) |> List.ofArray
    
    let dirsWithPattern dir pattern = Directory.GetDirectories(dir, pattern) |> List.ofArray
    let dirsWithPatternRec dir pattern = 
        Directory.GetDirectories(dir, pattern, SearchOption.AllDirectories) |> List.ofArray

    let rec GetFreeFile state (generator: int -> string) =
        let out = generator state
        if File.Exists(out) then (GetFreeFile (state + 1) generator)
        else out
        
    let rec GetFreeDir state (generator: int -> string) =
        let out = generator state
        if Directory.Exists(out) then (GetFreeDir (state + 1) generator)
        else out

    let rec firstOccurenceOfFile (file: string) (dir: string) : string option = 
        match (filesWithPattern dir file) with
        | [file] -> Some file
        | _ :: _ -> failwithf "Search returned multiple files with the same name, this is impossible."
        | [] -> List.tryPick (firstOccurenceOfFile file) (dirsIn dir)
        
    let inline (@@) basePath newPath = Path.Combine(basePath, newPath)
    