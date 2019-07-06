#!/bin/bash
clear
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir/../
ps -ef | grep "Test.Server.exe" | grep -v grep |awk '{print $2}' | xargs kill -9
mono bin/Test.Server.exe
