IF "%1" == "" (
    ECHO "Usage tag.bat <Version>"
    ECHO "Example: tag.bat 1.0.0.0"
    EXIT 1
)
SET VERSION=%1

git tag %VERSION%
IF %ERRORLEVEL% NEQ 0 EXIT 1
git push --tags