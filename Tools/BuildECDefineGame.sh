#!/bin/bash
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
pwd
echo "Roslyn building ECDefine.Game"
output="$(msbuild /property:Configuration=Debug /p:WarningLevel=0 /verbosity:minimal ./Src/ECS.ECDefine.Game/ECS.ECDefine.Game.csproj)"
echo $output
