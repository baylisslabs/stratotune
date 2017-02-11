using System;

using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;

using bit.projects.iphone.chromatictuner.model;
using bit.shared.ios.msgbus;

namespace bit.projects.iphone.chromatictuner
{
    public class SettingsDialogElement : RootElement, ISettingsDialogViewControllerDelegate
    {   
        private IMsgBus _msgBus;
        private IUserSettingsService _userSettings;
        private SupportConfigSection _supportConfig;       
        private Section[] _sections;
        private ElementBuilder _eb;
        private UserSettings _currentSettings;
        private bool _needsUpdate;
        private PresetsElement _presetsElement;
        private A4CalibrationElement _a4CalibrationElement;      
		private UIViewController _viewController;

        public SettingsDialogElement(IMsgBus msgBus,
                                     IUserSettingsService userSettings,                                    
                                     SupportConfigSection supportConfig) : base("Settings")
        {
            _msgBus = msgBus;
            _userSettings = userSettings;
            _supportConfig = supportConfig;
            _currentSettings = _userSettings.RetrieveCurrent();
            _needsUpdate = false;
            _eb = new ElementBuilder(()=>{_needsUpdate=true;});
            _sections = build ();
            this.Add(_sections);
        }


        public static SettingsDialogViewController MakeWithViewController (
            IMsgBus msgBus,
            IUserSettingsService userSettings,           
            SupportConfigSection supportConfig)
        {
            var rootElem = new SettingsDialogElement(msgBus,userSettings,supportConfig);
			rootElem._viewController = rootElem.MakeViewController();
			return (SettingsDialogViewController)rootElem._viewController;
        }

        protected override UIViewController MakeViewController ()
        {
            var sdvc = new  SettingsDialogViewController(UITableViewStyle.Grouped, this,true);                  
            sdvc.Delegate = this;             
            return sdvc;
        }
                                   
        private Section[] build()
        {           
            var sections = new [] {
                new Section() {                   
                    (_presetsElement = new PresetsElement(_userSettings))
                },          
                new Section() {
                    _eb.FromList("Tone Generator","pitch_pipe_waveform",
                                 PitchPipeWaveformTypeExtensions.Values,
                                 i=>i.Id,
                                 i=>i.Description,
                                 ()=>(_currentSettings.PitchPipeWaveform),
                                 (v)=>{_currentSettings.PitchPipeWaveform=v;
                        _msgBus.Publish(new TunerSettingsChangeMsg { PitchPipeWaveform = v});})
                },
                new Section() {
                    (_a4CalibrationElement=new A4CalibrationElement("A4 Reference","Hz",
                                   ()=>_currentSettings.A4Calibration,
                                   v=>{_currentSettings.A4Calibration=v;                                        
                                    _msgBus.Publish(new TunerSettingsChangeMsg { A4Calibration = v});
                                        _needsUpdate=true;},
                                    UserSettingsExtensions.A4CalibrationMin,
                                    UserSettingsExtensions.A4CalibrationMax)),
                    _eb.FromList("Notation Style","notation",
                                 MusicNotationTypeExtensions.Values,
                                 i=>i.Id,
                                 i=>i.Description,
                                 ()=>_currentSettings.Notation,
                                 v=>{_currentSettings.Notation=v; }),             
                    /*_eb.FromList("Temperament","temperament",
                                 TemperamentTypeExtensions.Values,
                                 i=>i.Id,
                                 i=>i.Description,
                                ()=>(_currentSettings.Temperament),
                                (v)=>{_currentSettings.Temperament=v; })*/
                    _eb.FromList("Transposition","transposition",
                                 TranspositionTypeExtensions.Values,
                                 i=>i.Id,
                                 i=>i.Description,
                                 ()=>(_currentSettings.Transposition),
                                 (v)=>{_currentSettings.Transposition=v; }),
                },
                new Section() {
                    _eb.FromList("Needle Smoothing","needle_damping",
                                 NeedleDampingTypeExtensions.Values,
                                 i=>i.Id,
                                 i=>i.Description,
                                 ()=>(_currentSettings.NeedleDamping),
                                 (v)=>{_currentSettings.NeedleDamping=v;}),
                    /*                    _eb.FromBool("Frequency Display",
                                 ()=>(_currentSettings.FrequencyDisplay),
                                 (v)=>{_currentSettings.FrequencyDisplay=v;})*/
                },
                new Section() {
                    new RootElement("Support") {
                        new Section() {
                            new StringElement("Website",()=>{openUrl(_supportConfig.UrlWebsite);}),                      
                        },
                        new Section() {
                            new StringElement("Rate this App",()=>{openUrl(_supportConfig.UrlReview);}),
							new StringElement("Email a Friend",()=>{presentMailForm(null,"Hi, Check out this iPhone App!","http://itunes.apple.com/app/id579888710");}),
							new StringElement("Contact Us",()=>{presentMailForm(_supportConfig.SupportEmail,null,null);}),
							new StringElement("Report a Problem",()=>{presentMailForm(_supportConfig.SupportEmail,"I'd like to report a problem",null);})
                        }
                    }
                    //new StringElement("More Apps by Bayliss IT"),          
                }
            };         
            return sections;
        }

        private void openUrl(string url)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
        }

		private void presentMailForm(string to, string subject, string body)
		{
			var canSend = MFMailComposeViewController.CanSendMail;
			if (_viewController!=null && canSend) {
				var _mailController = new MFMailComposeViewController ();
				if (!string.IsNullOrWhiteSpace (to)) {
					_mailController.SetToRecipients (new string[] { to });
				}
				if (!string.IsNullOrWhiteSpace (subject)) {
					_mailController.SetSubject (subject);
				}
				if (!string.IsNullOrWhiteSpace (body)) {
					_mailController.SetMessageBody (body, false);
				}
				_mailController.Finished += handleComposeMailFinished;
				_viewController.PresentViewController (_mailController, true, null);
			}
		}

		private void handleComposeMailFinished(object sender, MFComposeResultEventArgs e)
		{
			e.Controller.DismissViewController (true, null);		
		}

        #region ISettingsDialogViewControllerDelegate implementation

        void ISettingsDialogViewControllerDelegate.CommitEditingStyle (SettingsDialogViewController sdvc, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
           
        }

        UITableViewCellEditingStyle ISettingsDialogViewControllerDelegate.EditingStyleForRow (SettingsDialogViewController sdvc, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.None;
        }

        void ISettingsDialogViewControllerDelegate.ViewWillDisappear (SettingsDialogViewController sdvc, bool animated)
        {         
            if (_needsUpdate) {
                _needsUpdate = !_userSettings.Update (_currentSettings);
            }
        }

        void ISettingsDialogViewControllerDelegate.ViewWillAppear (SettingsDialogViewController sdvc, bool animated)
        {                               
            int id = _userSettings.RetrieveCurrentId ();
            if (_currentSettings.Id != id) {
                if (_needsUpdate) {
                    _userSettings.Update (_currentSettings);
                }
                _currentSettings = _userSettings.RetrieveCurrent ();              
                _presetsElement.Update(id);
                _a4CalibrationElement.CallGetters();
                _eb.CallGetters();
                sdvc.ReloadData();   

                _msgBus.Publish(new TunerSettingsChangeMsg {
                    PitchPipeWaveform = _currentSettings.PitchPipeWaveform, 
                    A4Calibration = _currentSettings.A4Calibration});

                _needsUpdate = false;
            }                 
        }

        void ISettingsDialogViewControllerDelegate.AddButtonClicked(SettingsDialogViewController sdvc) 
        {                                

        }
        
        void ISettingsDialogViewControllerDelegate.DoneButtonClicked (SettingsDialogViewController sdvc)
        {                
            sdvc.NavigationController.DismissViewController (true,null);
        }
        
        void ISettingsDialogViewControllerDelegate.EditButtonClicked (SettingsDialogViewController sdvc) 
        {
                            
        }
        
        void ISettingsDialogViewControllerDelegate.CancelButtonClicked(SettingsDialogViewController sdvc) 
        {
            
        }

        #endregion
    }
}

