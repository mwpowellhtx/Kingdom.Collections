@echo off

setlocal

rem TODO: TBD: the R# xUnit testing saga is as of this moment still unresolved
rem TODO: TBD: specifically concerning treatment of user furnished xUnit settings
rem TODO: TBD: working to move past this using Visual Studio Test Explorer, console runner, etc
rem TODO: TBD: although it would be very good to have that capability in sound working order from the jetbrains folks sooner than later

:set_vars
rem set local_dir=C:\Dev\NuGet\packages
set local_dir=G:\Dev\NuGet\local\packages
set nupkg_ext=.nupkg

rem Prepare for delimited list of Projects.
if not defined delim (
    set delim=,
)

:set_projects
set projects=
rem Setup All Projects
set all_projects=Kingdom.Collections.ImmutableBitArray
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations.Attributes
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations.Analyzers
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations.Generators
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations.BuildTime
set all_projects=%all_projects%%delim%Kingdom.CodeAnalysis.Verification
set all_projects=%all_projects%%delim%Kingdom.CodeAnalysis.Verifiers.CodeFixes
set all_projects=%all_projects%%delim%Kingdom.CodeAnalysis.Verifiers.Diagnostics
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations.Tests
set all_projects=%all_projects%%delim%Kingdom.Collections.Stacks
set all_projects=%all_projects%%delim%Kingdom.Collections.Queues
set all_projects=%all_projects%%delim%Kingdom.Collections.Deques
rem Setup Enumerations Projects
set enum_projects=Kingdom.Collections.ImmutableBitArray
set enum_projects=%enum_projects%%delim%Kingdom.Collections.Enumerations.Attributes
set enum_projects=%enum_projects%%delim%Kingdom.Collections.Enumerations.Analyzers
set enum_projects=%enum_projects%%delim%Kingdom.Collections.Enumerations.Generators
set enum_projects=%enum_projects%%delim%Kingdom.Collections.Enumerations.BuildTime
set enum_projects=%enum_projects%%delim%Kingdom.CodeAnalysis.Verification
set enum_projects=%enum_projects%%delim%Kingdom.CodeAnalysis.Verifiers.CodeFixes
set enum_projects=%enum_projects%%delim%Kingdom.CodeAnalysis.Verifiers.Diagnostics
set enum_projects=%enum_projects%%delim%Kingdom.Collections.Enumerations
set enum_projects=%enum_projects%%delim%Kingdom.Collections.Enumerations.Tests
rem Setup Bit Array
set bit_array_projects=Kingdom.Collections.ImmutableBitArray

:parse_args

if "%1" == "" (
    goto :end_args
)

:set_config
if "%1" == "--config" (
    set config=%2
    shift
    goto :next_args
)

:add_project
if "%1" == "--project" (
    if not defined projects (
        set projects=%2
    ) else (
        set projects=%projects%%delim%%2
    )
    shift
    goto :next_args
)

:add_all_projects
if "%1" == "--all" (
    set projects=%all_projects%
	goto :next_args
)

:add_enum_projects
if "%1" == "--enums" (
    if not defined projects (
        set projects=%enum_projects%
    ) else (
        set projects=%projects%%delim%%enum_projects%
    )
	goto :next_args
)
if "%1" == "--enumerations" (
    if not defined projects (
        set projects=%enum_projects%
    ) else (
        set projects=%projects%%delim%%enum_projects%
    )
	goto :next_args
)

:add_bit_array_projects
if "%1" == "--bits" (
    if not defined projects (
        set projects=%bit_array_projects%
    ) else (
        set projects=%projects%%delim%%bit_array_projects%
    )
	goto :next_args
)
if "%1" == "--bit-array" (
    if not defined projects (
        set projects=%bit_array_projects%
    ) else (
        set projects=%projects%%delim%%bit_array_projects%
    )
	goto :next_args
)

:next_args
shift

goto :parse_args

:end_args

:verify_args

:verify_projects
if not defined projects (
    rem In which case, there is nothing else to do.
    goto :end
)

:verify_config
if not defined config (
    set config=Release
)

:process_projects
rem Do the main areas here.
pushd ..\..

rem This is an interesting package for local consumption, but I do not think
rem it should be published in a broader context, at least not yet.
:next_project
if defined projects (
    for /f "tokens=1* delims=%delim%" %%p in ("%projects%") do (
        rem Processing now as a function of input arguments.
        call :copy_local %%p
        set projects=%%q
    )
    goto :next_project
)

popd

goto :end

:copy_local
pushd %1\bin\%config%
set package_path=%1.*%nupkg_ext%
rem set package_path=%1.*%nupkg_ext%
if not exist "%package_path%" (
    echo Package '%package_path%' not found.
)
rem We need to scan beyond the wildcard package path here and get to specifics.
for %%x in ("%package_path%") do (
    if not exist "%local_dir%" mkdir "%local_dir%"
    rem Make sure that we do not inadvertently overwrite already existing package versions.
    if not exist "%local_dir%\%%x" echo Copying '%%x' package to local directory '%local_dir%'...
    if not exist "%local_dir%\%%x" xcopy /Y "%%x" "%local_dir%"
)
popd
exit /b

:end

endlocal

pause
