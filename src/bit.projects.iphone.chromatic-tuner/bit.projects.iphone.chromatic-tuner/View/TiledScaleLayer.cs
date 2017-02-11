using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;
using MonoTouch.UIKit;

using bit.projects.iphone.chromatictuner.model;

namespace bit.projects.iphone.chromatictuner
{
    public class TiledScaleLayer
    {
        private CALayer[] _scaleLayers;      
        private UIImage _stImage;

        public PointF Position { get { return _scaleLayers[0].Position; } set { setPosition(value); } }

        public TiledScaleLayer (MusicNotationType notationType)
        {           
            _scaleLayers = new [] { new CALayer(), new CALayer() };
            _stImage = scaleTapeImage(notationType);                   
            _scaleLayers[0].Delegate = new LayerDelegateAdapter { DrawLayerHandler = this.scaleLayer_DrawLayer };
            _scaleLayers[0].Bounds = new RectangleF(0,0,_stImage.Size.Width/2,_stImage.Size.Height);
            _scaleLayers[0].AnchorPoint = new PointF(1.0f,0.5f);           
            _scaleLayers[0].ContentsScale = UIScreen.MainScreen.Scale;
            _scaleLayers[0].MasksToBounds = true; 

            _scaleLayers[1].Delegate = new LayerDelegateAdapter { DrawLayerHandler = this.scaleLayer_DrawLayer };
            _scaleLayers[1].Bounds = new RectangleF(_stImage.Size.Width/2,0,_stImage.Size.Width/2,_stImage.Size.Height);
            _scaleLayers[1].AnchorPoint = new PointF(0.0f,0.5f);           
            _scaleLayers[1].ContentsScale = UIScreen.MainScreen.Scale;
            _scaleLayers[1].MasksToBounds = true; 
        }

        public void AddToLayer(CALayer parent)
        {
            parent.AddSublayer(_scaleLayers[0]);                               
            parent.AddSublayer(_scaleLayers[1]);                    
            _scaleLayers[0].SetNeedsDisplay();
            _scaleLayers[1].SetNeedsDisplay();
        }

        public PointF GetPresentedPosition ()
        {
            if (_scaleLayers[0].PresentationLayer != null) {
                return _scaleLayers[0].PresentationLayer.Position;
            }
            return _scaleLayers[0].Position;
        }

        public void MoveToPosition (PointF newPos, NSAction completionBlock, bool animated = false, bool linear = false, double? duration = null)
        {
            CATransaction.Begin ();

            if (!animated) {
                CATransaction.AnimationDuration = 0;               
            } else {
                if (linear) {
                    CATransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear);
                }
                if (duration.HasValue) {
                    CATransaction.AnimationDuration = duration.Value;   
                }
            }

            setPosition (newPos);
                       
            CATransaction.CompletionBlock = completionBlock;
            CATransaction.Commit ();
        }
          
        public void SetNotationType (MusicNotationType notationType)
        {         
            var image = scaleTapeImage (notationType);                  
            _stImage = image;
            _scaleLayers[0].SetNeedsDisplay();
            _scaleLayers[1].SetNeedsDisplay();                                     
        }

        private void setPosition(PointF newPos)
        {
            _scaleLayers[0].Position = _scaleLayers[1].Position = newPos;
        }

        private void scaleLayer_DrawLayer (CALayer layer, CGContext context)
        {           
            _stImage.Draw (new PointF(0,layer.Bounds.Top));               
        }

        private static UIImage scaleTapeImage (MusicNotationType notationType)
        {           
            return new UIImage(String.Format("Content/Images/scale_tape_{0}.png",notationType.Id.ToString().ToLowerInvariant()));          
        }      

    }
}

