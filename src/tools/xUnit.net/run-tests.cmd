@echo off

setlocal

rem TODO: TBD: do not keep this particular comment...
rem TODO: TBD: apply my learnings concerning :: vs rem comments
rem TODO: TBD: also apply my learnings re: for loop iteration over a range/list of delimited items
rem TODO: TBD: apply this to my packaging scripts as well...
rem TODO: TBD: the R# xUnit testing saga is as of this moment still unresolved
rem TODO: TBD: specifically concerning treatment of user furnished xUnit settings
rem TODO: TBD: working to move past this using Visual Studio Test Explorer, console runner, etc
rem TODO: TBD: although it would be very good to have that capability in sound working order from the jetbrains folks sooner than later

rem Projects are Comma (,) delimited.
set delim=,
set projects=
set xunit_opts=

:parseargs

rem rem For debugging...
rem echo %%1 = "%1"

if "%1" == "" (
    goto :endargs
)

if "%1" == "--config" (
    shift
    rem echo config %2
    set config=%2
    goto :nextarg
)

if "%1" == "--project" (
    shift
    rem echo project %2
    if "%projects%" == "" (
        set projects=%2
    ) else (
        set projects=%projects%%delim%%2
    )
    goto :nextarg
)

rem Now handle some xUnit.net specific options.
if "%1" == "--verbose" (
    rem echo verbose
    if "%xunit_opts%" == "" (
        set xunit_opts=-verbose
    ) else (
        set xunit_opts=%xunit_opts% -verbose
    )
    goto :nextarg
)

if "%1" == "--parallel" (
    rem echo parallel %2
    set parallel_opts=none%delim%collections%delim%assemblies%delim%all
:next_parallel
    for /f "tokens=1* delims=%delim%" %%p in ("%parallel_opts%") do (
        if "%2" == "%%p" (
            if "%xunit_opts%" == "" (
                set xunit_opts=-parallel %2
            ) else (
                set xunit_opts=%xunit_opts% -parallel %2
            )
            set parallel_opts=
        ) else (
            set parallel_opts=%%q
        )
    )
    if defined parallel_opts (
        goto :next_parallel
    )
    shift
    goto :nextarg
)

if "%1" == "--xml" (
    shift
    rem echo xml %2
    if "%xunit_opts%" == "" (
        set xunit_opts=-xml %2.xml
    ) else (
        set xunit_opts=%xunit_opts% -xml %2.xml
    )
    goto :nextarg
)

if "%1" == "--stop-on-failure" (
    rem echo stop-on-failure
    if "%xunit_opts%" == "" (
        set xunit_opts=-stoponfail
    ) else (
        set xunit_opts=%xunit_opts% -stoponfail
    )
    goto :nextarg
)

if "%1" == "--fail-skips" (
    rem echo fail-skips
    if "%xunit_opts%" == "" (
        set xunit_opts=-failskips
    ) else (
        set xunit_opts=%xunit_opts% -failskips
    )
    goto :nextarg
)

:nextarg

shift

goto :parseargs

:endargs

rem TODO: TBD: may make these input arguments...

set xunit_console_version=2.3.1
set xunit_bin_ext=.exe
set xunit_bin_dir=packages\xunit.runner.console.%xunit_console_version%\tools\net452
rem rem That's correct, there is no Prefix in this instance. Just take the Bin as-is.
set xunit_prefix=

rem set xunit_bin_dll=.dll
rem set xunit_bin_dir=packages\xunit.runner.console.%xunit_console_version%\tools\netcoreapp2.0
rem set xunit_prefix=dotnet run 

set xunit_bin=%xunit_bin_dir%\xunit.console%xunit_bin_ext%

rem Do the main areas here.
:testprojects

pushd ..\..

rem Projects will have already been set, appended, etc.
:nextproject
if not "%projects%" == "" (
    for /f "tokens=1* delims=%delim%" %%p in ("%projects%") do (
        rem Processing now as a function of input arguments.
        call :run %%p
        set projects=%%q
    )
)
if defined projects (
    goto :nextproject
)

popd

goto :end

:run
pushd %1\bin\%config%
rem Re-set this one every time.
set extensions=exe%delim%dll
:nextext
rem Iterate the possible Extensions here.
for /f "tokens=1* delims=%delim%" %%e in ("%extensions%") do (
    set extensions=%%f
    rem Then test for existence.
    if exist "%1.%%e" (
        echo Invoking console tests for "%1.%%e" ...
        echo %xunit_prefix%"..\..\..\%xunit_bin%" "%1.%%e" %xunit_opts%
        %xunit_prefix%"..\..\..\%xunit_bin%" "%1.%%e" %xunit_opts%
        if errorlevel 0 (
            echo All tests passed.
        ) else if errorlevel 1 (
            echo One or more tests failed.
        ) else if errorlevel 3 (
            echo Error with one or more xUnit arguments: %xunit_prefix% %xunit_bin% "%1.%%e".
        ) else if errorlevel 4 (
            echo Problem loading one or more assemblies during test: "%1\bin\%config%\%1.%%e".
        ) else if errorlevel -1073741510 (
            echo User canceled tests.
        )
    ) else (
        echo Test artifact %1\bin\%config%\%1.%%e not found or has not yet been built.
    )
)
if defined extensions (
    goto :nextext
)
popd
exit /b

goto :end

:end

endlocal
