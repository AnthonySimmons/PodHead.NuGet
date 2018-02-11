SET PREV_DIR=%CD%
CD %~dp0\..

IF "%1" == "" (
    ECHO "Usage build_test_publish.bat <Version>"
    ECHO "Example: build_test_publish.bat 1.0.0.0"
    EXIT 1
)
SET VERSION=%1

CALL build\build.bat %VERSION%
IF %ERRORLEVEL% NEQ 0 EXIT 1
CALL build\test.bat
IF %ERRORLEVEL% NEQ 0 EXIT 1
CALL build\pack.bat %VERSION%
IF %ERRORLEVEL% NEQ 0 EXIT 1
CALL build\tag.bat %VERSION%
IF %ERRORLEVEL% NEQ 0 EXIT 1
CALL build\publish.bat

SET RESULT=%ERRORLEVEL%
CD %PREV_DIR%
EXIT /B %RESULT%