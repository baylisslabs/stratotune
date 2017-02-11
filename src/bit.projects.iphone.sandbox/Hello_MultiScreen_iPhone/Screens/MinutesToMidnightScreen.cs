
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Hello_MultiScreen_iPhone
{
	public partial class MinutesToMidnightScreen : UIViewController
	{
		public MinutesToMidnightScreen () : base ("MinutesToMidnightScreen", null)
		{
			this.Title = "Minutes to Midnight";
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			this.lblCountDown.Font = UIFont.FromName("DBLCDTempBlack",60);
			setCountDownText();
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		public void UpdateCountDown()
		{
			setCountDownText();
		}

		private void setCountDownText ()
		{
			if (this.lblCountDown != null) {
				var timeToGo = (DateTime.Today - DateTime.Now) + TimeSpan.FromDays (1);
				this.lblCountDown.Text = String.Format ("{0:00}:{1:00}:{2:00}", timeToGo.Hours, timeToGo.Minutes, timeToGo.Seconds);
			}
		}
	}
}

