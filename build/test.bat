SET PREV_DIR=%CD%
CD %~dp0\..

packages\NUnit.ConsoleRunner.3.8.0\tools\nunit3-console.exe ^
--x86 ^
--labels=All ^
Test\PodHead.UnitTest\bin\Release\PodHead.UnitTest.dll ^
Test\PodHead.FunctionalTests\bin\Release\PodHead.FunctionalTests.dll ^
Test\PodHead.IntegrationTests\bin\Release\PodHead.IntegrationTests.dll

SET RESULT=%ERRORLEVEL%
CD %PREV_DIR%
EXIT /B %RESULT%