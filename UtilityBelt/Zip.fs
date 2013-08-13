//
// Zip.fs
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
open System.IO
open System.IO.Packaging

module Zip = 
    let ZipPath (zipfile : string) (path : string) = 
        use zip = Package.Open(zipfile, FileMode.CreateNew)
        for fpath in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories) do
            let fileUri = new Uri("/" + fpath.Replace(path, ""), UriKind.Relative)
            let packagePart = zip.CreatePart(fileUri, Net.Mime.MediaTypeNames.Application.Octet, CompressionOption.Fast)
            use stream = new FileStream(fpath, FileMode.Open, FileAccess.Read)
            stream.CopyTo(packagePart.GetStream())
        zip.Close()