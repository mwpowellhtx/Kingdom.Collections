@echo off

setlocal

set xcopy_exe=xcopy.exe
set local_nuget_packages_dir=C:\Dev\NuGet\packages

for %%f in (..\..\Kingdom.Collections.ImmutableBitArray\*.nupkg) do %xcopy_exe% /Y /F "%%f" "%local_nuget_packages_dir%"

endlocal

pause
