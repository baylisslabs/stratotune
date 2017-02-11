using System;

namespace bit.shared.numerics
{
    /// <summary>
    /// Represents a function ax2 + bx + c
    /// </summary>
    public struct QuadraticD
    {
        public double A; 
        public double B;
        public double C;
        
        public double Discriminant { get { return B*B - 4.0*A*C; } }
        public double? AxisOfSym { get { return (A!=0)?-B / (2.0 * A):(double?)null; } }
        public Point2D? Vertex { get { return (A!=0)?new Point2D(-B / (2.0 * A),(B*B - 4.0*A*C)/(4.0*A)):(Point2D?)null; } }
        
        public QuadraticD(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="bit.shared.numerics.QuadraticD"/> struct.
        /// </summary>
        /// <param name='coeffs'>
        /// Coeffs. coefficients of a polynomial of degree 2
        /// </param>
        public QuadraticD(VectorD coeffs)
        {
            A = coeffs[2];
            B = coeffs[1];
            C = coeffs[0];
        }                        
    }
}

