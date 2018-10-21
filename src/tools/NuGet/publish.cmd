@echo off

setlocal

rem We do not publish the API key as part of the script itself.
if "%my_nuget_api_key%"=="" (
    echo You are prohibited from making these sorts of changes.
    goto :end
)

rem Default list delimiter is Comma (,).
:redefine_delim
if not defined delim (
    set delim=,
)
rem Redefine the delimiter when a Dot (.) is discovered.
rem Anticipates potentially accepting Delimiter as a command line arg.
if "%delim%" == "." (
    set delim=
    goto :redefine_delim
)

rem Go ahead and pre-seed the Projects up front.
:set_all_projects
set all_projects=
rem 1)
set all_projects=Kingdom.Collections.Enumerations.Attributes
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations.Analyzers
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations.Generators
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations.BuildTime
rem 2)
set all_projects=%all_projects%%delim%Kingdom.CodeAnalysis.Verifiers.CodeFixes
set all_projects=%all_projects%%delim%Kingdom.CodeAnalysis.Verifiers.Diagnostics
rem 3)
set all_projects=%all_projects%%delim%Kingdom.Collections.ImmutableBitArray
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations
set all_projects=%all_projects%%delim%Kingdom.Collections.Enumerations.Tests
rem 4)
set all_projects=%all_projects%%delim%Kingdom.Collections.Stacks
set all_projects=%all_projects%%delim%Kingdom.Collections.Queues
set all_projects=%all_projects%%delim%Kingdom.Collections.Deques

:parse_args

rem Done parsing the args.
if "%1" == "" (
    goto :end_args
)

:set_config
if "%1" == "--config" (
    set config=%2
    shift
    goto :next_arg
)

:add_project
if "%1" == "--project" (
    rem Add a Project to the Projects list.
    if not defined projects (
        set projects=%2
    ) else (
        set projects=%projects%%delim%%2
    )
    shift
    goto :next_arg
)

rem Prepare to publish All Projects.
:all_projects
if "%1" == "--all" (
    set projects=%all_projects%
    goto :next_arg
)

:next_arg

shift

goto :parse_args

:end_args

:verify_args

:verify_config
if ("%config%" == "") (
    rem Assumes Release Configuration when not otherwise specified.
    set config=Release
)

:publish_projects

:set_vars
rem Expecting NuGet to be found in the System Path.
set nuget_exe=NuGet.exe
set nuget_push_verbosity=detailed
set nuget_push_source=https://api.nuget.org/v3/index.json

set nuget_push_opts=%nuget_push_opts% %my_nuget_api_key%
set nuget_push_opts=%nuget_push_opts% -Verbosity %nuget_push_verbosity%
set nuget_push_opts=%nuget_push_opts% -NonInteractive
set nuget_push_opts=%nuget_push_opts% -Source %nuget_push_source%

rem Do the main areas here.
pushd ..\..

:next_project
if defined projects (
    for /f "tokens=1* delims=%delim%" %%p in ("%projects%") do (
        if not "%%p" == "" (
            call :publish_pkg %%p
        )
        set projects=%%q
        goto :next_project
    )
)

popd

goto :end

:publish_pkg
for %%f in ("%1\bin\%config%\%1.*.nupkg") do (
    %nuget_exe% push "%%f" %nuget_push_opts%
)
exit /b

:end

endlocal

pause
