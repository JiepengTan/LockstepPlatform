#!/bin/bash
clear
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
pwd
cd ./bin/
echo "1.Msg Code gen"
mono ./Lockstep.Tools.CodeGenerator.dll ../Config/CodeGenerator/Config.json