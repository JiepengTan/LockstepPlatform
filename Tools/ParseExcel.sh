#!/bin/bash
clear
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir
cd ./bin/
pwd
echo "ExcelParser"
mono Lockstep.Tools.ExcelParser.dll Tank.json