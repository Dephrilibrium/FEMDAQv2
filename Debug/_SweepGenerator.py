# Imports
import numpy as np


# swpPar = ["Rpts", "U7", "CC0", "CC1", "CC2", "CC3"]
swpPar = ["Rpts", "U7", "CC0"]

iterStps = [7, 5, 5, 7]
swpVal = [
            [[1, 1], [0, 1400], [0, 0]], # Iter 1
            [[1, 1], [1400, 1400], [0, 10]], # Iter 2
            [[1, 1], [1400, 1400], [10, 0]], # Iter 3
            [[1, 1], [1400, 0], [0, 0]], # Iter 4
         ]



####### DO NOT TOUCH AREA #######
iterCnt = iterStps.__len__() # Get amount of sequence-steps
# Calculate how big the np.array must be
parCnt = swpPar.__len__()
rowCnt = 0

for iStp in range(iterCnt):
    iterStps[iStp] = iterStps[iStp] + 1 # Steps to Points

    # Build Rows/Columns per Iter
    for iPar in range(parCnt):
        # rowCnt = rowCnt + 1
        swpPar = np.linspace(swpVal[iStp][iPar][0], swpVal[iStp][iPar][1], iterStps[iStp])
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

print("Hello World")
# Linear
# for iter in range(iterCnt):
#     np.lin




















