using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace bit.projects.iphone.chromatictuner
{
    public class ElementBuilder
    {
        private List<Action> _getters;
        private Action _notifyOnSet;

        public ElementBuilder(Action notifyOnSet)
        {
            _notifyOnSet = notifyOnSet;
            _getters = new List<Action>();
        }

        public void CallGetters ()
        {
            foreach (var getter in _getters) {
                getter();
            }
        }

        public BooleanElement FromBool(string caption, Func<bool> getter, Action<bool> setter)
        {
            var be = new BooleanElement(caption,getter());           
            be.ValueChanged += (object sender, EventArgs e) => { setter(be.Value); _notifyOnSet();};
            _getters.Add(()=>{be.Value=getter();});
            return be;
        }

        /*public EntryElement FromDouble(string caption, 
                                       string units, 
                                       Func<double> getter, 
                                       Action<double> setter,
                                       double min = Double.MinValue,
                                       double max = Double.MaxValue)
        {
            var ee = new EntryElement(caption,units,getter().ToString());           
            ee.KeyboardType = UIKeyboardType.NumbersAndPunctuation;           
            ee.TextAlignment = UITextAlignment.Right;
            ee.ReturnKeyType = UIReturnKeyType.Done;           
            ee.EntryEnded += (s,e) => { 
                double result = getter();  
                double parsed;
                bool success = false;
                if(double.TryParse(ee.Value,out parsed)) {
                    if(parsed >= min && parsed < max) {                      
                        setter(parsed); 
                        _notifyOnSet();
                        result = parsed;
                        success = true;
                    }
                }

                if(!success) {
                    var alert = new UIAlertView(caption,
                                            String.Format("Please enter a value between {0} and {1} {2}",min,max,units)
                                            ,null,"Ok",null);
                    alert.Show();
                }

                ee.Value = result.ToString();
                //return success;
            };
            _getters.Add(()=>{ee.Value=getter().ToString();});
            return ee;
        }*/

        public RootElement FromList<T,IdType> (string caption, string groupKey, T[] items, Func<T,IdType> itemToId, Func<T,string> itemToLabel, Func<T> getter, Action<T> setter)
        {                    
            var list = items.ToList();
            var re = new RootElement(caption, new RadioGroup(groupKey,list.FindIndex(i=>itemToId(i).Equals(itemToId((getter()))))));
            re.Add(listToSection<T>(list,itemToLabel,groupKey,()=>{setter(list[re.RadioSelected]);_notifyOnSet();})); 
            _getters.Add(()=>{re.RadioSelected=list.FindIndex(i=>itemToId(i).Equals(itemToId((getter()))));});
            return re;
        }
               
        private Section listToSection<T> (List<T> items,  Func<T,string> itemToLabel, string groupKey, Action tapped)
        {
            var section = new Section();
            foreach (var item in items) {
                var elem = new RadioElement(itemToLabel(item),groupKey);
                elem.Tapped += () => { if (tapped!=null) tapped(); };
                section.Add(elem);
            }
            return section;
        }    
    }
}

