IF NOT EXIST "%1Merged" mkdir "%1Merged"
"%1..\..\..\Libs\ILMerge\ILMerge.exe" /lib:"C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v4.0" /closed /targetplatform:v4,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /out:"%1Merged\NSemVer.dll" NSemVer.dll NSemVer.Core.dll Mono.Cecil.dll
del "%1NSemVer.dll"
del "%1NSemVer.pdb"
move "%1Merged\NSemVer.dll" "%1NSemVer.dll"
move "%1Merged\NSemVer.pdb" "%1NSemVer.pdb"