
using System;
using System.Drawing;
using System.Web;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Hello_MultiScreen_iPhone
{
	public partial class OpenUrlScreen : UIViewController
	{
		public OpenUrlScreen () : base ("OpenUrlScreen", null)
		{
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

		partial void btnBrowserUrl (NSObject sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl("http://www.bayliss-it.com.au"));
		}

		partial void btnEmailUrl (NSObject sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl("mailto://chris@bayliss-it.com.au"));
		}

		partial void btnMapUrl (NSObject sender)
		{
			var url = new NSUrl(string.Format(@"http://maps.google.com/maps?q={0}", HttpUtility.UrlEncode("9 Temple Mews, Iluka, WA")));
			UIApplication.SharedApplication.OpenUrl(url);					
		}

		partial void btnPhoneUrl (NSObject sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl("tel://0411767363"));
		}

		partial void btnSMSUrl (NSObject sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl("sms://0411767363"));
		}
	}
}

