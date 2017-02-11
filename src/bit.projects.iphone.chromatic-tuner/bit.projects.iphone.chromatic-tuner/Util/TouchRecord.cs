using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace bit.projects.iphone.chromatictuner
{
    public class TouchRecord
    {
        public PointF LocationInView { get; private set; }
        public double Timestamp { get; private set; }
        public PointF VelocityInView { get; set; }

        public TouchRecord (UIView view, UITouch touch)
        {
            this.LocationInView = touch.LocationInView(view);
            this.Timestamp = touch.Timestamp;
        }
    }
}

