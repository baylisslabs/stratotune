using System;

using Jayrock.JsonRpc;

using bit.shared.numerics;
using bit.shared.testutil;

namespace bit.shared.audio.FuncTests
{
	
	class TestService : JsonRpcService
	{
		[JsonRpcMethod]
        public double[][] MatrixD_GaussianElimination(double[][] elems)
        {
            var matrix = new MatrixD(elems);
            var result = matrix.DoGaussianElimination();
            if(result!=null) {
                return result.ToArray();
            }
            return null;
        }
        
        [JsonRpcMethod]
        public double[][] MatrixD_GaussJordan(double[][] elems)
        {
            var matrix = new MatrixD(elems);
            var result = matrix.DoGaussJordan();
            if(result!=null) {
                return result.ToArray();
            }
            return null;
        }
        
        [JsonRpcMethod]
        public QuadraticD? LeastSquaresD_Parabola2(double[][] source)
        {
            var points = Map.Transform(source,(i,p)=>new Point2D(p[0],p[1]));
            var ls = new LeastSquaresD(points);
            var result = ls.Parabola2();
            if(result.HasValue) {
                return result;
            }
            return null;
        }
        
	}
	
	class MainClass
	{
		public static void Main (string[] args)
		{
			JsonPipeRpcDispatcher disp = new JsonPipeRpcDispatcher(new TestService());
            disp.RunLoop(args);
		}
	}
}
