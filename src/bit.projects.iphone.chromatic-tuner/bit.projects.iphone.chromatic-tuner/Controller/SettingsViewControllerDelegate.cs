using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace bit.projects.iphone.chromatictuner
{
    public interface ISettingsDialogViewControllerDelegate
    {
        void CommitEditingStyle(SettingsDialogViewController sdvc, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath);        
        UITableViewCellEditingStyle EditingStyleForRow(SettingsDialogViewController sdvc, NSIndexPath indexPath) ;        
        void ViewWillDisappear(SettingsDialogViewController sdvc, bool animated) ;
        void ViewWillAppear(SettingsDialogViewController sdvc, bool animated);
        void AddButtonClicked(SettingsDialogViewController sdvc);
        void DoneButtonClicked(SettingsDialogViewController sdvc);
        void EditButtonClicked(SettingsDialogViewController sdvc);
        void CancelButtonClicked(SettingsDialogViewController sdvc);	
    }
    
    public class NullSettingsDialogViewControllerDelegate : ISettingsDialogViewControllerDelegate
    {
        public void CommitEditingStyle(SettingsDialogViewController sdvc, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath) {}    
        public UITableViewCellEditingStyle EditingStyleForRow (SettingsDialogViewController sdvc, NSIndexPath indexPath) {
            return UITableViewCellEditingStyle.None;
        }
        public void ViewWillDisappear(SettingsDialogViewController sdvc, bool animated) {}
        public void ViewWillAppear(SettingsDialogViewController sdvc, bool animated) {}

        public void AddButtonClicked(SettingsDialogViewController sdvc) {}
        public void DoneButtonClicked(SettingsDialogViewController sdvc) {}
        public void EditButtonClicked (SettingsDialogViewController sdvc) {}
        public void CancelButtonClicked(SettingsDialogViewController sdvc) {}
    }
}

