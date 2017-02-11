using System;
using System.Drawing;

using MonoTouch.CoreGraphics;

namespace bit.projects.iphone.chromatictuner
{
    public static class GeometryExtensions
    {
        public static RectangleF SetBottom(this RectangleF rect, float bottom)
        {
            rect.Height = Math.Max(0,bottom-rect.Top);
            return rect;
        }

        public static RectangleF SetRight(this RectangleF rect, float right)
        {
            rect.Width = Math.Max(0,right-rect.Top);
            return rect;
        }

        public static RectangleF SetBottomRight(this RectangleF rect, float right, float bottom)
        {
            rect.Height = Math.Max(0,bottom-rect.Top);
            rect.Width = Math.Max(0,right-rect.Top);
            return rect;
        }
            
        public static RectangleF SetTop(this RectangleF rect, float top)
        {
            rect.Height = Math.Max(0,rect.Bottom - top);
            rect.Y = top;
            return rect;
        }

        public static RectangleF SetLeft(this RectangleF rect, float left)
        {
            rect.Width = Math.Max(0,rect.Right-left);
            rect.X = left;
            return rect;
        }

        public static RectangleF SetTopLeft(this RectangleF rect, float left, float top)
        {
            rect.Height = Math.Max(0,rect.Bottom - top);
            rect.Y = top;
            rect.Width = Math.Max(0,rect.Right-left);
            rect.X = left;
            return rect;
        }

        public static PointF Scale(this PointF orig,float s)
        {
            return new PointF(orig.X*s,orig.Y*s);
        }

        public static PointF Translate(this PointF orig,float x, float y)
        {
            return new PointF(orig.X+x,orig.Y+y);
        }

        public static PointF Translate(this PointF orig, PointF offset)
        {
            return new PointF(orig.X+offset.X,orig.Y+offset.Y);
        }
    }
}

