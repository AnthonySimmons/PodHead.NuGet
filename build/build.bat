SET PREV_DIR=%CD%
CD %~dp0\..

IF "%1" == "" (
    ECHO "Usage build.bat <Version>"
    ECHO "Example: build.bat 1.0.0.0"
    EXIT 1
)
SET VERSION=%1

CALL NuGet.exe restore PodHead.sln
CALL MSBuild.exe PodHead.sln /t:Clean;Build /p:AssemblyVersionNumber=%VERSION% /p:Configuration=Release /p:Platform=x86

SET RESULT=%ERRORLEVEL%
CD %PREV_DIR%
EXIT /B %RESULT%