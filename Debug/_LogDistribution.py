import numpy as np
import pyperclip
import io


def sprint(*args, end='\r\n', **kwargs):
    sio = io.StringIO()
    print(*args, **kwargs, end=end, file=sio)
    return sio.getvalue()


min = 1e-2; # Voltage-Value! Divide by used shunt for real current value
max = 10;    # Voltage-Value! Divide by used shunt for real current value
pts = 20;
a = (np.power(10, (np.linspace(np.log10(min),np.log10(max),pts + 1))))
b = np.flip(a);
c = np.append(a, b)
# c = np.transpose(c)
del a, b

out = ""
for i in range(len(c)):
    out = out + sprint("{:2.4f}".format(c[i]))

pyperclip.copy(out) # copy buffer to clipboard :)
