namespace NSemVer

open Mono.Cecil

type ChangeType = 
    | Added
    | Removed
    | Modified
    | Unchanged
    override this.ToString() = 
        match this with
        | Added -> "Added"
        | Removed -> "Removed"
        | Modified -> "Modified"
        | Unchanged -> "Unchanged"

type ParameterChange = { Parameter: ParameterDefinition; ChangeType: ChangeType; }
type MethodChange = { Method:MethodDefinition; ChangeType:ChangeType; ParameterChanges: seq<ParameterChange> }
type TypeChange = { Type: TypeDefinition; ChangeType: ChangeType; MethodChanges: seq<MethodChange> }
type ModuleChange = { Module: ModuleDefinition; ChangeType: ChangeType; TypeChanges: seq<TypeChange> }
type AssemblyChanges = { Older: AssemblyDefinition; Newer: AssemblyDefinition; ModuleChanges: seq<ModuleChange> }