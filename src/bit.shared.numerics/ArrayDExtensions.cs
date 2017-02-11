using System;
using System.Collections.Generic;

namespace bit.shared.numerics
{
	public static class ArrayDExtensions
	{        
        
        public static double[] slice(this double[] source, int start, int count)
        {
            var result = new double[count];
            Array.Copy(source,start,result,0,count);
            return result;
        }
		public static double[] exp(this double[] data)
		{
			var result = new double[data.Length];
			for (int i=0; i<data.Length; ++i) {
				result [i] = Math.Exp(data[i]);
			}
			return result;
		}
		
        public static double sum(this double[] data)
        {
            var sum = 0.0;
            for (int i=0; i<data.Length; ++i) {
                sum += data[i];
            }
            return sum;
        }
        
        public static double[] product(this double[] a, double[] b)
        {
            var result = new double[a.Length];
            for (int i=0; i<a.Length; ++i) {
                result [i] = a[i]*b[i];
            }
            return result;
        }
        
        public static double[] square(this double[] data)
        {
            var result = new double[data.Length];
            for (int i=0; i<data.Length; ++i) {
                result [i] = data[i]*data[i];
            }
            return result;
        }
        
		public static double[] cube(this double[] data)
		{
			var result = new double[data.Length];
			for (int i=0; i<data.Length; ++i) {
				result [i] = data[i]*data[i]*data[i];
			}
			return result;
		}
        
        public static double[] to4th(this double[] data)
        {
            var result = new double[data.Length];
            for (int i=0; i<data.Length; ++i) {
                var sq = data[i]*data[i];
                result [i] = sq*sq;
            }
            return result;
        }

		public static double[] pwr(this double[] data, double exp)
		{
			var result = new double[data.Length];
			for (int i=0; i<data.Length; ++i) {
				result [i] = Math.Pow(data[i],exp);
			}
			return result;
		}
		
		public static double[] to_double(this int[] data)
		{
			var result = new double[data.Length];
			for (int i=0; i<data.Length; ++i) {
				result [i] = data[i];
			}
			return result;
		}
		
		public static float[] to_float(this int[] data)
		{
			var result = new float[data.Length];
			for (int i=0; i<data.Length; ++i) {
				result [i] = data[i];
			}
			return result;
		}
		
		public static double[] gate(this double[] data, double thresh)
		{
			var result = new double[data.Length];
			for (int i=0; i<data.Length; ++i) {
				var x = data[i];
				result [i] = (Math.Abs(x)<thresh?0:x);
			}
			return result;
		}
		
		public static int[] rising_edge_events (this int[] data)
		{
			var events = new List<int>();
			int q = data[0];
			for(int i=1;i<data.Length;++i) {
				var c = data[i];
				if(c==1&&q==-1) {
					events.Add(i);
				}
				q=c;
			}
			return events.ToArray();
		}
		
		public static int[] latch(this double[] data, double thresh)
		{
			var result = new int[data.Length];
			var q = 0;
			for (int i=0; i<data.Length; ++i) {
				var x = data[i];
				if(x>thresh) q = 1;
				else if (x<-thresh) q = -1;
				result [i] = q;
			}
			return result;
		}
		
		public static double[] lowpass(this double[] data, double alpha)
		{
			var result = new double[data.Length];
			result[0] = data[0];
			for (int i=1; i<data.Length; ++i) {
				result[i] = result[i-1]+alpha*(data[i]-result[i-1]);
			}
			return result;
		}
		
		public static double[] last (this double[] data, int n)
		{
			var result = new double[n];
			for (int i=0; i<n; ++i) {
				result[i] = data[data.Length-n+i];
			}
			return result;
		}
		
		public static double[] differentiate (this double[] func)
		{
			double[] diff = new double[func.Length-1];
			double a = func[0];
			for (int i=0; i<diff.Length; ++i) {
				double b = func[i+1];
				diff[i] = b - a;
				a = b;
			}
			return diff;
		}
		
		public static double[] integrate (this double[] func)
		{
			double[] result = new double[func.Length];
			double a = 0;
			for (int i=0; i<func.Length; ++i) {
				double b = func[i] + a;
				result[i] = b;
				a = b;
			}
			return result;
		}
		
		public static void norm (this double[] data, double a, double b)
		{
			double max = double.MinValue;
			double min = double.MaxValue;
			var amp = b - a;
			foreach (var x in data) {
				if(x>max) max = x;
				if(x<min) min = x;
			}
			
			var range = max-min;
			if (range > 0) {
				for (int i=0; i<data.Length; ++i) {
					data [i] = a + amp/range*(data [i]-min);
				}
			}
		}
		
        /* TODO: move this */
		public static int[] range (int min, int max, int step=1)
		{			
			var a = min;
			var n = (max-min)/step;
			var result = new int[n];
			for(int i=0;i<n;a+=step,++i) {
				result[i] = a;
			}
			return result;
		}
	}
}

