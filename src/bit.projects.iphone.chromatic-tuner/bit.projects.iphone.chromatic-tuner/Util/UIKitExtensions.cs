using System;
using System.Drawing;

using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace bit.projects.iphone.chromatictuner
{
    public static class UIKitExtensions
    {
        const float WIDESCREEN_HEIGHT = 568f;

        public static bool IsWideScreen(this UIScreen screen)
        {
            return (Math.Abs(screen.Bounds.Height-WIDESCREEN_HEIGHT)<=float.Epsilon);
        }       

        public static UIImage Load_568hIfWideScreen(this UIScreen screen, string pngName)
        {
            if (screen.IsWideScreen()) {
                return new UIImage(pngName.Replace(".png","_568h.png"));
            } else {
                return new UIImage(pngName);
            }
        }
    }
}

