using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;
using MonoTouch.UIKit;

using bit.shared.numerics;
using bit.shared.audio;

namespace bit.projects.iphone.chromatictuner
{
    public class CentsDialView : UIView
    {       
        private const float _semiToneRadians = 0.6f*(float)Math.PI;     
        private const float _minorTickLength = 10;       
        private const float _needleThickness = 2;
        private const float _needleExtraLength = _minorTickLength + 10;
        private const float _needleLimit = 25.5f;
        private static readonly UIColor _needleColor = UIColor.FromRGB(255,40,40);
        private static readonly UIColor _needleShadowColor = UIColor.FromRGBA(0,0,0,30);
                              
        private readonly CALayer _scaleLayer;
        private readonly CALayer _needleLayer;
        private readonly CALayer _needleShadowLayer;
        private readonly float _outerRadius;

        private MidiNote? _value;
        private MidiNote? _reference;

        public MidiNote? Reference { get { return _reference; } set { setReference (value); } }
        public MidiNote? Value { get { return _value; } set { setNote (value); } }

        public CentsDialView (RectangleF frame) : base(frame)
        {           
            this.Opaque = true;
            this.BackgroundColor = UIColor.White;           
                      
            var rect = new RectangleF (-this.Bounds.Width / 2f, -this.Bounds.Width - this.Bounds.Height / 2f, this.Bounds.Width, this.Bounds.Height);

            _outerRadius = this.Bounds.Width;
            _scaleLayer = new CALayer ();       
            var image = new UIImage("Content/Images/cents_dial.png");                                            
            _scaleLayer.Contents = image.CGImage;                
            _scaleLayer.Bounds = new RectangleF (0, 0, image.Size.Width, image.Size.Height);
            _scaleLayer.AnchorPoint = new PointF(0.5f,0.5f);
            _scaleLayer.Position = new PointF(this.Bounds.GetMidX(),this.Bounds.GetMidY());
            _scaleLayer.ContentsScale = UIScreen.MainScreen.Scale;

            _needleLayer = createNeedleLayer (rect, new PointF(0,0),_needleThickness,_needleColor);
            _needleShadowLayer = createNeedleLayer(rect, new PointF(4,2),_needleThickness+1f,_needleShadowColor);

            this.Layer.MasksToBounds = true;
            this.Layer.AddSublayer (_scaleLayer);     
            this.Layer.AddSublayer (_needleShadowLayer);
            this.Layer.AddSublayer (_needleLayer);
            _needleShadowLayer.SetNeedsDisplay();
            _needleLayer.SetNeedsDisplay (); 
        }

        private CALayer createNeedleLayer (RectangleF bounds, PointF offset, float lineThickness, UIColor color)
        {
            var layer = new CALayer();
            layer.Delegate = new LayerDelegateAdapter { 
                DrawLayerHandler = (l,c) => {
                    this.needleLayer_DrawLayer(l,c,lineThickness,color);
                }
            };
            layer.MasksToBounds = true;
            layer.Bounds = bounds;
            layer.AnchorPoint = new PointF(0.5f,-bounds.Top/bounds.Height);
            layer.Position = new PointF(this.Bounds.GetMidX(),-bounds.Top).Translate(offset);
            layer.ContentsScale = UIScreen.MainScreen.Scale;
            return layer;

        }
              
        private void setReference (MidiNote? note)
        {
            if (note.HasValue) {
                _reference = note.Value.NearestNote ();
            } else {
                _reference = null;
            }

            setNote(_value);
        }

        private void setNote (MidiNote? note)
        {
            float pitchBend = -0.40f;
            if(note.HasValue) {
                if(!_reference.HasValue) {
                    pitchBend = (float)note.Value.PitchBendFraction();       
                } else {
                    pitchBend = (float)(note.Value.NoteNumber() - _reference.Value.NoteNumber());    
                }
            }
            double theta = pitchBend * _semiToneRadians;
            var limit = _semiToneRadians*_needleLimit/100f;
            theta = Math.Max(-limit,Math.Min(limit,theta));
            var newTrans = CATransform3D.MakeRotation((float)theta,0,0,1);   
            CATransaction.Begin();
            _needleLayer.Transform = newTrans;           
            _needleShadowLayer.Transform = newTrans;
            CATransaction.Commit();
            _value = note;
        }
                   
        private void needleLayer_DrawLayer (CALayer layer, CGContext context, float lineThickness, UIColor color)
        {           
            context.SaveState ();
            context.SetLineWidth (lineThickness);
            context.SetStrokeColor (color.CGColor);
            context.SetFillColor (color.CGColor);
            context.MoveTo(0,0);
            context.AddLineToPoint(0, -(_outerRadius + _needleExtraLength));
            context.StrokePath ();          
            context.RestoreState ();
        }
    }
}

