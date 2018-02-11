SET PREV_DIR=%CD%
CD %~dp0\..

IF "%1" == "" (
    ECHO "Usage publish.bat <Version>"
    ECHO "Example: publish.bat 1.0.0.0"
    EXIT 1
)
SET VERSION=%1

nuget push PodHead.%VERSION%.nupkg -NonInteractive -Source https://api.nuget.org/v3/index.json

SET RESULT=%ERRORLEVEL%
CD %PREV_DIR%
EXIT /B %RESULT%