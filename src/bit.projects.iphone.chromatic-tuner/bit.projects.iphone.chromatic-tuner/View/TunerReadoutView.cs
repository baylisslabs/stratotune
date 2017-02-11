using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;

using bit.shared.numerics;
using bit.shared.audio;
using bit.projects.iphone.chromatictuner.model;

namespace bit.projects.iphone.chromatictuner
{
	public class TunerReadoutView : UIView
	{     
        private static readonly UIColor _backlightColor = UIColor.White;
        private static readonly UIColor _backlightOffColor = UIColor.FromWhiteAlpha(0.5f,1);

        private ScaleTapeView _scaleTapeView;
        private CentsDialView _centsDialView;
        private SliderSwitch _sliderSwitch;
        private SliderSwitch _noteLock;

        private TranspositionType _transpositionType;       
        private MidiNote? _detectedNote;
        private bool _powerOn;
        private bool _inRequestNoteChange;
        private bool _wantsNoteChange;
        private bool _centsFollowsInput;

        public enum ModeSliderState
        {
            Tuner = 0,
            Auto = 1,
            PitchPipe = 2
        }
              
        public MidiNote? NoteValue {  set { setNoteValue(value); } }      
        public bool WantsNoteChange {  get { return _wantsNoteChange; } set { setWantsNoteChange(value); } }
        public bool CentsFollowsInput { get { return _centsFollowsInput; } set { setCentsFollowsInput(value); } }
        public bool EnableNoteLock {  get { return _scaleTapeView.NoteLock; } set { _scaleTapeView.NoteLock = value; } }
        public ModeSliderState SliderState { get { return (ModeSliderState)_sliderSwitch.IntValue; } set { _sliderSwitch.IntValue = (int)value; } }
        public bool NoteLockSliderState { get { return !_noteLock.BooleanValue; } set { _noteLock.BooleanValue = !value; } }
        public bool PowerOn { get { return _powerOn; } set { setPowerState(value); } }

        public event ScaleTapeView.RequestNoteChangeDelegate RequestNoteChange;
        public event SliderSwitch.ValueChangedDelegate SliderValueChanged;
        public event SliderSwitch.ValueChangedDelegate NoteLockValueChanged;
               
		public TunerReadoutView (RectangleF frame, IStaticGeometry staticGeometry) :  base(frame)
		{                    
            this.Opaque = true;
            this.BackgroundColor = _backlightOffColor;
                      
            _centsDialView = new CentsDialView (staticGeometry.cents_dial_cutout);
            _centsDialView.BackgroundColor = _backlightOffColor;
            _scaleTapeView = new ScaleTapeView(staticGeometry.scale_tape_cutout);                            
            _scaleTapeView.RequestNoteChange += handleRequestNoteChange;
            _scaleTapeView.BackgroundColor = _backlightOffColor;
            _sliderSwitch = new SliderSwitch(staticGeometry.mode_switch_cutout,"Content/Images/mode_switch_slider.png",3,20f);
            _sliderSwitch.ValueChanged += handleSliderValueChanged;
            _noteLock = new SliderSwitch(staticGeometry.notelock_switch_cutout,"Content/Images/notelock_slider.png",2,5f);
            _noteLock.ValueChanged += handleNoteLockValueChanged;
            _noteLock.BooleanValue = true;           
            _detectedNote = null;
            _transpositionType = TranspositionTypeExtensions.FromId(TranspositionType.Enum.C);

            var panelImg = UIScreen.MainScreen.Load_568hIfWideScreen("Content/Images/panel_for_view.png");
            var panelImgView = new UIImageView(panelImg);
            panelImgView.Opaque = false;

            this.AddSubview(_sliderSwitch);   
            this.AddSubview(_noteLock);  
            this.AddSubview(_centsDialView);  
            this.AddSubview(_scaleTapeView);             
            this.AddSubview(panelImgView);
            this.BringSubviewToFront(panelImgView);

            _transpositionType = TranspositionTypeExtensions.FromId(TranspositionType.Enum.C);
        }
                                                                                                    
        public void SetNotationType (MusicNotationType notationType)
        {
            if (notationType != null) {
                _scaleTapeView.SetNotationType (notationType);
            }
        }

        public void SetTranspositionType(TranspositionType transpositionType)
        {
            if (transpositionType != null) {
                _transpositionType = transpositionType;

                if (_wantsNoteChange && this.RequestNoteChange != null) {
                    var newActualNote = _scaleTapeView.GetDisplayedNote().Transpose (_transpositionType.SemiToneShift);
                    this.RequestNoteChange (newActualNote);
                }    
            }
        }

        public void StopMoving()
        {
            _scaleTapeView.StopMoving();
        }

        private void setWantsNoteChange (bool newValue)
        {
            _wantsNoteChange = newValue;
            if (_wantsNoteChange && this.RequestNoteChange != null) {
                var newActualNote = _scaleTapeView.GetDisplayedNote().Transpose (_transpositionType.SemiToneShift);
                this.RequestNoteChange (newActualNote);
            }  
        }

        private void setCentsFollowsInput (bool newValue)
        {
            _centsFollowsInput = newValue;
            updateCentsView();
        }

        private void setNoteValue (MidiNote? note)
        {
            _detectedNote = note;          
            updateViews();           
        }

        private void setPowerState (bool on)
        {
            if (on != _powerOn) {       
                var to_color = on ? _backlightColor : _backlightOffColor;
                float duration = 0.25f;
                UIView.Animate (duration,()=>{
                    _centsDialView.BackgroundColor = to_color;
                    _scaleTapeView.BackgroundColor = to_color;
                });                                
                _powerOn = on;
                updateCentsView();
            }
        }

        private void updateViews ()
        {           
            if (!_inRequestNoteChange && _detectedNote.HasValue) {
                _scaleTapeView.MoveToNote(_detectedNote.Value.Transpose (-_transpositionType.SemiToneShift));
            }
            updateCentsView();
        }

        private void updateCentsView ()
        {                      
            if (_powerOn && (_detectedNote.HasValue || !_centsFollowsInput)) {
                if(_scaleTapeView.NoteLock) {
                    _centsDialView.Reference = _scaleTapeView.GetDisplayedNote();
                } else {
                    _centsDialView.Reference = null;
                }
                if(_centsFollowsInput) {
                    _centsDialView.Value = _detectedNote.Value.Transpose (-_transpositionType.SemiToneShift);               
                } else {
                    _centsDialView.Value = _scaleTapeView.GetDisplayedNote();
                }
            } else {
                _centsDialView.Value = null;
            }
        }
               
        private void handleRequestNoteChange (MidiNote newNote)
        {           
            _inRequestNoteChange = true;
            try {                           
                updateViews();

                if (_wantsNoteChange && this.RequestNoteChange != null) {
                    var newActualNote = newNote.Transpose (_transpositionType.SemiToneShift);
                    this.RequestNoteChange (newActualNote);
                }    
            } finally {
                _inRequestNoteChange = false;
            }
        }     

        private void handleSliderValueChanged (object sender, EventArgs e)
        {
            if (this.SliderValueChanged!=null) {
                this.SliderValueChanged(sender,e);
            }
        }

        private void handleNoteLockValueChanged (object sender, EventArgs e)
        {
            if (this.NoteLockValueChanged!=null) {
                this.NoteLockValueChanged(sender,e);
            }
        }

	}
}

