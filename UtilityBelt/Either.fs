//
// Either.fs
//
// Author:
//       Ashley Towns <ashleyis@me.com>
//
// Copyright (c) 2013 Ashley Towns
//
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
        
module Either =
    type Either<'Left, 'Right> = 
        | Left of 'Left
        | Right of 'Right
        
        member x.isLeft = 
            match x with
            | Left _ -> true
            | Right _ -> false
        
        member x.isRight = not x.isLeft
        
        /// Converts an either to an Option<'T> where 'T is the value of 'Right
        /// 'Left is dropped and converted to None
        member x.toOption : Option<'Right> = 
            match x with
            | Left _ -> None
            | Right x -> Some x
        
        /// If Either is left aligned, replace value with leftValue
        /// otherwise return 'Right
        member x.defaultArg leftValue = 
            match x with
            | Left _ -> leftValue
            | Right value -> value
        
        /// Get the value, if right otherwise throws an exception
        member x.get : 'Right = 
            match x with
            | Left _ -> failwithf "%A expected to be right" x
            | Right value -> value
        
        /// map f: 'Right -> 'U, if left aligned rewrap left and return
        static member map (f : 'Right -> 'U) (eit : Either<'Left, 'Right>) : Either<'Left, 'U> = 
            match eit with
            | Left x -> Left x
            | Right x -> Right(f x)
        static member lift (f : 'Right -> 'U) (eit : Either<'Left, 'Right>) = Either.map f eit
        
        /// apply function on Right otherwise return left, function is expected
        /// to return an Either with the same type as 'Left
        static member bind (f : 'Right -> Either<'Left, 'U>) (eit : Either<'Left, 'Right>) : Either<'Left, 'U> = 
            match eit with
            | Right x -> f x
            | Left x -> Left x
        
        /// Iterate over right value, if left ignore. returns unit.
        static member iter (f : 'Right -> unit) (eit : Either<'Left, 'Right>) : unit = 
            match eit with
            | Left y -> ()
            | Right y -> (f y |> ignore)
            
        static member (>>=) (item, f) = Either.bind f item
        static member (<.) (item, f) = Either.lift f item
        