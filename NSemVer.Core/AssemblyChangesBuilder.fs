namespace NSemVer

open ChangeBuilderModule
open Mono.Cecil

type public AssemblyChangesBuilder() = 
    member x.GetChanges (older:AssemblyDefinition) (newer:AssemblyDefinition) = 
        calcAssemblyChanges older newer
