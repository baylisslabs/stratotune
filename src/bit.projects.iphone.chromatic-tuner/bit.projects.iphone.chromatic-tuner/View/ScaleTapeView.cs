using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;
using MonoTouch.UIKit;

using bit.shared.logging;
using bit.shared.numerics;
using bit.shared.audio;
using bit.projects.iphone.chromatictuner.model;

namespace bit.projects.iphone.chromatictuner
{
    public class ScaleTapeView : UIView
    {
        //static Logger _log = LogManager.GetLogger("bit_projects_iphone_chromatic_tunerScaleTapeView");
               
        private static readonly UIColor _needleColor = UIColor.FromRGB(255,40,40);
        private static readonly UIColor _needleShadowColor = UIColor.FromRGBA(0,0,0,30);
        private static readonly UIColor _bgColor = UIColor.White;

        private const float _needleThickness = 1;
        private const float _semiTonePts = 100;       
        private const int _semiTones = MidiNote.SEMI_TONES_PER_OCTAVE;  
        private const float _octavePts = _semiTonePts * _semiTones;              
        private const double _swipeThresholdSecs = 0.025;
        private const float _limitStopX = 2 * _semiTonePts;
        private const float _panDampingExp = 0.95f;
        private const float _panDamplingLin = 10f;
        private const float _limitDampingExp = 0.2f;
        private const float _limitDamplingLin = 50f;
        private const double _updatePanRateSecs = 1d/20;
        private const float _resistDrag = 5;
        private const float _noteLockVelocityThresh = 400;
        private const double _springBackAnimDuration = 0.5;
        private static readonly MidiNote _zeroC = MidiNoteExtensions.Notes.C5;   

        private readonly TiledScaleLayer _scaleLayer;
        private readonly CALayer _needleLayer;
       
        private readonly float _zeroX;
        private readonly float _zeroY;
        private readonly float _minX;
        private readonly float _maxX;
                     
        private MusicNotationType _notationType; 
         
        private TouchRecord _lastTouch;     
        private float _panVelocity;       
        private float _lastNoteSentX;
              
        public bool NoteLock { get; set; }
        public delegate void RequestNoteChangeDelegate(MidiNote newNote);
        public event RequestNoteChangeDelegate RequestNoteChange;
         
        public ScaleTapeView (RectangleF frame) : base(frame)
        {           
            this.Opaque = true;
            this.BackgroundColor = _bgColor;          
            this.Layer.MasksToBounds = true;

            var vp_rect = new RectangleF(-this.Bounds.Width/2,0,this.Bounds.Width,this.Bounds.Height);          

            _notationType = MusicNotationTypeExtensions.FromId(default(MusicNotationType.Enum));
            _scaleLayer = new TiledScaleLayer(_notationType);                                   
            _scaleLayer.Position = new PointF(this.Bounds.GetMidX(),this.Bounds.GetMidY());
            _zeroX = _scaleLayer.Position.X;
            _zeroY = _scaleLayer.Position.Y;          

            _needleLayer = new CALayer();
            _needleLayer.Delegate = new LayerDelegateAdapter { DrawLayerHandler = this.needleLayer_DrawLayer };
            _needleLayer.MasksToBounds = true;
            _needleLayer.Bounds = vp_rect;
            _needleLayer.AnchorPoint = new PointF(0.5f,0.5f);
            _needleLayer.Position = new PointF(this.Bounds.GetMidX(),this.Bounds.GetMidY());
            _needleLayer.ContentsScale = UIScreen.MainScreen.Scale;         

            var pgr=new UIPanGestureRecognizer(onPanGesture);
            pgr.CancelsTouchesInView = false;
            this.AddGestureRecognizer(pgr);

            var tgr = new UITapGestureRecognizer(onTapGesture);
            tgr.CancelsTouchesInView = false;
            tgr.NumberOfTapsRequired = 2;
            this.AddGestureRecognizer(tgr);
                  
            NSTimer.CreateRepeatingScheduledTimer(_updatePanRateSecs,onUpdatePanTick);

            _minX = (float)noteToPosition(MidiNoteExtensions.Notes.C9);
            _maxX = (float)noteToPosition(MidiNoteExtensions.Notes.C0);

            setNote(MidiNoteExtensions.Notes.C4,false,true);
                    
            _scaleLayer.AddToLayer(this.Layer);    
            this.Layer.AddSublayer (_needleLayer);           
            _needleLayer.SetNeedsDisplay ();            
        }

        public void SetNotationType (MusicNotationType notationType)
        {
            _notationType = notationType;
            _scaleLayer.SetNotationType(_notationType);
        }

        public override void TouchesBegan (NSSet touches, UIEvent evt)
        {           
            base.TouchesBegan (touches, evt);              
          
            UITouch touch = touches.AnyObject as UITouch;
            if (touch != null) {               
                setPanVelocity(0);
                _lastTouch = new TouchRecord(this,touch);
            }
        }

        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            base.TouchesMoved (touches, evt); 
            
            UITouch touch = touches.AnyObject as UITouch;
            if (touch != null) {               
                var deltaX = touch.LocationInView(this).X - touch.PreviousLocationInView(this).X;  
                if((_scaleLayer.Position.X < _minX) || (_scaleLayer.Position.X > _maxX)) {
                    deltaX /= _resistDrag;
                }
                var newX = _scaleLayer.Position.X + deltaX; 
                moveToPosition(newX,false);               
                _lastTouch = new TouchRecord(this,touch);
            }
        }

        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);              
         
            UITouch touch = touches.AnyObject as UITouch;
            if (touch != null) {                                              
                if(_lastTouch!=null&&(touch.Timestamp-_lastTouch.Timestamp)<_swipeThresholdSecs) {
                    //_log.Debug("SwipeFinish(v={0}px/s)",_lastTouch.VelocityInView);
                    if((_scaleLayer.Position.X < _minX) || (_scaleLayer.Position.X > _maxX)) {
                        setPanVelocity(_lastTouch.VelocityInView.X/_resistDrag);
                    } else {
                        setPanVelocity(_lastTouch.VelocityInView.X);
                    }
                }              
            }
                                           
            _lastTouch = null;
        }
                           
        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled (touches, evt);
            _lastTouch = null;
        }

        public void StopMoving ()
        {
            setPanVelocity(0);
        }

        public void MoveToNote (MidiNote note)
        {
            setNote(note,true,false);
        }

        public MidiNote GetDisplayedNote ()
        {
            return noteFromPosition (getPresentedPosition ().X);
        }

        private void onPanGesture (UIPanGestureRecognizer pgr)
        {
            //_log.Debug ("onPanGesture({0},{1},{2})", pgr.VelocityInView (this), pgr.TranslationInView (this), pgr.State);
            if (_lastTouch != null) {
                _lastTouch.VelocityInView = pgr.VelocityInView(this);
            }
        }

        private void onTapGesture (UITapGestureRecognizer tgr)
        {
            //_log.Debug ("onTapGesture({0})", tgr.LocationInView (this));
            if (_panVelocity == 0) {    
                var pos_x = getPresentedPosition ().X;
                var dir = this.Bounds.GetMidX()-tgr.LocationInView(this).X;
                if(dir<0) {
                    if (pos_x > _minX) {                  
                        moveToPosition (pos_x-_octavePts, false, false, null);
                    }
                } else {
                    if (pos_x < _maxX) {                  
                        moveToPosition (pos_x+_octavePts, false, false, null);
                    }                
                }              
            }

        }

        private void animationCompleted ()
        {
            if (_lastTouch == null) {
                springBackAtEnds();
            }
        }

        /* todo - check cpu usage impact */
        private void onUpdatePanTick ()
        {     
            var pos_x = getPresentedPosition ().X;

            if (_lastTouch == null) {
                doNoteLock();
                springBackAtEnds (); 
                if (_panVelocity != 0) {                   
                    if ((pos_x <= _minX) || 
                        (pos_x >= _maxX)) {
                        setPanVelocity (apply_damping (_panVelocity, _limitDampingExp, _limitDamplingLin));           
                    } else {                       
                        setPanVelocity (apply_damping (_panVelocity, _panDampingExp, _panDamplingLin));                   
                    }
                } 
            }
                               
            if(pos_x!=_lastNoteSentX) {
                var note = noteFromPosition (pos_x);
                this.RequestNoteChange (note);  
                _lastNoteSentX = pos_x;
            }           
        }

        private float apply_damping (float velocity, float expFactor, float linFactor)
        {
            var abs_v = Math.Abs (velocity);                 
            var next_v = Math.Max (0, abs_v * expFactor - linFactor);
            return next_v * Math.Sign (velocity);  
        }

        private void springBackAtEnds ()
        {
            if (_panVelocity == 0) {
                var pos_x = getPresentedPosition ().X;
                if (pos_x < _minX) {                  
                    moveToPosition (_minX, true, false, _springBackAnimDuration);
                } else if (pos_x > _maxX) {                  
                    moveToPosition (_maxX, true, false, _springBackAnimDuration);                    
                } 
            }
        }

        private void doNoteLock ()
        {
            if (this.NoteLock && Math.Abs(_panVelocity) < _noteLockVelocityThresh) {
                var pos_x = getPresentedPosition ().X;
                if (pos_x >= _minX && pos_x <= _maxX) {
                    var note_at_x = noteFromPosition(pos_x);
                    MidiNote nearest_note;
                    if(_panVelocity==0) {
                        nearest_note = note_at_x.NearestNote();
                    } else {
                        nearest_note = MidiNote.FromNote(_panVelocity > 0 ? Math.Floor (note_at_x.NoteNumber()) : Math.Ceiling(note_at_x.NoteNumber()));
                    }
                    if(note_at_x!=nearest_note) {
                        setNote(nearest_note,true,true,_springBackAnimDuration);
                    }
                }                  
            }
        }
                            
        private MidiNote noteFromPosition (float pos_x)
        {                   
            return MidiNote.FromNote(_zeroC.NoteNumber() - ((pos_x - _zeroX) / _semiTonePts));
        }

        private float noteToPosition (MidiNote note)
        {
            return (float)(_zeroX + (_zeroC.NoteNumber() - note.NoteNumber()) * _semiTonePts);          
        }
              
        private void setNote (MidiNote note, bool animated, bool overrideNoteLock, double? duration = null)
        {                
            if (overrideNoteLock || this.NoteLock == false) {
                var newX = noteToPosition (note);              

                if (newX != _scaleLayer.Position.X) {
                    /* stop any current panning */
                    _panVelocity = 0;
                                                 
                    /* animate to destination */
                    moveToPosition (newX, animated, false, duration);
                }
            }
        }

        private void moveToPosition (float pos_x, bool animated = false, bool linear = false, double? duration = null)
        {
            //_log.Trace("moveToPosition: {0},{1},{2}",pos_x,animated,linear);
            _scaleLayer.MoveToPosition(new PointF (pos_x, _zeroY),animationCompleted,animated,linear,duration);           
        }

        private void setPanVelocity (float vel_x)
        {
            var curr_x = getPresentedPosition().X;

            if (vel_x == 0) {
                moveToPosition(curr_x,false);
            }
            else {               
                var end_x = (vel_x>0)?_maxX+_limitStopX:_minX-_limitStopX;            
                var delta_t = Math.Abs((curr_x-end_x)/vel_x);
                moveToPosition(curr_x,false);
                moveToPosition(end_x,true,true,delta_t);              
            }    
            _panVelocity = vel_x;
        }       

        private PointF getPresentedPosition ()
        {
            return _scaleLayer.GetPresentedPosition();
        }
                           
        private void needleLayer_DrawLayer (CALayer layer, CGContext context)
        {
            context.SaveState ();
            context.SetLineWidth (_needleThickness);
            context.SetStrokeColor (_needleColor.CGColor);
            context.MoveTo(0, layer.Bounds.Top);
            context.AddLineToPoint(0,layer.Bounds.Bottom);         
            context.StrokePath ();
            context.SetStrokeColor(_needleShadowColor.CGColor);
            context.SetLineWidth (_needleThickness+1);
            context.MoveTo(4, layer.Bounds.Top);
            context.AddLineToPoint(4,layer.Bounds.Bottom);
            context.StrokePath ();
            context.RestoreState ();
        }                       
    }
}

