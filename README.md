# FEMDAQv2
This project is a measurement tool developed at the OTH for our measurement purposes. The tool is written in C# (.net 4.8). For that the tool basically provides a timed measurement interval and queries measurements or source-updates at the beginning or the end of a interval.
For each device a thread is created[^1] to allow simultaneous data aquisition.

For a few devices drivers already exists:
- Dummy devices (for test reasons):
  - DmyMU.cs
  - DmySMU.cs
  - DmySU.cs
- GPIB devices[^1]:
  - FUGHCP350.cs[^2]
  - HP4145B.[^2]
  - KE6485.c[^2]
  - KE6487.c[^2]
  - DigMultDMM7510.cs
  - FUGMCP140.cs
  - HP4155B.cs
  - KE2400.cs
  - KE6517B.cs
  - SR785.cs
  - WavGen33511B.cs
- Serial devices:
  - MKS909AR.cs
  - VSH8x.cs
  - FEAR16v2.cs
  - MOVE1250.cs
  - VD9x.cs
- USB devices:
  - DMM7510_USB.cs
  - DSOX3000WavGen.cs
  - DSOX3034T.cs
  - HF2LI.cs
- Network devices:
  - RTO2034.cs
  - PiCam.cs
  - PyCam2.cs




[^1]: GPIB does not support simultaneous communication, which is why GPIB devices are collected as a list and iterated one by one.
[^2]: Code by Langer

# Setup
## Hardware
ToDo

## Underlaying system and libraries
ToDo


Written by haum



[VisualStudio2022]:(https://visualstudio.microsoft.com/de/vs/)