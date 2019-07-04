#!/bin/bash
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
pwd
echo "Roslyn building CodeGenEntitas"
output="$(msbuild /property:Configuration=Debug /p:WarningLevel=3 /verbosity:minimal ../CodeGenEntitas/CodeGenEntitas.csproj)"
echo $output

