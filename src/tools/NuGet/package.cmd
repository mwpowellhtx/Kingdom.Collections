@echo off

setlocal

set packages_path=tools\NuGet\packages
set nuget_exe=NuGet.exe
set csproj_ext=.csproj
set nupkg_ext=.nupkg

pushd ..\..

call :packproject Kingdom.Collections.ImmutableBitArray
call :packproject Kingdom.Collections.Enumerations
call :packproject Kingdom.Collections.Enumerations.Tests

popd

goto :end

:packproject
set f=%1
set csproj_path=%f%\%f%%csproj_ext%
set package_path=%f%.*%nupkg_ext%
if exist "%csproj_path%" (
    %nuget_exe% pack "%csproj_path%" -Build -Properties Configuration=Release
)
if exist "%package_path%" (
    if not exist "%packages_path%" mkdir "%packages_path%"
    move /Y "%package_path%" "%packages_path%"
)
exit /b

:end

endlocal

pause
