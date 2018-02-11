import os
from build_command import run_command

test_dlls = [
    "Test\\PodHead.UnitTest\\bin\\Release\\PodHead.UnitTest.dll",
    "Test\\PodHead.FunctionalTests\\bin\\Release\\PodHead.FunctionalTests.dll",
    "Test\\PodHead.IntegrationTests\\bin\\Release\\PodHead.IntegrationTests.dll"
]

def run_tests():
    if os.environ['RUNTESTS'] == "False":
        return
    run_command("packages\\NUnit.ConsoleRunner.3.8.0\\tools\\nunit3-console.exe --x86 --labels=All %s" % " ".join(test_dlls))

if __name__ == "__main__":
    run_tests()