import os
import sys
from build_command import run_command
from run_tests import run_tests

prev_dir = ''

def run_msbuild(version):
	run_command("msbuild.exe PodHead.sln /t:Clean;Build /p:AssemblyVersionNumber=%s /p:Configuration=Release /p:Platform=x86 " % version)


def run_build(version):
	prev_dir = os.getcwd()
	script_dir = os.path.dirname(os.path.realpath(__file__))
	sln_dir = os.path.join(script_dir, "..")
	try:
		os.chdir(sln_dir)
		run_msbuild(version)
		run_tests()
	finally:
		os.chdir(prev_dir)


def print_usage():
	print("Usage python build.py <VersionNumber>")
	print("Example: python build.by 1.0.0.0")


if __name__ == "__main__":
	if len(sys.argv) < 2:
		print_usage()
		sys.exit(1)
	run_build(sys.argv[1])