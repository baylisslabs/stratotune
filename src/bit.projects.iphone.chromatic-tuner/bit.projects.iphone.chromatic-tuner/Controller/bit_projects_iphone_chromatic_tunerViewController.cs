using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using MonoTouch.Dialog;

using bit.shared.audio;
using bit.shared.ios.audio;
using bit.shared.logging;

using bit.shared.ios.msgbus;
using bit.projects.iphone.chromatictuner.model;

namespace bit.projects.iphone.chromatictuner
{
    public class bit_projects_iphone_chromatic_tunerViewController : UIViewController
    {
        //static Logger _log = LogManager.GetLogger("bit_projects_iphone_chromatic_tunerViewController");

        private TunerReadoutView _tunerReadoutView;       
        private SettingsDialogViewController _settingsDialogViewController;   
        private UINavigationController _settingsNavController;
		private AppRatingFlow _appRatingFlow;

		private IAppFeedbackService _appFeedback;
        private IUserSettingsService _userSettings;
        private IMsgBus _msgBus;       
        private IStaticGeometry _staticGeometry;

        private UIButton _masterOnButton;
        private UIButton _headPhoneButton;       

        private UIAlertView _headPhoneAlert;

        bool _headPhoneAlertShown;  
        bool _headPhoneAlertDisabled;
        bool _firstTimeViewWillAppear = true;
		bool _firstTimeViewDidAppear = true;

        public bit_projects_iphone_chromatic_tunerViewController (
            IMsgBus msgbus,       
            SupportConfigSection supportConfig,
            IUserSettingsService userSettings,
			IAppFeedbackService appFeedback)
            : base ()
        {                
            _msgBus = msgbus;
            _userSettings = userSettings; 
			_appFeedback = appFeedback;      
            _staticGeometry = StaticGeometryExtensions.CreateForScreen(UIScreen.MainScreen);
            _tunerReadoutView = new TunerReadoutView (this.View.Bounds,_staticGeometry);  
            _tunerReadoutView.RequestNoteChange += handleRequestNoteChange;   
            _tunerReadoutView.SliderValueChanged += handleSliderValueChanged;
            _tunerReadoutView.NoteLockValueChanged += handleNoteLockValueChanged;

            _settingsNavController = new UINavigationController();
            _settingsDialogViewController = SettingsDialogElement.MakeWithViewController(_msgBus,_userSettings,supportConfig);           
            _settingsNavController.PushViewController(_settingsDialogViewController,false);           
                

            var settingsIcon = new UIImage("Content/Images/19-gear.png");         
            var onOffIcon = new UIImage("Content/Images/51-power.png");
            var onOffIconGlow = new UIImage("Content/Images/51-power-glow.png");
            var headphoneIcon = new UIImage("Content/Images/120-headphones.png");
            var headphoneIconGlow = new UIImage("Content/Images/120-headphones-glow.png");

            var settingsButton = new UIButton(UIButtonType.Custom);
            settingsButton.SetImage(settingsIcon,UIControlState.Normal);
            settingsButton.Frame = _staticGeometry.settings_btn;
            settingsButton.TouchUpInside += settingsButtonPressed;
            settingsButton.ShowsTouchWhenHighlighted = true;

            _masterOnButton = new UIButton(UIButtonType.Custom);
            _masterOnButton.SetImage(onOffIcon,UIControlState.Normal);
            _masterOnButton.SetImage(onOffIconGlow,UIControlState.Highlighted);
            _masterOnButton.Frame = _staticGeometry.power_btn;
            _masterOnButton.TouchUpInside += masterOnButtonPressed;                     

            _headPhoneButton = new UIButton(UIButtonType.Custom);
            _headPhoneButton.SetImage(headphoneIcon,UIControlState.Normal);
            _headPhoneButton.SetImage(headphoneIconGlow,UIControlState.Highlighted);
            _headPhoneButton.Frame = _staticGeometry.headphone_btn;
            _headPhoneButton.TouchUpInside += headPhoneButtonPressed;         
                            
            _headPhoneAlert = createHeadphoneAlertView(false);
			_appRatingFlow = new AppRatingFlow (_appFeedback, supportConfig);                     
                                                  
            this.View.AddSubview (_tunerReadoutView);
            this.View.AddSubview (settingsButton);
            this.View.AddSubview (_masterOnButton);
            this.View.AddSubview (_headPhoneButton);           

            _msgBus.Subscribe<TunerPitchDataMsg>(processTunerPitchDataMsg);
            _msgBus.Subscribe<TunerStatusMsg>(processTunerStatusMsg);
        }
                                                         
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
            
            // Release any cached data, images, etc that aren't in use.
        }
               
        public void WillEnterForeground ()
        {
			if (_appFeedback.IncrementUsage ()) {
				_appRatingFlow.ShowRatingFlow ();
			}
        }

        public void DidEnterBackground ()
        {
            _tunerReadoutView.StopMoving();
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
            this.NavigationController.SetToolbarHidden (true, animated);
            this.NavigationController.SetNavigationBarHidden (true, animated);           
			_appRatingFlow.SetViewController (this);

            var us = _userSettings.RetrieveCurrent ();
            _tunerReadoutView.SetNotationType (us.Notation);
            _tunerReadoutView.SetTranspositionType (us.Transposition);           

            _msgBus.Publish (new TunerSettingsChangeMsg {
                A4Calibration = us.A4Calibration,
                Damping = us.NeedleDamping.Alpha,           
                PitchPipeWaveform = us.PitchPipeWaveform
            });

			if (_firstTimeViewWillAppear) {           
                _msgBus.Publish ( new TunerControlChangeMsg { MasterEnable = FlipFlopOp.Set });                        
				_firstTimeViewWillAppear = false;

                var sc = _userSettings.GetSystemConfig ();
                if (sc!=null) {
                    _headPhoneAlertDisabled = sc.HeadPhoneAlertDisabled;
                }
            }
        }

        public override void ViewWillDisappear (bool animated)
        {           
            base.ViewWillDisappear (animated);
            _tunerReadoutView.StopMoving();
			_appRatingFlow.SetViewController (null);
        }

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			if (_firstTimeViewDidAppear) {
				_firstTimeViewDidAppear = false;
				if (_appFeedback.IncrementUsage ()) {
					_appRatingFlow.ShowRatingFlow ();
				}
			}
		}

        private void handleRequestNoteChange (MidiNote newNote)
        {           
            _msgBus.Publish (new TunerSettingsChangeMsg {               
                PitchPipeNote = newNote                 
            });                  
        }
            
        private void processTunerPitchDataMsg (TunerPitchDataMsg msg)
        {                      
            _tunerReadoutView.NoteValue = msg.DetectedNote;               
        }     

        private void processTunerStatusMsg (TunerStatusMsg msg)
        {                       
            bool showHeadPhoneAlert = msg.OutputIsBuiltInSpeaker && msg.Mode == TunerMode.AutoPitchPipe;          
            _headPhoneButton.Enabled = msg.MasterEnable && !msg.IsInterrupted && showHeadPhoneAlert;                            
            _masterOnButton.Enabled = !msg.IsInterrupted;
            _masterOnButton.Highlighted = !msg.IsInterrupted && msg.MasterEnable;
            _headPhoneButton.Highlighted = showHeadPhoneAlert && msg.MasterEnable && !msg.IsInterrupted;                     
                    
            _tunerReadoutView.PowerOn = !msg.IsInterrupted && msg.MasterEnable;
            _tunerReadoutView.NoteLockSliderState =  msg.NoteLock;                     
            _tunerReadoutView.WantsNoteChange = (msg.Mode == TunerMode.PitchPipe || (msg.Mode == TunerMode.AutoPitchPipe && msg.NoteLock))
                && msg.MasterEnable && !msg.IsInterrupted;
            _tunerReadoutView.CentsFollowsInput = msg.Mode != TunerMode.PitchPipe;
            _tunerReadoutView.EnableNoteLock = msg.NoteLock && msg.MasterEnable && !msg.IsInterrupted;

            switch (msg.Mode) {
            case TunerMode.Tuning: _tunerReadoutView.SliderState = TunerReadoutView.ModeSliderState.Tuner; break;
            case TunerMode.PitchPipe: _tunerReadoutView.SliderState = TunerReadoutView.ModeSliderState.PitchPipe; break;
            case TunerMode.AutoPitchPipe: _tunerReadoutView.SliderState = TunerReadoutView.ModeSliderState.Auto; break;
            }

            if (!_headPhoneAlert.Visible && showHeadPhoneAlert
                && !_headPhoneAlertShown && !_headPhoneAlertDisabled
                && msg.MasterEnable && !msg.IsInterrupted) {               
                _headPhoneAlert = createHeadphoneAlertView(true);
                _headPhoneAlert.Show ();
                _headPhoneAlertShown = true;
            } else if (_headPhoneAlert.Visible && !showHeadPhoneAlert) {
                _headPhoneAlert.DismissWithClickedButtonIndex (0, true);
            }
            if (!showHeadPhoneAlert) {
                _headPhoneAlertShown = false;
            }              
        }   

        private void handleSliderValueChanged (object s, EventArgs e)
        {
            TunerMode newMode = default(TunerMode);
            switch (_tunerReadoutView.SliderState) {
            case  TunerReadoutView.ModeSliderState.Tuner: newMode = TunerMode.Tuning; break;
            case  TunerReadoutView.ModeSliderState.PitchPipe: newMode = TunerMode.PitchPipe; break;
            case  TunerReadoutView.ModeSliderState.Auto: newMode = TunerMode.AutoPitchPipe; break;
            }

            _msgBus.Publish(new TunerControlChangeMsg {
                Mode = newMode            
            }); 
        }

        private void handleNoteLockValueChanged (object s, EventArgs e)
        {
            if (_tunerReadoutView.NoteLockSliderState) {
                _msgBus.Publish (new TunerControlChangeMsg {
                    NoteLock = FlipFlopOp.Set           
                });
            } else {
                _msgBus.Publish (new TunerControlChangeMsg {
                    NoteLock = FlipFlopOp.Reset           
                });
            }
        }

        private void settingsButtonPressed(object s, EventArgs e)
        {                  
            _settingsDialogViewController.NavigationItem.LeftBarButtonItem = _settingsDialogViewController.DoneButton;
            _settingsNavController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
            this.NavigationController.PresentViewController(_settingsNavController,true,null);
        }
		            
        private void headPhoneButtonPressed (object s, EventArgs e)
        {                 
            _headPhoneAlert = createHeadphoneAlertView(false);
            _headPhoneAlert.Show();
        } 

        private void handleHeadphoneAlertClicked (object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 1) {
                _headPhoneAlertDisabled = true;               
                _userSettings.SetHeadPhoneAlertDisabled(true);
            }
        }
            
        private void masterOnButtonPressed (object s, EventArgs e)
        {                         
            _msgBus.Publish(new TunerControlChangeMsg {
                MasterEnable = FlipFlopOp.Toggle               
            }); 
        }  

        private UIAlertView createHeadphoneAlertView(bool showDontShowAgain)
        {
            var view = new UIAlertView(
                "Alert"
                ,"The pitch pipe output has been disabled temporarily to avoid interference with the tuner input. Please plug-in headphones."
                ,null,"OK",!showDontShowAgain ? null : new [] {"Don't show again"});
            view.Clicked += handleHeadphoneAlertClicked;
            return view;
        }

    }
}

