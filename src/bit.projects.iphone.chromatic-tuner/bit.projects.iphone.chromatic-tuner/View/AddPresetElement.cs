using System;
using System.Linq;

using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using bit.projects.iphone.chromatictuner.model;

namespace bit.projects.iphone.chromatictuner
{
    public class AddPresetElement : RootElement, ISettingsDialogViewControllerDelegate
    {   
        private IUserSettingsService _userSettings;
          
        private Section[] _sections;
        //private ElementBuilder _eb;       
        private EntryElement _nameElement;
        private SettingsDialogViewController _sdvc;

        public AddPresetElement(IUserSettingsService userSettings) : base("Add Profile")
        {
            _userSettings = userSettings;           
            //_eb = new ElementBuilder(()=>{});
            _sections = build ();
            this.Add(_sections);
        }

        public static UIViewController MakeWithViewController (IUserSettingsService userSettings)
        {
            var rootElem = new AddPresetElement(userSettings);
            return rootElem.MakeViewController();
        }
        
        protected override UIViewController MakeViewController ()
        {
            var sdvc = new  SettingsDialogViewController(UITableViewStyle.Grouped, this,true);  
            sdvc.NavigationItem.Prompt = "Enter new profile name";           
            sdvc.NavigationItem.LeftBarButtonItem = sdvc.CancelButton;          
            sdvc.Delegate = this;
            return sdvc;
        }
                                       
        private Section[] build()
        {          
            _nameElement = new EntryElement("Name","Name",null,false);
            _nameElement.BecomeFirstResponder(true);
            _nameElement.AutocorrectionType = UITextAutocorrectionType.No;          
            _nameElement.ShouldReturn += handleShouldReturn;
            _nameElement.ReturnKeyType = UIReturnKeyType.Done;
            var sections = new [] {
                new Section() {                   
                    _nameElement
                }        
            };         
            return sections;
        }

        bool handleShouldReturn ()
        {
            if (_sdvc != null) {
                if (addPreset (_nameElement.Value)) {
                    _sdvc.NavigationController.PopViewControllerAnimated (true);
                    return true;
                }
            }
            return false;        
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

        }

        void ISettingsDialogViewControllerDelegate.ViewWillAppear (SettingsDialogViewController sdvc, bool animated)
        {
            _sdvc = sdvc;
            _nameElement.Value = null;
        }

        void ISettingsDialogViewControllerDelegate.AddButtonClicked (SettingsDialogViewController sdvc)
        {                                
           
        }
        
        void ISettingsDialogViewControllerDelegate.DoneButtonClicked(SettingsDialogViewController sdvc) 
        {                
                 
        }
        
        void ISettingsDialogViewControllerDelegate.EditButtonClicked (SettingsDialogViewController sdvc) 
        {
                          
        }
        
        void ISettingsDialogViewControllerDelegate.CancelButtonClicked(SettingsDialogViewController sdvc) 
        {
            sdvc.NavigationController.PopViewControllerAnimated(true);
        }

        private bool addPreset (string name)
        {
            name = (name ?? string.Empty).Trim();           
            if (string.IsNullOrWhiteSpace (_nameElement.Value)) {
                var alert = new UIAlertView ("Name required", "Please enter a profile name", null, "OK", null);
                alert.Show ();
                return false;
            }
            var allPresets = _userSettings.RetrieveAll ();
            if (allPresets.Any (p => string.Compare (name, p.Name, StringComparison.OrdinalIgnoreCase) == 0)) {
                var alert = new UIAlertView ("Sorry", "That name is already used, please try another", null, "OK", null);
                alert.Show ();
                return false;
            }
                      
            var preset = _userSettings.Add (name);
            if (preset != null) {               
                _userSettings.SetCurrent(preset.Id);
                return true;
            } else {
                var alert = new UIAlertView ("Sorry", "There was a problem adding the new profile, please try again", null, "OK", null);
                alert.Show ();
                return false;
            }
        }

        #endregion
    }
}

