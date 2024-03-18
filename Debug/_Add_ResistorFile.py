##################################################################################
# This script adds a "value.resistor" file to the folders which contain measure- #
# ment data. This can be used to automatically load the used resistor-value to   #
# remove the resitance-dependency of electrical measurement-data.                #
#                                                                                #
# How to use (variable explanation):                                             #
# SkipBadSubdirs: If this is enabled, the subfolders of any _XX marked pa-   #
#                      rents are skipped in addition.                            #
# parentDir:          Folder which is scanned recursevly for measurement-files.  #
# picDir:             Name of the subfolder where the PyCam2 pictures are stored #
#                      in.                                                       #
# ResistorValue:      The value which gets stored in the value.resistor file.    #
#                                                                                #
# 2023 © haum (OTH-Regensburg)                                                   #
##################################################################################

import os
from time import time

def DiffTime(t0, t1):
    return t1 - t0
def DiffToNow(t0):
    return DiffTime(t0, time())
def Time2Human(time):
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
        return "[%02dd %02dh %02dm %02ds %03dms]" % (DD, HH, MM, SS, MS)
    elif not HH == 0:
        return "[%02dh %02dm %02ds %03dms]" % (HH, MM, SS, MS)
    elif not MM == 0:
        return "[%02dm %02ds %03dms]" % (MM, SS, MS)
    elif not SS == 0:
        return "[%02ds %03dms]" % (SS, MS)
    else:
        return "[%03dms]" % (MS)
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
def LogLine(t0, yellowMsg="", whiteMessage="", yFill = 0, wFill=65, end=""):
    """Writes a colorized log-line

    Args:
        t0 (time): Time-value
        yellowMsg (str, optional): Yellow infotext. Defaults to "".
        whiteMessage (str, optional): White infotext. Defaults to "".
        yFill (int, optional): Ensures the yellow messages is int chars long. Defaults to 0.
        wFill (int, optional): Ensured the entire message is int chars long. Defaults to 65.
        end (str, optional): Custom line-end. Defaults to "".
    """
    wFill -= yellowMsg.__len__()
    if yFill > wFill:
        wFill = yFill
        
    if t0 == None:
        print("".rjust(18) + bcolors.WARNING + " " + yellowMsg.ljust(yFill) + bcolors.ENDC + whiteMessage.ljust(wFill-yFill), end=end)
    elif t0 < 0:
        print(bcolors.WARNING + " " + yellowMsg.ljust(yFill) + bcolors.ENDC + whiteMessage.ljust(wFill-yFill), end=end)
    else:
        print(bcolors.OKBLUE + Time2Human(DiffToNow(t0)).rjust(18) + bcolors.WARNING + " " + yellowMsg.ljust(yFill) + bcolors.ENDC + whiteMessage.ljust(wFill), end=end)


###### USER AREA ######
SkipBadSubdirs = False                                   # If a parent folder is marked as bad measurement, the subdirectories also skipped!y


# Specific or parent directory of folders you want to add the value.resistor file
wds = [ 
# r"Z:\_FEMDAQ V2 for Measurement\Hausi\240130 PCBLens 1st Try\02 Messungen\02_01 Recondition Cycle",
r"Z:\_FEMDAQ V2 for Measurement\Hausi\240130 PCBLens 1st Try\02 Messungen",
]

# Specific paths which should be ignored by this script (useful when giving a parent directory)
wdsIgnore = [ 
r"Z:\_FEMDAQ V2 for Measurement\Hausi\240130 PCBLens 1st Try\02 Messungen\00_00 Light & Darkshot pre-Measurements",
r"Z:\_FEMDAQ V2 for Measurement\Hausi\240130 PCBLens 1st Try\02 Messungen\00_01 KS Lenses",
r"Z:\_FEMDAQ V2 for Measurement\Hausi\240130 PCBLens 1st Try\02 Messungen\00_02 KS Tips",
]


OverrideValue = False
ResistorValue = 1e6







###### DO NOT TOUCH AREA ######
t0 = time()
_XXBadDirs = list()

for parentDir in wds:
    for root, dirs, files in os.walk(parentDir): # Iterate recursevily through parentDir
        # Firstly check if path contains one of the already marked bad measurement-folders
        if any(root.__contains__(_bDir) for _bDir in _XXBadDirs):
            print(bcolors.OKBLUE + Time2Human(DiffToNow(t0)).rjust(18) + bcolors.WARNING + " Bad parent - skipped: " + bcolors.ENDC + root)
            continue
        # Folder marked as bad measurement -> Skip
        if root.endswith("_XX"):
            if SkipBadSubdirs == True:
                _XXBadDirs.append(root)
            print(bcolors.OKBLUE + Time2Human(DiffToNow(t0)).rjust(18) + bcolors.WARNING + " Marked as bad - skipped: " + bcolors.ENDC + root)
            continue
        
        # Check for specificly ignored folders
        if wdsIgnore.__contains__(root):
            print(bcolors.OKBLUE + Time2Human(DiffToNow(t0)).rjust(18) + bcolors.WARNING + " WD-Ignore - skipped: " + bcolors.ENDC + root)
            continue

        print("\n" + bcolors.OKBLUE + Time2Human(DiffToNow(t0)).rjust(18) + bcolors.WARNING + " Entering: " + bcolors.ENDC + root)

        #Check if current directory contains measurement-files
        #    Directory found when it contains a Pics directory and *.dat files
        # if not dirs.__contains__(picDir) or not any(f.endswith(".dat") for f in files): # Old one, but sometime i want to have value.resistor also in folders were the pictures are not extracted yet!
        if not any(f.endswith(".dat") for f in files):
            print("".rjust(18) + bcolors.WARNING + " Nothing interesting here" + bcolors.ENDC)
        else:
            print(bcolors.OKBLUE + Time2Human(DiffToNow(t0)).rjust(18) + bcolors.WARNING + " Possible directory found: " + bcolors.ENDC + root)
            _fPathResistor = os.path.join(root, "value.resistor")
            if OverrideValue == False:
                if os.path.exists(_fPathResistor):  # When file exists already
                    LogLine(t0=t0, yellowMsg=f'"value.resistor"', whiteMessage=f" already exsist (Override=Off)", wFill=0, end="\n" )
                    continue                        #  Jump over
            _output = "%e" % ResistorValue
            f = open(_fPathResistor, "w")
            f.write(_output)
            f.close()
            LogLine(None, "Saved: ", _output + " to " + os.path.basename(_fPathResistor), wFill=0, end="\n" )