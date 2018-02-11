IF "%1" == "" (
    ECHO "Usage tag.bat <Version>"
    ECHO "Example: tag.bat 1.0.0.0"
    EXIT 1
)
SET VERSION=%1

git tag %VERSION%
git push --tags