
@echo off
setlocal EnableDelayedExpansion

set "rootFolder=%~dp0"
set "rootFolder=%rootFolder:~0,-1%"
for %%i in ("%rootFolder%") do set "rootFolder=%%~dpi"

set "subFolders="

for /d %%f in ("%rootFolder%*") do (
    set "subFolders=!subFolders! %%~nxf"
)

set "subFolders=%subFolders:~1%"

for %%f in (%subFolders%) do (
    echo.
    echo Updating %%~nf folder...
    cd /d "%rootFolder%%%~f"
    cm partial update
    echo.
)

echo All folders have been updated.
pause
