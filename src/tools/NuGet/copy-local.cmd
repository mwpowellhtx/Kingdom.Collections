@echo off

setlocal

:setvars
:: set local_dir=C:\Dev\NuGet\packages
set local_dir=G:\Dev\NuGet\local\packages
set nupkg_ext=.nupkg

:setconfig
set config=%1
if ("%config%" === "") (
    set config=Release
)

:: Do the main areas here.
pushd ..\..
:: pushd packages

:: This is an interesting package for local consumption, but I do not think
:: it should be published in a broader context, at least not yet.
call :cplocal Kingdom.CodeAnalysis.Verification

call :cplocal Kingdom.Collections.Enumerations.Attributes
call :cplocal Kingdom.Collections.Enumerations.Analyzers
call :cplocal Kingdom.Collections.Enumerations.Generators
call :cplocal Kingdom.Collections.Enumerations.BuildTime

call :cplocal Kingdom.CodeAnalysis.Verifiers.CodeFixes
call :cplocal Kingdom.CodeAnalysis.Verifiers.Diagnostics

call :cplocal Kingdom.Collections.ImmutableBitArray
call :cplocal Kingdom.Collections.Enumerations
call :cplocal Kingdom.Collections.Enumerations.Tests
call :cplocal Kingdom.Collections.Stacks
call :cplocal Kingdom.Collections.Queues
call :cplocal Kingdom.Collections.Deques

popd

goto :end

:cplocal
set f=%1
set package_path=%f%\bin\%config%\%f%.*%nupkg_ext%
::set package_path=%f%.*%nupkg_ext%
if not exist "%package_path%" (
    echo Package '%package_path%' not found.
)
:: We need to scan beyond the wildcard package path here and get to specifics.
for %%x in ("%package_path%") do (
    if not exist "%local_dir%" mkdir "%local_dir%"
    :: Make sure that we do not inadvertently overwrite already existing package versions.
    if not exist "%local_dir%\%%x" echo Copying '%%x' package to local directory '%local_dir%'...
    if not exist "%local_dir%\%%x" xcopy /Y "%%x" "%local_dir%"
)
exit /b

:end

endlocal

pause
