SET PREV_DIR=%CD%
CD %~dp0\..

IF "%VERSION%" == "" (SET VERSION=%1)

CALL NuGet.exe restore PodHead.sln
CALL packages\python.3.6.1\tools\python.exe build\build.py %VERSION%

SET RESULT=%ERRORLEVEL%
CD %PREV_DIR%
EXIT /B %RESULT%