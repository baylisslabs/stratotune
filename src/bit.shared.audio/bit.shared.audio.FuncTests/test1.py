
import proxy as px

import numpy as np
import matplotlib.pyplot as plt
import math


#f0 = 220.06;
f0 = 27.5;
class params:
    sampleRate = 44100.0
    windowLength = 4800;

def getSineWave(points,sampleRate,freq,amp,phase):
    y = []
    for x in points:        
        y.append(amp*math.sin(phase+2.0*math.pi*x*freq/sampleRate))
    return y

x = range(0,params.windowLength)
y0 = getSineWave(x,params.sampleRate,f0,1.0,0)
y1 = getSineWave(x,params.sampleRate,f0*2.0,0.0,0)
y = np.add(y0,y1) * 10000
test_proxy = px.Proxy()
pd_result = test_proxy.PitchDetector_Process32BitMonoLinearPCM(y.tolist(),params.sampleRate)
corr_data = pd_result['correlationData']['points']
ls_data = pd_result['leastSquaresData']['points']
quad = pd_result['leastSquaresParabola']
print ls_data
fig = plt.figure(1)
ax = plt.subplot(111)
ax.text(15,0.9,'f_0_hz = %.2f' % (pd_result['f_0_Hz']))
ax.text(15,0.85,'q_hz = %.2f' % (pd_result['q_Hz']))
ax.text(15,0.8,'rate = %1.f Hz' % (pd_result['samplingRate']))
ax.text(15,0.75,'processingTime = %f ms' % (pd_result['processingTimeMs']))
ax.text(15,0.7,'quadratic fit = %d points' % (len(ls_data)))
ax.text(15,0.65,'quadratic coeffs = %.2fx2 + %.2fx + %.2f' % (quad['a'],quad['b'],quad['c']))
ax.annotate('Selected Minima', xy=(pd_result['selectedMinima']['x'],pd_result['selectedMinima']['y'])
        ,xytext=(600,0.6),arrowprops=dict(facecolor='red',shrink=0.0))
plt.plot([p['x'] for p in corr_data],[p['y'] for p in corr_data])
for xy in ls_data:
    plt.plot(xy['x'],xy['y'],'+')
plt.show()

