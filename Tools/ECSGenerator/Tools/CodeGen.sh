#!/bin/bash
clear
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
cd ../bin/
pwd
echo "1.Code gen"
mono ../bin/Lockstep.Tools.ECSGenerator.dll

