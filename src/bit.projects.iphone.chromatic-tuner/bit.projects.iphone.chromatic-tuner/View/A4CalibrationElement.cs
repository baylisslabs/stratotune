using System;
using System.Linq;

using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using bit.projects.iphone.chromatictuner.model;

namespace bit.projects.iphone.chromatictuner
{
    public class A4CalibrationElement : RootElement, ISettingsDialogViewControllerDelegate
    {                    
        private Section[] _sections;
        //private ElementBuilder _eb;       
        private EntryElement _entryElement;
        private SettingsDialogViewController _sdvc;

        private string _units;
        private Func<double> _getter;
        private Action<double> _setter;
        private double _min;
        private double _max;

        public A4CalibrationElement(string caption, 
                                    string units, 
                                    Func<double> getter, 
                                    Action<double> setter,                                 
                                    double min = Double.MinValue,
                                    double max = Double.MaxValue) : base(caption,0,0)
        {                
            //_eb = new ElementBuilder(()=>{});
            _units = units;
            _getter = getter;
            _setter = setter;
            _min = min;
            _max = max;
            _sections = build ();
            this.Add(_sections);
        }

        /*public static UIViewController MakeWithViewController ()
        {
            var rootElem = new A4CalibrationElement();
            return rootElem.MakeViewController();
        }*/

        public void CallGetters()
        {
            _entryElement.Value = _getter().ToString();
        }
        
        protected override UIViewController MakeViewController ()
        {
            var sdvc = new  SettingsDialogViewController(UITableViewStyle.Grouped, this,true);  
            sdvc.NavigationItem.Prompt = "Enter new value";           
            sdvc.NavigationItem.LeftBarButtonItem = sdvc.CancelButton;          
            sdvc.Delegate = this;
            return sdvc;
        }
                                                                                  
        private Section[] build()
        {          
            buildEntryElement();
          
            var sections = new [] {
                new Section() {                   
                    _entryElement
                }        
            };         
            return sections;
        }

        private void buildEntryElement()
        {
            _entryElement = new EntryElement(this.Caption,_units,_getter().ToString());    
            _entryElement.BecomeFirstResponder(true);          
            _entryElement.ShouldReturn += handleShouldReturn;
            _entryElement.ReturnKeyType = UIReturnKeyType.Done;
            _entryElement.TextAlignment = UITextAlignment.Right;        
            _entryElement.KeyboardType = UIKeyboardType.NumbersAndPunctuation;   
        }

        bool handleShouldReturn ()
        {
            if (_sdvc != null) {
                if (setValue (_entryElement.Value)) {
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


            // workaround for EntryElement alignment bug
            buildEntryElement();
            _sections[0].Clear();
            _sections[0].Add(_entryElement);

            //_entryElement.Value = _getter().ToString();                
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
            _entryElement.Value = _getter().ToString();
            sdvc.NavigationController.PopViewControllerAnimated(true);
        }

        private bool setValue (string text)
        {
            text = (text ?? string.Empty).Trim();
            double result = _getter();  
            double parsed;
            bool success = false;
            if(double.TryParse(text,out parsed)) {
                if(parsed >= _min && parsed <= _max) {                      
                    _setter(parsed);                 
                    result = parsed;
                    success = true;
                }
            }
            
            if(!success) {
                var alert = new UIAlertView(this.Caption,
                                            String.Format("Please enter a value between {0} and {1} {2}",_min,_max,_units)
                                            ,null,"OK",null);
                alert.Show();
            }

            _entryElement.Value = result.ToString();
            return success;
        }

        #endregion
    }
}

