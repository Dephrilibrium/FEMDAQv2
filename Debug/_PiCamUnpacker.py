# Imports
import os
import os.path
import subprocess
from xml.dom.pulldom import END_DOCUMENT
import sys
import time


# Preamble & Helpers
class bcolors:
    HEADER = "\033[95m"
    OKBLUE = "\033[94m"
    OKCYAN = "\033[96m"
    OKGREEN = "\033[92m"
    WARNING = "\033[93m"
    FAIL = "\033[91m"
    ENDC = "\033[0m"
    BOLD = "\033[1m"
    UNDERLINE = "\033[4m"


class Logger(object):
    def __init__(self):
        self.terminal = sys.stdout
        self.log = open(xLog, "w")

    def write(self, message):
        self.terminal.write(message)
        self.log.write(message)

    def flush(self):
        # this flush method is needed for python 3 compatibility.
        # this handles the flush command by doing nothing.
        # you might want to specify some extra behavior here.
        pass


def TimeDelta(t0, t1):
    return t1 - t0


def ConvertTime2Human(time):
    DD = int(time / (3600 * 24))  # Get integer days
    time = time - (3600 * 24 * DD)  # Remove days from time
    HH = int(time / 3600)  # Get integer hours
    time = time - (3600 * HH)  # Remove hours from time
    MM = int(time / 60)  # Get integer minutes
    time = time - (60 * MM)  # Remove hours from time
    SS = int(time % 60)  # Get integer minutes
    MS = int((time % 1) * 100)  # Get integer milliseconds

    # Check the cases and return correct string
    if not DD == 0:
        return "%02dd %02dh %02dm %02ds %03dms" % (DD, HH, MM, SS, MS)
    elif not HH == 0:
        return "%02dh %02dm %02ds %03dms" % (HH, MM, SS, MS)
    elif not MM == 0:
        return "%02dm %02ds %03dms" % (MM, SS, MS)
    elif not SS == 0:
        return "%02ds %03dms" % (SS, MS)
    else:
        return "%03dms" % (MS)


def PrintProcessStats(t0, t1, t2):
    print("Processed files:" + bcolors.OKGREEN + str(fCnt).rjust(24) + bcolors.ENDC)
    print("Process time:   " + bcolors.OKBLUE + ConvertTime2Human(TimeDelta(t1, t2)).rjust(24) + bcolors.ENDC)
    print("Cumulative time:" + bcolors.OKBLUE + ConvertTime2Human(TimeDelta(t0, t2)).rjust(24) + bcolors.ENDC
    )


def PrintVerbosePaths(fPath, xPath, fPathAbs, xPathAbs):
    print("rel. fPath: " + fPath)
    print("rel. xPath: " + xPath)
    print("abs. fPath: " + fPathAbs)
    print("abs. xPath: " + xPathAbs)


def RunCmd(cmd):
    if skipCmd == True:
        return

    if verbose == True:
        subprocess.call(cmd)
    else:
        subprocess.call(cmd, stdout=subprocess.DEVNULL, stderr=subprocess.STDOUT)  # Only catch errors


##########################################################
# Script to extract the PiCam tar.gz to a Pics folder    #
# Tested on Win10 with standard-installation-path        #
##########################################################


###### USER AREA ######
xCmd = '"C:\\Program Files\\7-Zip\\7z.exe"'  # Path to 7zip
xPath = "Pics"  # Subdirectory where extract to. !!! Do not add a leading / or \ !!!
xLog = "_PiCamUnpacker.log"  # Filename to log output

rmCmd = "del /f"  # delete command to delete files

fileTypes = [".jpg", ".jpeg",]  # List of filetype which is counted at the end for statistics

# Debug flags
verbose = False  # Debug-flag if 7zip should ouput infos to it's extraction progresses
skipCmd = False  # Set to true to skip the cmd-exection (for test purposes)


###### DO NOT TOUCH AREA ######
t0 = time.time()  # Script starts
# Prepare format-string for 7zip
szCmd = xCmd + ' x "{}" -o"{}" -r -y'

# Prepare format-string for file-delete
rmCmd = rmCmd + ' "{}"'


# Change working-directory
sys.stdout = Logger()
owdPath = os.getcwd()
pyPath = os.path.dirname(__file__)
os.chdir(pyPath)
cwdPath = os.getcwd()
if verbose == True:
    print(bcolors.WARNING + "Changing WD..." + bcolors.ENDC)
    print(bcolors.FAIL + "Old" + bcolors.ENDC + " working dir:\t" + owdPath)
    print("Python File dir:\t" + pyPath)
print(bcolors.OKBLUE + "New" + bcolors.ENDC + " working dir:\t" + cwdPath)
print("Changing WD " + bcolors.OKGREEN + "OK" + bcolors.ENDC)


# Extract tar.gz
print("\r\n")
print("Extracting *.tar.gz:")
fCnt = 0
for dirpath, dirnames, filenames in os.walk("."):
    for filename in [f for f in filenames if f.endswith(".tar.gz")]:
        _fPath = os.path.join(dirpath, filename)
        _fPathAbs = os.path.abspath(_fPath)
        _xPath = os.path.join(dirpath, xPath)
        _xPathAbs = os.path.abspath(_xPath)
        if verbose == True:
            PrintVerbosePaths(_fPath, _xPath, _fPathAbs, _xPathAbs)

        print(bcolors.WARNING + "Unpacking:" + bcolors.ENDC + ' "' + _fPath + '" -> "' + _xPath,end="",)
        cmd = str.format(szCmd, _fPath, _xPath)
        RunCmd(cmd)
        print(bcolors.OKGREEN + "\tExtracted" + bcolors.ENDC)
        fCnt = fCnt + 1
t1 = time.time()
PrintProcessStats(t0, t0, t1)

# Extract .tar
print("\r\n")
print("Extracting *.tar:")
fCnt = 0
for dirpath, dirnames, filenames in os.walk("."):
    for filename in [f for f in filenames if f.endswith(".tar")]:
        _fPath = os.path.join(dirpath, filename)
        _fPathAbs = os.path.abspath(_fPath)
        _xPath = os.path.join(dirpath)
        _xPathAbs = os.path.abspath(_xPath)
        if verbose == True:
            PrintVerbosePaths(_fPath, _xPath, _fPathAbs, _xPathAbs)

        print( bcolors.WARNING + "Unpacking:" + bcolors.ENDC + ' "' + _fPath + '" -> "' + _xPath, end="",)
        cmd = str.format(szCmd, _fPath, _xPath)
        RunCmd(cmd)
        print(bcolors.OKGREEN + "\tExtracted" + bcolors.ENDC)
        fCnt = fCnt + 1
t2 = time.time()
PrintProcessStats(t0, t1, t2)

# Clean up .tar
print("\r\n")
print("Cleaning up *.tar:")
fCnt = 0
for dirpath, dirnames, filenames in os.walk("."):
    for filename in [f for f in filenames if f.endswith(".tar")]:
        _fPath = os.path.join(dirpath, filename)
        _fPathAbs = os.path.abspath(_fPath)
        _xPath = "-"  # Not existing
        _xPathAbs = "-"  # Not existing
        if verbose == True:
            PrintVerbosePaths(_fPath, _xPath, _fPathAbs, _xPathAbs)

        print(bcolors.FAIL + "Cleaning up:" + bcolors.ENDC + ' "' + _fPath, end="")
        cmd = str.format(rmCmd, _fPath)
        RunCmd(cmd)
        print(bcolors.OKGREEN + "\tDeleted" + bcolors.ENDC)
        fCnt = fCnt + 1
t3 = time.time()
PrintProcessStats(t0, t2, t3)


# Count pictures
print("\r\n")
print(str.format("Counting files of type: {}", fileTypes))
fCnt = 0
for dirpath, dirnames, filenames in os.walk("."):
    _filenames = [
        fn
        for fn in os.listdir(dirpath)
        if any(fn.lower().endswith(fType) for fType in fileTypes)
    ]

    _fCnt = len(_filenames)
    fCnt = fCnt + _fCnt
    if verbose == True:
        print("Found" + bcolors.OKGREEN + str(_fCnt).rjust(10) + bcolors.ENDC + '   files in "' + dirpath + '"')
    else:
        if not _fCnt == 0:
            print("Found" + bcolors.OKGREEN + str(_fCnt).rjust(10) + bcolors.ENDC + '   files in "' + dirpath + '"')
t4 = time.time()
print("Found" + bcolors.OKBLUE + str(fCnt).rjust(10) + bcolors.ENDC + "   files overall")
PrintProcessStats(t0, t3, t4)


sys.stdout.flush() # be sure everything is written to the log-file!