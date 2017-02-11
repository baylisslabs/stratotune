using System;

namespace bit.shared.numerics
{    
   public class VectorD
    {
        private double[] _v;
        
        public int Dimension { get { return _v.Length; } }
        
        public VectorD(int dimension)
        {
            _v = new double[dimension];
        }
        
        public VectorD(double[] values)
        {
            _v = values;
        }
        
        public VectorD(VectorD vector)
        {
            _v = new double[vector._v.Length];
            for(int i=0;i<_v.Length;++i) {
                _v[i] = vector._v[i];
            }
        }
        
        public double this[int i]
        {
            get 
            {
                return _v[i];
            }
            set
            {
                _v[i] = value;
            }
        }
        
        public double[] ToArray()
        {
            return _v;            
        }
        
        public override string ToString ()
        {
              return "["+string.Join(",",_v)+"]";
        }
                                  
        public static VectorD With(params double[] values)
        {
            return new VectorD(values);
        }
      
        public static VectorD operator +(VectorD a, VectorD b)
        {
            var r = new VectorD(a);
            for(int i=0;i<r._v.Length;++i) {
                r._v[i] += b._v[i];
            }
            return r;
        }

        public static VectorD operator -(VectorD a, VectorD b)
        {
            var r = new VectorD(a);
            for(int i=0;i<r._v.Length;++i) {
                r._v[i] -= b._v[i];
            }
            return r;
        }

        public static VectorD operator -(VectorD b)
        {
            var r = new VectorD(b);
            for(int i=0;i<r._v.Length;++i) {
                r._v[i] = -r._v[i];
            }
            return r;
        }

        public static VectorD operator *(VectorD a, double s)
        {
            var r = new VectorD(a);
            for(int i=0;i<r._v.Length;++i) {
                r._v[i] *= s;
            }
            return r;
        }

        public static VectorD operator *(double s, VectorD a)
        {
            var r = new VectorD(a);
            for(int i=0;i<r._v.Length;++i) {
                r._v[i] *= s;
            }
            return r;
        }

        public static double operator *(VectorD a, VectorD b)
        {
            double s = 0;
            for(int i=0;i<a._v.Length;++i) {
                s += a._v[i] * b._v[i];
            }
            return s;
        }                       
    }    
}

