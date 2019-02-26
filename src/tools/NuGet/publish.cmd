@echo off

setlocal

:set_vars
set nuget_api_key=%MY_NUGET_API_KEY%

rem We do not publish the API key as part of the script itself.
if "%nuget_api_key%" == "" (
    echo You are prohibited from making these sorts of changes.
    goto :end
)

rem Default list delimiter is Comma (,).
:redefine_delim
if "%delim%" == "" (
    set delim=,
)
rem Redefine the delimiter when a Dot (.) is discovered.
rem Anticipates potentially accepting Delimiter as a command line arg.
if "%delim%" == "." (
    set delim=
    goto :redefine_delim
)

rem Go ahead and pre-seed the Projects up front.
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
set all_projects=%all_projects%%delim%Kingdom.Collections.Bidirectionals
set all_projects=%all_projects%%delim%Kingdom.Collections.Stacks
set all_projects=%all_projects%%delim%Kingdom.Collections.Queues
set all_projects=%all_projects%%delim%Kingdom.Collections.Deques
rem Setup Collections Projects
set collections_projects=Kingdom.Collections.Bidirectionals
set collections_projects=%collections_projects%%delim%Kingdom.Collections.Stacks
set collections_projects=%collections_projects%%delim%Kingdom.Collections.Queues
set collections_projects=%collections_projects%%delim%Kingdom.Collections.Deques
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
rem Setup Bit Array Projects
set bit_array_projects=Kingdom.Collections.ImmutableBitArray
rem Setup Bidirectional Projects
set bidi_projects=Kingdom.Collections.Bidirectionals


:parse_args

if "%1" == "--nuget" (
    set destination=nuget
    goto :next_arg
)
if "%1" == "--local" (
    set destination=local
    goto :next_arg
)

:set_dry_run
if "%1" == "--dry" (
    set dry=true
    goto :next_arg
)
if "%1" == "--dry-run" (
    set dry=true
    goto :next_arg
)
if "%1" == "--wet" (
    set dry=false
    goto :next_arg
)
if "%1" == "--wet-run" (
    set dry=false
    goto :next_arg
)

:set_config
if "%1" == "--config" (
    set config=%2
    shift
    goto :next_arg
)

:add_enum_projects
if "%1" == "--enums" (
    rem Prepare to publish Enumerations Projects.
    if "%projects%" == "" (
        set projects=%enum_projects%
    ) else (
        set projects=%projects%%delim%%enum_projects%
    )
    goto :next_arg
)

if "%1" == "--enumerations" (
    rem Prepare to publish Enumerations Projects.
    if "%projects%" == "" (
        set projects=%enum_projects%
    ) else (
        set projects=%projects%%delim%%enum_projects%
    )
    goto :next_arg
)

:add_bit_array_projects
if "%1" == "--bits" (
    rem Prepare to publish Bit Array Projects.
    if "%projects%" == "" (
        set projects=%bit_array_projects%
    ) else (
        set projects=%projects%%delim%%bit_array_projects%
    )
    goto :next_arg
)

if "%1" == "--bit-array" (
    rem Prepare to publish Bit Array Projects.
    if "%projects%" == "" (
        set projects=%bit_array_projects%
    ) else (
        set projects=%projects%%delim%%bit_array_projects%
    )
    goto :next_arg
)

:add_bidi_projects
if "%1" == "--bidi" (
    rem Prepare to publish Bidirectional Projects.
    if "%projects%" == "" (
        set projects=%bidi_projects%
    ) else (
        set projects=%projects%%delim%%bidi_projects%
    )
    goto :next_arg
)

if "%1" == "--bidirectional" (
    rem Prepare to publish Bidirectional Projects.
    if "%projects%" == "" (
        set projects=%bidi_projects%
    ) else (
        set projects=%projects%%delim%%bidi_projects%
    )
    goto :next_arg
)

:add_collections_projects
if "%1" == "--collections" (
    rem Prepare to publish Collections Projects.
    if "%projects%" == "" (
        set projects=%collections_projects%
    ) else (
        set projects=%projects%%delim%%collections_projects%
    )
    goto :next_arg
)

:add_all_projects
if "%1" == "--all" (
    rem Prepare to publish All Projects.
    set projects=%all_projects%
    goto :next_arg
)

:add_project
if "%1" == "--project" (
    rem Add a Project to the Projects list.
    if "%projects%" == "" (
        set projects=%2
    ) else (
        set projects=%projects%%delim%%2
    )
    shift
    goto :next_arg
)

:next_arg

shift

if "%1" == "" goto :end_args

goto :parse_args

:end_args

:verify_args

:verify_dry
rem Assumes we want a Live (Wet) Run when unspecified.
if "%dry%" == "" set dry=false

:verify_destination
if "%destination%" == "" set destination=local

:verify_config
rem Assumes Release Configuration when not otherwise specified.
if "%config%" == "" set config=Release

:publish_projects

:set_vars

set xcopy_exe=xcopy.exe
set nuget_exe=NuGet.exe

set nupkg_ext=.nupkg
set publish_local_dir=G:\Dev\NuGet\local\packages

rem Expecting NuGet to be found in the System Path.
set nuget_api_src=https://api.nuget.org/v3/index.json
set nuget_push_verbosity=detailed

set nuget_push_opts=%nuget_push_opts% %nuget_api_key%
set nuget_push_opts=%nuget_push_opts% -Verbosity %nuget_push_verbosity%
set nuget_push_opts=%nuget_push_opts% -NonInteractive
set nuget_push_opts=%nuget_push_opts% -Source %nuget_api_src%

rem Do the main areas here.
pushd ..\..

if not "%projects%" == "" (
    echo Processing '%config%' configuration for '%projects%' ...
)
:next_project
if not "%projects%" == "" (
    for /f "tokens=1* delims=%delim%" %%p in ("%projects%") do (
        call :publish_pkg %%p
        set projects=%%q
        goto :next_project
    )
)

popd

goto :end

:publish_pkg
for %%f in ("%1\bin\%config%\%1*%nupkg_ext%") do (

    if "%destination%-%dry%" == "local-true" (
        echo Set to copy "%%f" to "%publish_local_dir%".
    )

    if "%destination%-%dry%" == "local-false" (
        if not exist "%publish_local_dir%" mkdir "%publish_local_dir%"
        echo Copying "%%f" package to local directory "%publish_local_dir%" ...
        %xcopy_exe% /q /y "%%f" "%publish_local_dir%"
    )

    if "%destination%-%dry%" == "nuget-true" (
        echo Dry run: %nuget_exe% push "%%f"%nuget_push_opts%
    )

    if "%destination%-%dry%" == "nuget-false" (
        echo Running: %nuget_exe% push "%%f"%nuget_push_opts%
        %nuget_exe% push "%%f"%nuget_push_opts%
    )
)
exit /b

:end

endlocal

pause
