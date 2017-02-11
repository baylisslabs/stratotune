using System;
using System.Linq;

namespace bit.shared.numerics
{
    public class LeastSquaresD
    {
        private Point2D[] _points;
        
        public LeastSquaresD (Point2D[] points)
        {
            _points = points;
        }        
        
        public QuadraticD? Parabola2()
        {                   
            var xvals = _points.XValues();
            var yvals = _points.YValues();
            var nvals = _points.Length;
            
            var xvals2  = xvals.square();
            var sx = xvals.sum();
            var sx2 = xvals2.sum();
            var sx3 = xvals.cube().sum();
            var sx4 = xvals.to4th().sum();
            
            var augm = MatrixD.With(
                        VectorD.With(nvals,sx,sx2,yvals.sum())
                        ,VectorD.With(sx,sx2,sx3,yvals.product(xvals).sum())
                        ,VectorD.With(sx2,sx3,sx4,yvals.product(xvals2).sum()));
                         
            if(augm.DoGaussJordan()!=null)
            {
                return new QuadraticD(augm.Col(3));
            }            
            return null;
        }
    }
}

