#!/bin/bash
#!/bin/bash
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
pwd
echo " ------------Build LPEngine ...-------------"
msbuild /property:Configuration=Debug /t:Clean /p:WarningLevel=0 /verbosity:minimal LPEngine.sln
echo " ------------Build LockstepPlatform ...-------------"
msbuild /property:Configuration=Debug /t:Clean /p:WarningLevel=0 /verbosity:minimal LockstepPlatform.sln
echo " ------------CopyLibs ...-------------"