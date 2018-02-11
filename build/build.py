import os
from build_command import run_command
from run_tests import run_tests

prev_dir = ''

def run_nuget():
	run_command("nuget.exe restore PodHead.sln")


def run_msbuild():
	run_command("msbuild.exe PodHead.sln /t:Clean;Build /p:Configuration=Release;Platform=x86")


def run_build():
	prev_dir = os.getcwd()
	script_dir = os.path.dirname(os.path.realpath(__file__))
	sln_dir = os.path.join(script_dir, "..")
	try:
		os.chdir(sln_dir)
		run_nuget()
		run_msbuild()
		run_tests()
	finally:
		os.chdir(prev_dir)


if __name__ == "__main__":
	run_build()