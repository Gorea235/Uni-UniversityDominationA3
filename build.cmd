@echo off
echo Building project...
"C:\Program Files\Unity\Editor\Unity.exe" -batchmode -runEditorTests -logFile "Build/build.log" -quit -executeMethod ProjectBuilder.BuildProject
echo Project Built!
pause