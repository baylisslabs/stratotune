using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

using bit.shared.numerics;

namespace Hello_MultiScreen_iPhone
{
	public class TunerReadoutView : UIView
	{
		public DiscreteData2D DataPoints { get; set; }
		public Point2D? FirstMin { get; set; }

		public TunerReadoutView (RectangleF frame) :  base(frame)
		{

		}

		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);
			using (CGContext context = UIGraphics.GetCurrentContext()) {
				context.ClearRect(this.Bounds);

				if(DataPoints!=null&&DataPoints.Any()) {
					float xspan = (float)DataPoints.XSpan();
					float minx = (float)DataPoints.MinX();
                    float yspan = 2;//(float)DataPoints.YSpan();
                    float miny = 0;//(float)DataPoints.MinY();
                    var points = DataPoints.Points;
					float n = points.Count;
					float w = this.Bounds.Width;
					float h = this.Bounds.Height;
					context.SetLineWidth(2f);
					context.SetStrokeColor(UIColor.Green.CGColor);
					context.MoveTo(this.Bounds.Left,this.Bounds.Bottom);
					if(xspan>0&&yspan>0) {
					for(int i=0;i<n;++i) {
							var x = w*((float)points[i].X-minx)/xspan;
							var y = h*((float)points[i].Y-miny)/yspan;
							var point = new PointF(this.Bounds.Left+x,this.Bounds.Bottom-y);
							context.AddLineToPoint(point.X,point.Y);
						}
						
						context.StrokePath();
						if(this.FirstMin!=null) {
							context.SetStrokeColor(UIColor.Red.CGColor);
							var x = w*((float)FirstMin.Value.X-minx)/xspan;
							context.MoveTo(this.Bounds.Left+x,this.Bounds.Bottom);				
							context.AddLineToPoint(this.Bounds.Left+x,this.Bounds.Top);	
							context.StrokePath();
						}
					}
				}			
			}
		}
	}
}

