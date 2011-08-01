namespace NSemVer

open Mono.Cecil

type ChangeType = 
    | Added
    | Removed
    | Matched
    override this.ToString() = 
        match this with
        | Added -> "Added"
        | Removed -> "Removed"
        | Matched -> "Matched"

type ParameterChange = { Parameter: ParameterDefinition; PreviousParameter: ParameterDefinition; ChangeType: ChangeType; }
type MethodChange = { Method:MethodDefinition; PreviousMethod: MethodDefinition; ChangeType:ChangeType; ParameterChanges: seq<ParameterChange> }
type MethodGroupChange = { MethodName: string; ChangeType: ChangeType; MethodChanges: seq<MethodChange> }
type TypeChange = { Type: TypeDefinition; ChangeType: ChangeType; MethodGroupChanges: seq<MethodGroupChange> }
type ModuleChange = { Module: ModuleDefinition; ChangeType: ChangeType; TypeChanges: seq<TypeChange> }
type AssemblyChanges = { Older: AssemblyDefinition; Newer: AssemblyDefinition; ModuleChanges: seq<ModuleChange> }