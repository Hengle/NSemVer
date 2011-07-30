namespace NSemVer

open ChangeBuilderModule
open Mono.Cecil

type public ChangeBuilder() = 
    member x.GetChanges (older:AssemblyDefinition) (newer:AssemblyDefinition) = 
        calcAssemblyChanges older newer
        