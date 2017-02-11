
using System;
using System.Drawing;
using System.Net;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
//using MonoTouch.ObjCRuntime

namespace Hello_MultiScreen_iPhone
{
	public partial class HelloUniverseScreen : UIViewController
	{
		public HelloUniverseScreen () : base ("HelloUniverseScreen", null)
		{
			this.Title = "What's my IP?";
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
			string host = Dns.GetHostName ();
			//if (ObjCRuntime.Runtime.Arch == ARCH.Device) {
			//	host += "local";
			//}
			var addresses = Dns.GetHostAddresses (host);
			if (addresses.Length > 0) {
				lblMyIp.Text = host+" "+addresses [0].ToString();
			}

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
	}
}

