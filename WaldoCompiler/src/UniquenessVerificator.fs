﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniquenessVerificator.fs" company="Oswald Maskens">
//   Copyright 2014 Oswald Maskens
//   
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
module OCA.WaldoCompiler.UniquenessVerificator

open OCA.AsmLib
open OCA.WaldoCompiler
open OCA.WaldoCompiler.Parser
open OFuncLib

let verify (source : Tree) : GenericAttempt<Tree, Positioned<string>> = 
    let rec verify (list : Tree) acc errors = 
        if list.IsEmpty then errors
        else if acc |> Set.contains (list.Head.name) then 
            verify list.Tail acc (errors |> Set.add (Positioned(list.Head.name, list.Head.position)))
        else verify list.Tail (acc |> Set.add list.Head.name) errors
    
    let errors = verify source Set.empty Set.empty
    if errors.IsEmpty then Ok source
    else 
        Fail(errors
             |> Set.toList
             |> List.map (Positioned.map (fun name -> sprintf "%s is defined more than once" name)))
