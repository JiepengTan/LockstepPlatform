#!/bin/bash
clear
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
echo "1.building"
rm -rf Bins
output="$(msbuild /property:Configuration=Debug /verbosity:minimal Entitas.sln)"
echo "2.copy libs"
mkdir Bins
cp -Rf Addons/*/bin/Debug/ Bins 
cd Bins
rm UnityEditor.dll
rm UnityEngine.dll
rm Entitas.Migration.CLI.exe
mkdir Editor
mv -f Desp*.dll Editor
mv -f *Editor.dll Editor
mv -f Entitas.Migration.dll Editor

mv -f Editor/Desp*Logging.dll .
mv -f Editor/Desp*Utils.dll .

