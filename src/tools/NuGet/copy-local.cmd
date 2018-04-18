@echo off

setlocal

:: set local_dir=C:\Dev\NuGet\packages
set local_dir=G:\Dev\NuGet\local\packages
set nupkg_ext=.nupkg

::
pushd packages

call :publishpackage Kingdom.Collections.ImmutableBitArray
call :publishpackage Kingdom.Collections.Enumerations
call :publishpackage Kingdom.Collections.Enumerations.Tests
call :publishpackage Kingdom.Collections.Stacks
call :publishpackage Kingdom.Collections.Queues
call :publishpackage Kingdom.Collections.Deques
popd

goto :end

:publishpackage
set f=%1
set package_path=%f%.*%nupkg_ext%
if not exist "%package_path%" (
    echo Package '%package_path%' not found.
)
:: We need to scan beyond the wildcard package path here and get to specifics.
for %%x in ("%package_path%") do (
    if not exist "%local_dir%" mkdir "%local_dir%"
    :: Make sure that we do not inadvertently overwrite already existing package versions.
    if not exist "%local_dir%\%%x" echo Moving '%%x' package to local directory '%local_dir%'...
    if not exist "%local_dir%\%%x" move /Y "%%x" "%local_dir%"
)
exit /b

:end

endlocal

pause
