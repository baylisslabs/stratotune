
import proxy as px

import numpy as np
import matplotlib.pyplot as plt
import math


#matrix = [[3,2,-1,1],[6,6,2,12],[3,-2,1,11]]
#matrix = [[0.00044,0.0003,-0.0001,0.00046],[4,1,1,1.5],[3,-9.2,-0.5,-8.2]]
#points = [[-10,2],[0,20],[2,5]]
#points = [[-10,50],[0,10],[2,-5],[8,150]]
points = [[1,1],[2,25],[56,78]]
test_proxy = px.Proxy()
#pd_result = test_proxy.MatrixD_GaussianElimination(matrix)
#pd_result = test_proxy.MatrixD_GaussJordan(matrix)
pd_result = test_proxy.LeastSquaresD_Parabola2(points);
print pd_result;

a = pd_result['a']
b = pd_result['b']
c = pd_result['c']

x = np.linspace(-15,15,100)
y = a*x*x+ b*x + c

extrema_x = -b/(2.0*a)
extrema_y = c + b*extrema_x + a*extrema_x*extrema_x

plt.figure(1)
ax = plt.subplot(111)
plt.plot(x,y)
for xy in points:
    plt.plot(xy[0],xy[1],'+')
    
ax.annotate('Minima', xy=(extrema_x,extrema_y)
        ,xytext=(10,200),arrowprops=dict(facecolor='red',shrink=0))

    
plt.show()

