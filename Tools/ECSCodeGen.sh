#!/bin/bash
clear
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
pwd
echo "1.Code gen"
mono ./Src/bin/Lockstep.Tools.ECSGenerator.exe ../../Config/ECSGenerator/Config.json

