module ChangeBuilderModule

open NSemVer
open Common
open Mono.Cecil

let createFixedParameterChange (c:ChangeType) (p:ParameterDefinition) =
    { Parameter = p; ChangeType = c }

let createFixedMethodChange (c:ChangeType) (m:MethodDefinition) =
    { Method = m; ChangeType = c; ParameterChanges = m.Parameters |> Seq.map (createFixedParameterChange c) }

let createFixedTypeChange (c:ChangeType) (t:TypeDefinition) =
    { Type = t; ChangeType = c; MethodChanges = t.Methods |> Seq.map (createFixedMethodChange c) }

let createFixedModuleChange (c:ChangeType) (m:ModuleDefinition) =
    { Module = m; ChangeType = c; TypeChanges = m.Types |> Seq.map (createFixedTypeChange c) }

let calcMethodChanges (older:MethodDefinition) (newer:MethodDefinition) = 
    let paramChanges = calcChanges (older.Parameters) (newer.Parameters) (fun x y -> x.Name = y.Name (* && TODO: check default values / out / ref etc *) ) 
    {
        Method = newer;
        ChangeType = ChangeType.Unchanged;
        ParameterChanges = 
            (paramChanges.Added |> Seq.map (createFixedParameterChange ChangeType.Added))
            |> Seq.append (paramChanges.Removed |> Seq.map (createFixedParameterChange ChangeType.Removed))
            |> Seq.append (paramChanges.Matched |> Seq.map (fun matched -> (createFixedParameterChange ChangeType.Unchanged matched.New)))
    }

let calcTypeChanges (older:TypeDefinition) (newer:TypeDefinition) =
    let paramTypeComparer (older:ParameterDefinition) (newer:ParameterDefinition) = older.ParameterType.FullName.CompareTo(newer.ParameterType.FullName)
    let methodCheck (older:MethodDefinition) (newer:MethodDefinition) =
        older.Name = newer.Name &&
        older.ReturnType.FullName = newer.ReturnType.FullName && // <<< TODO: Figure out why type void <> void
        (Seq.compareWith paramTypeComparer older.Parameters newer.Parameters = 0)
    let methodChanges = calcChanges older.Methods newer.Methods methodCheck
    {
        Type = newer;
        ChangeType = ChangeType.Unchanged;
        MethodChanges =
            (methodChanges.Added |> Seq.map (createFixedMethodChange ChangeType.Added))
            |> Seq.append (methodChanges.Removed |> Seq.map (createFixedMethodChange ChangeType.Removed))
            |> Seq.append (methodChanges.Matched |> Seq.map (fun matched -> (calcMethodChanges matched.Old matched.New)))
    }

let calcModuleChanges (older:ModuleDefinition) (newer:ModuleDefinition) =
    let getTypes (m:ModuleDefinition) = m.Types |> Seq.filter (fun x -> x.Name <> "<Module>")
    let typeChanges = calcChanges (getTypes(older)) (getTypes(newer)) (fun x y -> x.FullName = y.FullName)    
    {
        Module = newer;
        ChangeType = ChangeType.Unchanged;
        TypeChanges =
            (typeChanges.Added |> Seq.map (createFixedTypeChange ChangeType.Added))
            |> Seq.append (typeChanges.Removed |> Seq.map (createFixedTypeChange ChangeType.Removed))
            |> Seq.append (typeChanges.Matched |> Seq.map (fun matched -> (calcTypeChanges matched.Old matched.New)))
    }

let calcAssemblyChanges (older:AssemblyDefinition) (newer:AssemblyDefinition) = 
    let moduleChanges = calcChanges (older.Modules) (newer.Modules) (fun x y -> x.Name = y.Name)
    { 
        Older = older;
        Newer = newer;
        ModuleChanges =
            (moduleChanges.Added |> Seq.map (createFixedModuleChange ChangeType.Added))
            |> Seq.append (moduleChanges.Removed |> Seq.map (createFixedModuleChange ChangeType.Removed))
            |> Seq.append (moduleChanges.Matched |> Seq.map (fun matched -> (calcModuleChanges matched.Old matched.New)))
    }

