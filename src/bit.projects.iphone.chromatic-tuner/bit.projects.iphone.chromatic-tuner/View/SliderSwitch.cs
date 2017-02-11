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
    public class SliderSwitch : UIView
    {             
        private CALayer _sliderLayer;
        private int _numStates;
        private float _ptsPerState;
        private float _slideDistPts;
        private float _slideHeight;
        private float _zeroY;
        private float _zeroX;
        private bool _moving;

        private readonly bool _readonly = false;
        private int _state;
        public int IntValue { get { return _state; } set { setValue(value); } }
        public bool BooleanValue { get { return _state != 0; } set { setValue (value ? _numStates - 1 : 0); } }
           

        public delegate void ValueChangedDelegate(object sender, EventArgs e);
        public event ValueChangedDelegate ValueChanged;

        public SliderSwitch (RectangleF frame, string imageFile, int numStates, float inflate) : base(frame)
        {           
            var touchFrame = frame;
            touchFrame.Inflate(inflate,inflate);
            this.Frame = touchFrame;
            this.Opaque = true;
            this.BackgroundColor = UIColor.FromRGB (0x84, 0x84, 0x84);           
                     
            _numStates = Math.Max(2,numStates);
            _slideHeight = frame.Height;
            _slideDistPts = frame.Width - frame.Height;
            _ptsPerState = _slideDistPts / (_numStates-1);           

            _sliderLayer = new CALayer ();          
            var image = new UIImage (imageFile);                   
            _sliderLayer.Contents = image.CGImage;                
            _sliderLayer.Bounds = new RectangleF (0, 0, image.Size.Width, image.Size.Height);
            _sliderLayer.AnchorPoint = new PointF(0.0f,0.5f);
            _sliderLayer.Position = new PointF(inflate,this.Bounds.GetMidY());
            _sliderLayer.ContentsScale = UIScreen.MainScreen.Scale;
                     
            _zeroY = _sliderLayer.Position.Y;
            _zeroX = _sliderLayer.Position.X;

            this.Layer.MasksToBounds = true;
            this.Layer.AddSublayer (_sliderLayer);                   
        } 
                       
        public override void TouchesBegan (NSSet touches, UIEvent evt)
        {           
            base.TouchesBegan (touches, evt);                                     
            _moving = false;
        }
        
        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            base.TouchesMoved (touches, evt); 
            
            UITouch touch = touches.AnyObject as UITouch;
            if (touch != null) {    
                if(!_readonly) {
                    var deltaX = touch.LocationInView(this).X - touch.PreviousLocationInView(this).X;  
                    var newX = _sliderLayer.Position.Translate(deltaX,0);
                    moveToPosition(newX,false);    
                    _moving = true;
                }
            }
        }
        
        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);              
            
            UITouch touch = touches.AnyObject as UITouch;
            if (touch != null) {               
                if(!_readonly) {      
                    if(_moving) {                          
                        _state = positionToState(_sliderLayer.Position);
                    } else {
                        var up = touch.LocationInView(this);
                        if(_numStates>2) {
                            _state = positioninViewToState(up);     
                        } else {
                            _state = 1 - _state;
                        }
                    }
                    moveToPosition(stateToPosition(_state),true);
                    if(this.ValueChanged!=null) {
                        this.ValueChanged(this, new EventArgs());
                    }                 
                }
            }
            _moving = false;
        }
        
        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {           
            moveToPosition(stateToPosition(_state),true);    
            _moving = false;
        }


        private void setValue (int newState)
        {
            if (newState != _state) {
                _state = clampState (newState);            
                moveToPosition (stateToPosition (_state),true);
            }
        }

        private void moveToPosition (PointF newPos, bool animated = false)
        {
            CATransaction.Begin ();
            if (!animated) {
                CATransaction.AnimationDuration = 0;               
            }          
            _sliderLayer.Position = clampPosition(newPos);
            CATransaction.Commit ();
        }

        private PointF stateToPosition(int state)
        {
            state = clampState(state);

            return new PointF(_zeroX-state*_ptsPerState,_zeroY);
        }

        private int positioninViewToState (PointF pos)
        {
            return positionToState(pos.Translate(-_slideDistPts-_slideHeight/2f,0));
        }

        private int positionToState (PointF pos)
        {
            float statef = (_zeroX-pos.X) / _ptsPerState;
            int state = (int)Math.Round (statef);
            return clampState(state);
        }

        private int clampState(int state)
        {
            if(state<0) {
                return 0;
            }  else if (state >= _numStates) {
                return _numStates-1;
            }
            return state;
        }

        private PointF clampPosition(PointF pos)
        {
            return new PointF(Math.Max (_zeroX-_slideDistPts,Math.Min (_zeroX,pos.X)),_zeroY);           
        }


    }
}

