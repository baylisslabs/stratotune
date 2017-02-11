using System;

namespace bit.shared.numerics
{
	public struct Point2D
	{
		public double X;
		public double Y;
		public Point2D (double x, double y)
		{
			X = x;
			Y = y;
		}          
        public void Set(double x, double y)
        {
            X = x;
            Y = y;
        }        
	}	
    
    public static class Point2DExtensions
    {
        public static double[] XValues(this Point2D[] source)
        {
            var xv = new double[source.Length];
            for(int i=0;i<xv.Length;++i) {
                xv[i] = source[i].X;
            }
            return xv;
        }
        
        public static double[] YValues(this Point2D[] source)
        {
            var yv = new double[source.Length];
            for(int i=0;i<yv.Length;++i) {
                yv[i] = source[i].Y;
            }
            return yv;
        }        
    }
}

