@echo off

setlocal

:setconfig
set config=%1
if ("%config%" === "") (
    set config=Release
)

:: Do the main areas here.
pushd ..\..

call :cleanpkgs Kingdom.Collections.ImmutableBitArray
call :cleanpkgs Kingdom.Collections.Enumerations
call :cleanpkgs Kingdom.Collections.Enumerations.Tests
call :cleanpkgs Kingdom.Collections.Stacks
call :cleanpkgs Kingdom.Collections.Queues
call :cleanpkgs Kingdom.Collections.Deques

popd

goto :end

:cleanpkgs
set f=%1
for %%f in ("%f%\bin\%config%\%f%.*.nupkg") do (
    del /q "%%f"
    echo "%%f" cleaned...
)
exit /b

:end

endlocal

pause
