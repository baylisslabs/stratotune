using System;

using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace bit.projects.iphone.chromatictuner
{   
    public class LayerDelegateAdapter : CALayerDelegate
    {
        public delegate void DrawLayerDelegate(CALayer layer, CGContext context);

        public DrawLayerDelegate DrawLayerHandler { get; set; }
              
        public override void DrawLayer (CALayer layer, CGContext context)
        {
            if (this.DrawLayerHandler != null) {               
                UIGraphics.PushContext (context);                
                try {
                    this.DrawLayerHandler(layer,context);
                }
                finally {
                    UIGraphics.PopContext ();      
                }
            }
        }
    }
}

