
import proxy as px

import numpy as np
import matplotlib.pyplot as plt
import math


f0 = 220.06;
class params:
    sampleRate = 44100.0
    windowLength = 3200;

test_proxy = px.Proxy()

x = range(0,params.windowLength)

result = test_proxy.EnvelopeModulator_Generate (1.0,params.windowLength,params.sampleRate,'SineWave',f0)
y0 = result

result = test_proxy.EnvelopeModulator_Generate (1.0,params.windowLength,params.sampleRate,'SquareWave',f0)
y1 = result

result = test_proxy.EnvelopeModulator_Generate (1.0,params.windowLength,params.sampleRate,'TriangularWave',f0)
y2 = result

result = test_proxy.EnvelopeModulator_Generate (1.0,params.windowLength,params.sampleRate,'SawtoothWave',f0)
y3 = result


fig = plt.figure(1)
plt.subplot(411)
plt.plot(x,y0);
plt.subplot(412)
plt.plot(x,y1);
plt.subplot(413)
plt.plot(x,y2);
plt.subplot(414)
plt.plot(x,y3);
plt.show()

