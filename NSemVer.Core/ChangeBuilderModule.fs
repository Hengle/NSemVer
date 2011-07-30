module ChangeBuilderModule

open NSemVer
open Common
open Mono.Cecil

let createParameterChange (c:ChangeType) (p:ParameterDefinition) =
    { Parameter = p; ChangeType = c }

let createMethodChange (c:ChangeType) (m:MethodDefinition) =
    { Method = m; ChangeType = c; ParameterChanges = m.Parameters |> Seq.map (createParameterChange c) }

let createTypeChange (c:ChangeType) (t:TypeDefinition) =
    { Type = t; ChangeType = c; MethodChanges = t.Methods |> Seq.map (createMethodChange c) }

let createModuleChange (c:ChangeType) (m:ModuleDefinition) =
    { Module = m; ChangeType = c; TypeChanges = m.Types |> Seq.map (createTypeChange c) }

let calcMethodChanges (older:MethodDefinition) (newer:MethodDefinition) = 
    let paramChanges = calcChanges (older.Parameters) (newer.Parameters) (fun x y -> x.Name = y.Name (* && TODO: check default values / out / ref etc *) ) 
    {
        Method = newer;
        ChangeType = ChangeType.Unchanged;
        ParameterChanges =
            (paramChanges.Matched |> Seq.map (fun matched -> (createParameterChange ChangeType.Unchanged matched.New))) 
            |> Seq.append (paramChanges.Removed |> Seq.map (createParameterChange ChangeType.Removed))
            |> Seq.append (paramChanges.Added |> Seq.map (createParameterChange ChangeType.Added))
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
            (methodChanges.Matched |> Seq.map (fun matched -> (calcMethodChanges matched.Old matched.New)))
            |> Seq.append (methodChanges.Removed |> Seq.map (createMethodChange ChangeType.Removed))
            |> Seq.append (methodChanges.Added |> Seq.map (createMethodChange ChangeType.Added))
    }

let calcModuleChanges (older:ModuleDefinition) (newer:ModuleDefinition) =
    let getTypes (m:ModuleDefinition) = m.Types |> Seq.filter (fun x -> x.Name <> "<Module>")
    let typeChanges = calcChanges (getTypes(older)) (getTypes(newer)) (fun x y -> x.FullName = y.FullName)    
    {
        Module = newer;
        ChangeType = ChangeType.Unchanged;
        TypeChanges =
            (typeChanges.Matched |> Seq.map (fun matched -> (calcTypeChanges matched.Old matched.New)))
            |> Seq.append (typeChanges.Added |> Seq.map (createTypeChange ChangeType.Added))
            |> Seq.append (typeChanges.Removed |> Seq.map (createTypeChange ChangeType.Removed))
    }

let calcAssemblyChanges (older:AssemblyDefinition) (newer:AssemblyDefinition) = 
    let moduleChanges = calcChanges (older.Modules) (newer.Modules) (fun x y -> x.Name = y.Name)
    { 
        Older = older;
        Newer = newer;
        ModuleChanges =
            (moduleChanges.Matched |> Seq.map (fun matched -> (calcModuleChanges matched.Old matched.New)))
            |> Seq.append (moduleChanges.Added |> Seq.map (createModuleChange ChangeType.Added))
            |> Seq.append (moduleChanges.Removed |> Seq.map (createModuleChange ChangeType.Removed))
    }

