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

open UtilityBelt.File

module Zip = 
#if NO_EXTERNAL_DOTNETZIP 
    open System.IO.Packaging
    
    let UnzipFile (path: string) (outputFolder: string) = failwith "UnzipFile requires SharpZipLib"
    let UnzipFileToTmp (path: string) = failwith "UnzipFileToTmp requires SharpZipLib"
    
    let ZipPath (zipfile: string) (path: string) = 
        use zip = Package.Open(zipfile, FileMode.CreateNew)
        for fpath in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories) do
            let fileUri = new Uri("/" + fpath.Replace(path, ""), UriKind.Relative)
            let packagePart = zip.CreatePart(fileUri, Net.Mime.MediaTypeNames.Application.Octet, CompressionOption.Fast)
            use stream = new FileStream(fpath, FileMode.Open, FileAccess.Read)
            stream.CopyTo(packagePart.GetStream())
        zip.Close()  
#else
    open ICSharpCode.SharpZipLib.Core
    open ICSharpCode.SharpZipLib.Zip
    
    let UnzipFile (path: string) (outputFolder: string) = 
        let zipName = Path.GetFileNameWithoutExtension(path)
        mkdir_p outputFolder
        
        use zip = new ZipFile(path)
        for zipEntry in zip do
            let zipEntry = zipEntry :?> ZipEntry // -_-'
            if zipEntry.IsDirectory then
                if not (Directory.Exists(outputFolder @@ zipEntry.Name)) then
                    mkdir (outputFolder @@ zipEntry.Name)
            else
                use stream = new FileStream(outputFolder @@ zipEntry.Name, FileMode.CreateNew, FileAccess.Write)
                (zip.GetInputStream(zipEntry).CopyTo(stream))
        zip.IsStreamOwner <- true
        zip.Close()
        
        // Did the zip also include an single dir root directory (zip name?)
        if Directory.GetFileSystemEntries(outputFolder).Length = 1
           && zipName.ToLower() = Directory.GetDirectories(outputFolder).[0].ToLower() then
            Directory.GetDirectories(outputFolder).[0]
        else
            outputFolder

    let UnzipFileToTmp (path: string) = 
        UnzipFile path (Path.GetTempPath() @@ (Guid.NewGuid()).ToString())
    
    let ZipPath (zipfile: string) (path: string) = 
        use zip = ZipFile.Create(zipfile)
        zip.AddDirectory(path)
        zip.IsStreamOwner <- true
        zip.Close()
        
        path
#endif 
