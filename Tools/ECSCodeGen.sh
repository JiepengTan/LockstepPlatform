#!/bin/bash
clear
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
pwd
cd ./bin/
echo "1.Code gen"
mono ./Lockstep.Tools.ECSGenerator.exe ../Config/ECSGenerator/Config.json

echo "2.Complile generated code"
msbuild /property:Configuration=Debug /p:WarningLevel=0 /verbosity:minimal ../Src/ECS.ECSOutput/ECS.ECSOutput.csproj
