using System;
using System.Collections.Generic;

namespace bit.shared.numerics
{
	public class DiscreteData2D
	{
		private List<Point2D> _points;
		
		public List<Point2D> Points { get { return _points; }  set { _points = value; } }
		
		public DiscreteData2D ()
		{
		}

		//
		// Precondition: point[k+1].X > point[k].X for all points
		//
		public DiscreteData2D(Point2D[] points)
		{
			_points = new List<Point2D>(points);
		}
		
		public DiscreteData2D(List<Point2D> points)
		{
			_points = points;
		}
		
		public bool Any()
		{
			return _points!=null&&_points.Count>0;
		}
		
		public int Count()
		{
			return _points!=null?_points.Count:0;
		}
				
		public double XSpan ()
		{		
			return _points[_points.Count-1].X-_points[0].X;
		}
		
		public double MinX ()
		{		
			return _points[0].X;
		}
		
		public double MaxX ()
		{
			return _points [_points.Count-1].X;
		}
		
		public double YSpan ()
		{		
			return this.MaxY ()- this.MinY();
		}
		
		public double MinY ()
		{		
			double min = double.MaxValue;
			foreach(var p in _points) {
				if(p.Y<min) {
					min = p.Y;
				}
			}
			return min;
		}
		
		public double MaxY ()
		{
			double max = double.MinValue;
			foreach(var p in _points) {
				if(p.Y>max) {
					max = p.Y;
				}
			}
			return max;
		}

		public void NormaliseY(double a, double b)
		{
			double max = double.MinValue;
			double min = double.MaxValue;
			var amp = b - a;
			foreach (var p in _points) {
				if(p.Y>max) max = p.Y;
				if(p.Y<min) min = p.Y;
			}
			
			var range = max-min;
			if (range > 0) {
				for (int i=0; i<_points.Count; ++i) {
					var point = _points[i];
					_points[i] = new Point2D(point.X,a + amp/range*(_points [i].Y-min));					
				}
			}
		}

		public DiscreteData2D Differentiate()
		{
			Point2D[] diff = new Point2D[_points.Count-2];
			_generate_central_finite_differences((i,c,d)=>{
				diff[i] = d;
			});
			return new DiscreteData2D(diff);
		}

		public List<Point2D> Minima (double threshold)
		{
			List<Point2D> mins = new List<Point2D> ();
			Point2D last = _points[0];
			Point2D? down = null;
			Point2D? up = null;
			for(int i=1;i<_points.Count;++i) {
				var p = _points[i];

				if (p.Y<last.Y) {
					down = p;
					up = null;
				}
				else if(p.Y>last.Y) {
					up = last;
				}

				if(up!=null&down!=null) {
					if(down.Value.Y<threshold) {
						mins.Add(new Point2D((down.Value.X+up.Value.X)/2,down.Value.Y));
					}
					down = null;
					up = null;
				}

				last = p;
			}
			return mins;
		}
        
        public List<Point2D> MinimaBetweenZeroCrossing (double threshold, double zero_level)
        {
            List<Point2D> mins = new List<Point2D> ();
            Point2D last = _points [0];
            Point2D? down = null;
            Point2D? up = null;
            Point2D? local_min = null;
            for (int i=1; i<_points.Count; ++i) {
                var p = _points [i];
    
                if (p.Y < last.Y) {
                    down = p;
                    up = null;
                } else if (p.Y > last.Y) {
                    up = last;
                }
    
             
                if (up != null & down != null) {
                    if (down.Value.Y < threshold) {
                        if(local_min==null||local_min.Value.Y > down.Value.Y) {
                            local_min = new Point2D ((down.Value.X + up.Value.X) / 2, down.Value.Y);
                        }
                        
                    }
                    down = null;
                    up = null;
                }
                
                if (p.Y >= zero_level) {
                    if(local_min!=null) {
                        mins.Add (local_min.Value);
                    }
                    local_min = null;
                }
               
                last = p;
            }
            
            if(local_min!=null) {
                mins.Add (local_min.Value);
            }
            
            return mins;
        }

		// pre: a.X <= x <= b.X
		private double lerp_r(int i,double x)
		{
			var a = _points[i];
			var b = _points[i+1];
			return a.Y + (b.Y-a.Y)*(x-a.X)/(b.X-a.X);
		}
					
		private void _generate_central_finite_differences (Action<int,Point2D,Point2D> handler)
		{
			for (int i=0; i<_points.Count-2; ++i) {
				var a = _points[i];
				var c = _points[i+1];
				var b = _points[i+2];
				var h = Math.Min(b.X-c.X,c.X-a.X);

				handler(i,c,new Point2D(c.X,(lerp_r(i+1,c.X+h)-lerp_r(i,c.X-h))/(2.0*h)));
			}
		}
	}
}

