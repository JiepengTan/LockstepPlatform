#!/bin/bash

function printInfo {
	echo " ------------LockstepPlatform Setup ($_idx/$_count)$1 ...-------------"
	_idx=$[_idx + 1]
}
_idx=1
_count=8

echo " ------------ Welcome to LockstepPlatform !!-------------"
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
echo $_count
printInfo "Copy Unity's dlls"
_projectDir="$(pwd)"
cd ../LockstepPlatform/Libs/
_lpLibsDir="$(pwd)"
echo "_lpLibsDir: "$_lpLibsDir
rm -rf ./*Unity*
rm -rf ./Client/
rm -rf ./Server/
rm -rf ./LPEngine/
rm -rf ./Tools/
output="$(ps -ef | grep "Unity.app" |grep -v grep |awk '{print $8}' | sed -n '1,1 p')"
_relDir="Unity.app"
_unityDir="$(echo ${output%%Unity.app*}${_relDir})"
echo "_unityDir: "$_unityDir

cd $_unityDir
cd ./Contents/UnityExtensions/Unity/GUISystem
cp -rf ./*UI.* $_lpLibsDir
cd ../../../Managed/
cp -rf ./UnityEditor.dl* ./UnityEngine.* $_lpLibsDir
cd $_projectDir

printInfo "Build LPEngine "
msbuild /property:Configuration=Debug /p:WarningLevel=0 /verbosity:minimal ../LockstepPlatform/LPEngine.sln
printInfo "Build LockstepPlatform "
msbuild /property:Configuration=Debug /p:WarningLevel=0 /verbosity:minimal ../LockstepPlatform/LockstepPlatform.sln

printInfo "Copy libs "
mkdir -p Libs
cp -rf ../LockstepPlatform/Libs/ ./Libs/

printInfo "Copy Tools "
mkdir -p ./Tools/bin/
mkdir -p ./Tools/Src/
cp -rf ../LockstepPlatform/Tools/bin/ ./Tools/bin/
cp -rf ../LockstepPlatform/Tools/Config/ ./Tools/Config/
cp -rf ../LockstepPlatform/Tools/Src/*ECS* ./Tools/Src/
cp -rf ../LockstepPlatform/Tools/Build*.sh ./Tools/

printInfo "Build LPGame "
msbuild /property:Configuration=Debug /p:WarningLevel=0 /verbosity:minimal ./LPGame.sln

printInfo "Prepare Client CopySources "
cd Src/Unity
pwd
#./CopySources.sh

printInfo "Prepare Client GenConfig "
#./GenConfig.sh

echo "Setup done :)"
sleep 3