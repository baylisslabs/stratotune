using System;
using System.Drawing;

using MonoTouch.CoreGraphics;

namespace bit.projects.iphone.chromatictuner
{
    public class LayoutHelper
    {
        [Flags]
        public enum SizeFlags
        {
            None = 0x0,
            WidthNone = 0x1,
            WidthPx = 0x2,
            WidthPercent = 0x4,
            HeightNone = 0x8,
            HeightPx = 0x10,
            HeightPercent = 0x20,
            Px = HeightPx | WidthPx,
            Percent = HeightPercent |  WidthPercent
        }

        public delegate RectangleF LayoutFunc(RectangleF frame);
               
        public bool IntegralMode { get; set; }
        public RectangleF Frame { get; private set; }
        public RectangleF CurrentBlock { get; private set; }
        public RectangleF CurrentUsed { get; private set; }

        public LayoutHelper (RectangleF frame, bool integralMode = true)
        {
            this.IntegralMode = integralMode;
            if (this.IntegralMode) {
                frame = frame.Integral();
            }
            this.Frame = frame;
            this.CurrentBlock = frame; 
            this.CurrentUsed = new RectangleF (frame.Location, new SizeF (0, 0));  
        }

        public void DoBlockLayout (float? width = null, float? height = null, SizeFlags sizeFlags = SizeFlags.None, LayoutFunc layoutFunc = null)
        {               
            var frame = calcFrame (this.Frame, this.CurrentBlock, width, height, sizeFlags);
            if (layoutFunc != null) {
                frame = layoutFunc (frame);           
            }
            this.CurrentUsed = this.CurrentUsed.SetBottomRight (Math.Max (this.CurrentUsed.Right, frame.Right), Math.Max (this.CurrentUsed.Bottom, frame.Bottom));          
            this.CurrentBlock = this.CurrentBlock.SetTopLeft (this.Frame.Left, this.CurrentUsed.Bottom);    
            if (this.IntegralMode) {
                this.CurrentBlock = this.CurrentBlock.Integral();
            }
        }

        public void DoInlineLayout (float? width = null, float? height = null, SizeFlags sizeFlags = SizeFlags.None, LayoutFunc layoutFunc = null)
        {
            var frame = calcFrame(this.Frame, this.CurrentBlock, width, height, sizeFlags);
            if (layoutFunc != null) {
                frame = layoutFunc (frame);           
            }
            this.CurrentUsed = this.CurrentUsed.SetBottomRight(Math.Max(this.CurrentUsed.Right,frame.Right),Math.Max(this.CurrentUsed.Bottom,frame.Bottom));                 
            this.CurrentBlock = this.CurrentBlock.SetLeft(this.Frame.Left);
            if (this.IntegralMode) {
                this.CurrentBlock = this.CurrentBlock.Integral();
            }
        }

        private RectangleF calcFrame (RectangleF reference, RectangleF remaining, float? width, float? height, SizeFlags sizeFlags)
        {
            RectangleF result = remaining;

            if (width.HasValue) {
                if ((sizeFlags & SizeFlags.WidthPx) == SizeFlags.WidthPx) {
                    result.Width = width.Value;
                } else if ((sizeFlags & SizeFlags.WidthPercent) == SizeFlags.WidthPercent) {
                    result.Width = reference.Width * width.Value / 100f;
                }
            }

            if (height.HasValue) {
                if ((sizeFlags & SizeFlags.HeightPx) == SizeFlags.HeightPx) {
                    result.Height = height.Value;
                } else if ((sizeFlags & SizeFlags.HeightPercent) == SizeFlags.HeightPercent) {
                    result.Height = reference.Height * height.Value / 100f;
                }
            }
            return result;
        }
    }
}

