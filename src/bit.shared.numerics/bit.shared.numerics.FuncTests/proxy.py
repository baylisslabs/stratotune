
import jsonpiperpc as jrpc


class Proxy:
    def __init__(self):
        self.ep = jrpc.RpcEndPoint("bin/Debug/bit.shared.numerics.FuncTests.exe")

    def MatrixD_GaussianElimination(self,elems):
        return self.ep.call_method('MatrixD_GaussianElimination',[elems])
                
    def MatrixD_GaussJordan(self,elems):
        return self.ep.call_method('MatrixD_GaussJordan',[elems])
        
    def LeastSquaresD_Parabola2(self,points):
        return self.ep.call_method('LeastSquaresD_Parabola2',[points])        
