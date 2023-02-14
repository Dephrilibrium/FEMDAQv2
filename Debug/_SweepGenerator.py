# Imports
import numpy as np

# Preamble
def lin(start, stop, pts):
    return np.linspace(start, stop, pts)

def log10(start, stop, pts):
    if start <= 0 or stop <= 0:
        return lin(start, stop, pts)

    return np.power(10, (np.linspace(np.log10(start), np.log10(stop), pts)))





####### USER AREA #######
# swpPar = ["Rpts", "U7", "CC0", "CC1", "CC2", "CC3"]
swpPrms = ["Rpts", "U7", "CC0"]
parFunc = [lin, lin, log10]

iterStps = [28, 20, 20, 28]
swpVal = [
            [[1, 1], [0, 1400],    [0, 0]],                 # Iter 1
            [[1, 1], [1400, 1400], [1e-2, 10]],             # Iter 2
            [[1, 1], [1400, 1400], [10, 1e-2]],             # Iter 3
            [[1, 1], [1400, 0],    [0, 0]],                 # Iter 4
         ]


# fileName = str.format("ISwp I{}} to {} - U{}}V - {}Pts", swpVal) # Zeile Haus, so bitte ne eigene machen


####### DO NOT TOUCH AREA #######
iterCnt = iterStps.__len__() # Get amount of sequence-steps
# Calculate how big the np.array must be
parCnt = swpPrms.__len__()
rowCnt = 0

for iStp in range(iterCnt):
    iterStps[iStp] = iterStps[iStp] + 1 # Steps to Points

    # Build Rows/Columns per Iter
    for iPar in range(parCnt):
        # rowCnt = rowCnt + 1
        # swpPar = np.linspace(swpVal[iStp][iPar][0], swpVal[iStp][iPar][1], iterStps[iStp])
        swpPar = parFunc[iPar](swpVal[iStp][iPar][0], swpVal[iStp][iPar][1], iterStps[iStp])
        swpPar = np.transpose(np.atleast_2d(swpPar)) # Convert row to column-vector

        # Combine
        if iPar == 0:
            swpIter = swpPar
        else:
            swpIter = np.hstack((swpIter, swpPar))

    # Attach Iters
    if iStp == 0:
        swpOut = swpIter
    else:
        swpOut = np.vstack((swpOut, swpIter))
del swpPar, swpIter, iStp, iPar


f = open("_pySwpOut.swp", "w")
f.write(",".ljust(10).join(swpPrms) + "\n")
# np.savetxt(f, head, delimiter=",", fmt="%s")
np.savetxt(f, swpOut, delimiter=",".ljust(10), fmt="%.4f")
f.flush()
f.close()

print("Hello World")
# Linear
# for iter in range(iterCnt):
#     np.lin


