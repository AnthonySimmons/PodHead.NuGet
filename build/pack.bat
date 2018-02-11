SET PREV_DIR=%CD%
CD %~dp0\..

IF "%1" == "" (
    ECHO "Usage pack.bat <Version>"
    ECHO "Example: pack.bat 1.0.0.0"
    EXIT 1
)
SET VERSION=%1

nuget.exe pack PodHead\PodHead.csproj -Properties Configuration=Release -Version %VERSION%

SET RESULT=%ERRORLEVEL%
CD %PREV_DIR%
EXIT /B %RESULT%