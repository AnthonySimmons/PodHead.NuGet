import os

def run_command(command):
    print("Command: %s" % command)
    result = os.system(command)
    if result is not 0:
        raise Exception("Build Error!")

