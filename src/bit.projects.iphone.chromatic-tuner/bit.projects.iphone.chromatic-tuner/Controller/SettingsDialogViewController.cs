using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace bit.projects.iphone.chromatictuner
{
    public class SettingsDialogViewController : DialogViewController
    {
        public UIBarButtonItem AddButton { get; private set; }
        public UIBarButtonItem DoneButton { get; private set; }
        public UIBarButtonItem EditButton { get; private set; }
        public UIBarButtonItem CancelButton { get; private set; }

        public ISettingsDialogViewControllerDelegate Delegate { get; set; }

        private class _Source : DialogViewController.Source
        {
            private SettingsDialogViewController _sdvc;
            public _Source(SettingsDialogViewController dvc) : base(dvc)
            {
                _sdvc = dvc;
            }

            public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {                                       
                _sdvc.Delegate.CommitEditingStyle (_sdvc, editingStyle, indexPath);               
            }

            public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
            {               
                return _sdvc.Delegate.EditingStyleForRow (_sdvc, indexPath);
            }
        }
                         
        public SettingsDialogViewController (RootElement element, bool pushing) : this(UITableViewStyle.Grouped,element,pushing)
        {

        }

        public SettingsDialogViewController (UITableViewStyle style, RootElement element, bool pushing) : base(style,element,pushing)
        {
            this.AddButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, addPressed);
            this.DoneButton = new UIBarButtonItem (UIBarButtonSystemItem.Done, donePressed);
            this.EditButton = new UIBarButtonItem (UIBarButtonSystemItem.Edit, editPressed);
            this.CancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, cancelPressed);
            this.Delegate = new NullSettingsDialogViewControllerDelegate();
        }
             
        public override DialogViewController.Source CreateSizingSource (bool unevenRows)
        {
            //return base.CreateSizingSource (unevenRows);
            if (!unevenRows) {
                return new _Source (this);
            } else {
                return new SizingSource(this);
            }           
        }
                             
        public override void ViewWillDisappear (bool animated)
        {
            base.ViewWillDisappear (animated);
            this.Delegate.ViewWillDisappear (this, animated);          
        }  

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);         
            this.Delegate.ViewWillAppear (this, animated);           
        }  
               
        public override void SetEditing (bool editing, bool animated)
        {
            base.SetEditing (editing, animated);    
            if (editing == true) {
                this.NavigationItem.LeftBarButtonItem = this.DoneButton;
                this.NavigationItem.RightBarButtonItem = this.AddButton;
            } else {
                this.NavigationItem.LeftBarButtonItem = null;  
                this.NavigationItem.RightBarButtonItem = null;  
            }
        }
                              
        private void addPressed (object s, EventArgs e)
        {
            this.Delegate.AddButtonClicked(this);
        }

        private void donePressed (object s, EventArgs e)
        {
            if (this.Editing) {
                this.SetEditing (false, true);
            }
            this.Delegate.DoneButtonClicked(this);
        }

        private void editPressed (object s, EventArgs e)
        {
            this.Delegate.EditButtonClicked(this);
        }

        private void cancelPressed (object s, EventArgs e)
        {
            this.Delegate.CancelButtonClicked(this);
        }
    }
}

