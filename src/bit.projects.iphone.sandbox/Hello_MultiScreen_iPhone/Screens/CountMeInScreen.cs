
using System;
using System.Drawing;

using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Hello_MultiScreen_iPhone
{
	public partial class CountMeInScreen : UIViewController
	{
		private int _count = 0;
        private UISwitch _switch;
        private UISlider _slider;

		public CountMeInScreen () : base ("CountMeInScreen", null)
		{
            _switch = new UISwitch(new RectangleF(150,200,100,40));
            _slider = new UISlider(new RectangleF(20,250,300,40));

            var T = CGAffineTransform.MakeIdentity();
            //T.Scale(0.5f,0.75f);
            //T.Rotate((float)(Math.PI/2));
            _switch.Transform = T;
            _switch.OnImage = UIImage.FromFile("Images/Icons/54-lock.png");
            this.View.Add(_switch);
            this.View.Add(_slider);
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
				
        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
            this.lblCounter.Text = _count.ToString();
        }
					
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		partial void btnAdd (NSObject sender)
		{
			if(_count<999) {
				_count++;			
				this.lblCounter.Text = _count.ToString();
			}
		}

		partial void btnSubtract (NSObject sender)
		{
			if(_count>0) {
				_count--;						
				this.lblCounter.Text = _count.ToString();
			}
		}
	}
}

