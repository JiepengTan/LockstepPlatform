#!/bin/bash
echo " ------------LockstepPlatform Setup -------------"
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
pwd
echo " ------------(1/5)Build LPEngine ...-------------"
msbuild /property:Configuration=Debug /p:WarningLevel=0 /verbosity:minimal ../LockstepPlatform/LPEngine.sln
echo " ------------(2/5)Build LockstepPlatform ...-------------"
msbuild /property:Configuration=Debug /p:WarningLevel=0 /verbosity:minimal ../LockstepPlatform/LockstepPlatform.sln

echo " ------------(3/5)Copy libs  ...-------------"
mkdir -p Libs
cp -rf ../LockstepPlatform/Libs/ ./Libs/

echo " ------------(4/5)Copy Tools  ...-------------"
mkdir -p ./Tools/bin/
mkdir -p ./Tools/Src/
cp -rf ../LockstepPlatform/Tools/bin/ ./Tools/bin/
cp -rf ../LockstepPlatform/Tools/Config/ ./Tools/Config/
cp -rf ../LockstepPlatform/Tools/Src/*ECS* ./Tools/Src/
cp -rf ../LockstepPlatform/Tools/Build*.sh ./Tools/

echo " ------------(5/5)Build LPGame ...-------------"
msbuild /property:Configuration=Debug /p:WarningLevel=0 /verbosity:minimal ./LPGame.sln

echo "Setup done :)"
sleep 3