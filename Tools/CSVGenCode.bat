@echo off
cd ./Bin
CSVGenCode.exe -i=../excels -ocsv=../csv -obyte=../byte -ocode=../cscode -t=../TempleteFile/CSTemplete.txt
cd ..
::call CopyToProject.bat
::xcopy .\Config\CS\Output .\Test\CSVGenCode_CS\CSVGenCode_CS\Config /y