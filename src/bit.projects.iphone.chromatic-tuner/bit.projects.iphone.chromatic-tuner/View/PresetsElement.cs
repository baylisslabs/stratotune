using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

using bit.projects.iphone.chromatictuner.model;

namespace bit.projects.iphone.chromatictuner
{
    // refactor to editable settings list and specific presets stuff
    public class PresetsElement : RootElement, ISettingsDialogViewControllerDelegate
    {
        private Section _section;      
        private IUserSettingsService _usc;      
        private UIViewController _addPresetsViewController;

        private class PresetRadioElement : RadioElement
        {
            public int Id { get; set; }
            public bool DisallowDelete { get; set; }
            public PresetRadioElement(UserSettings us) : base(us.Name,"presets") {
                this.Id = us.Id;
                this.DisallowDelete = us.DisallowDelete;
            }
        }

        public PresetsElement (IUserSettingsService usc) : base("Profiles", new RadioGroup("presets",0))
        {           
            _usc = usc;
            _section = new Section ();
            _addPresetsViewController = AddPresetElement.MakeWithViewController(usc);
            populateList();
            this.Add(_section);           
            setRadioSelected(_usc.RetrieveCurrentId());
        }

        public void Update(int id)
        {           
            setRadioSelected(id);
        }
                             
        protected override UIViewController MakeViewController ()
        {
            var sdvc = new  SettingsDialogViewController(UITableViewStyle.Plain, this,true);          
            sdvc.Delegate = this;
            return sdvc;
        }
                                    
        protected override void PrepareDialogViewController (UIViewController vc)
        {
            base.PrepareDialogViewController (vc);
            var sdvc = vc as SettingsDialogViewController;
            if (sdvc != null) {               
                sdvc.NavigationItem.RightBarButtonItem = sdvc.EditButton;
            }
        }               
                                    
        private void setRadioSelected (int id)
        {
            for(int i=0;i<_section.Count;++i) {
                var rb = _section[i] as PresetRadioElement;
                if(rb!=null&&rb.Id==id) {
                    this.RadioSelected = i;
                    return;
                }
            }          
        }

        private int getSelectedKey ()
        {
            int selected = this.RadioSelected;
            if (selected < _section.Count) {
                var elem = _section [selected];
                var rb = elem as PresetRadioElement;
                if (rb != null) {
                    var id = rb.Id;                  
                    return id;
                }
            }
            return 0;
        }

        private void populateList ()
        {
            var allPresets = _usc.RetrieveAll ();
            if (allPresets != null) {
                foreach (var preset in allPresets) {
                    var re = new PresetRadioElement (preset);
                    re.Tapped += () => _usc.SetCurrent(getSelectedKey ());
                    _section.Add (re);
                }
            }
        }
              
        #region ISettingsDialogViewControllerDelegate implementation
        
        void ISettingsDialogViewControllerDelegate.CommitEditingStyle (SettingsDialogViewController sdvc, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            if (editingStyle == UITableViewCellEditingStyle.Delete) {
                if(indexPath.Section==0) {
                    if(indexPath.Row<_section.Count) {
                        var elem = _section [indexPath.Row];
                        var rb = elem as PresetRadioElement;
                        if(rb!=null) {
                            int newSelectedKey;
                            if(_usc.Delete(rb.Id,out newSelectedKey)) {
                                _section.Remove(elem);
                                setRadioSelected(newSelectedKey);
                            }
                        }                                             
                        sdvc.ReloadData();
                    }
                }
            }
        }
        
        UITableViewCellEditingStyle ISettingsDialogViewControllerDelegate.EditingStyleForRow (SettingsDialogViewController sdvc, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0) {
                if (indexPath.Row<_section.Count) {
                    var elem = _section [indexPath.Row];
                    var rb = elem as PresetRadioElement;
                    if(rb!=null) {
                        if(rb.DisallowDelete==false) {
                            return UITableViewCellEditingStyle.Delete;
                        } 
                    }
                }
            }
            return UITableViewCellEditingStyle.None;
        }
        
        void ISettingsDialogViewControllerDelegate.ViewWillDisappear (SettingsDialogViewController sdvc, bool animated)
        {
        }
        
        void ISettingsDialogViewControllerDelegate.ViewWillAppear (SettingsDialogViewController sdvc, bool animated)
        {
            _section.Clear();
            populateList();
            setRadioSelected(_usc.RetrieveCurrentId());
            sdvc.ReloadData();
        }

        void ISettingsDialogViewControllerDelegate.AddButtonClicked(SettingsDialogViewController sdvc) 
        {                                
            sdvc.NavigationController.PushViewController(_addPresetsViewController,true);           
        }

        void ISettingsDialogViewControllerDelegate.DoneButtonClicked(SettingsDialogViewController sdvc) 
        {                
            sdvc.NavigationItem.RightBarButtonItem = sdvc.EditButton;          
        }

        void ISettingsDialogViewControllerDelegate.EditButtonClicked (SettingsDialogViewController sdvc) 
        {
            sdvc.SetEditing(true,true);                  
        }

        void ISettingsDialogViewControllerDelegate.CancelButtonClicked(SettingsDialogViewController sdvc) 
        {

        }
        
        #endregion
    }
}

