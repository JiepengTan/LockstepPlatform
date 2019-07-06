#!/bin/bash
#!/bin/bash
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
pwd
echo " ------------Build LPEngine ...-------------"
msbuild /property:Configuration=Debug /t:Rebuild /p:WarningLevel=0 /verbosity:minimal LPEngine.sln
echo " ------------Build LockstepPlatform ...-------------"
msbuild /property:Configuration=Debug /t:Rebuild /p:WarningLevel=0 /verbosity:minimal LockstepPlatform.sln
echo "Done "