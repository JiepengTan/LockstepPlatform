#!/bin/bash
clear
dir="$(cd $(dirname ${BASH_SOURCE[0]}) && pwd)"
cd $dir/../
echo "1.building"
output="$(msbuild /property:Configuration=Debug /verbosity:minimal Server/Server.sln)"
echo "2.copy libs"
rm -rf Client/Assets/Plugins/Libs/*
rm -rf Server/Libs/*
cp -rf ./Libs/* Client/Assets/Plugins/Libs
cp -rf ./Libs/* Server/bin
echo "finish"
sleep 1