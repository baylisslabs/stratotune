
using System;
using System.Drawing;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Hello_MultiScreen_iPhone
{
	public partial class BonfireScreen : UIViewController
	{
		public BonfireScreen () : base ("BonfireScreen", null)
		{
			this.Title = "Bonfire";
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
			var campFireView = new UIImageView(this.View.Frame);
			campFireView.AnimationImages = Enumerable.Range(1,17).Select((n)=>(UIImage.FromFile(string.Format("Images/campFire{0:00}.gif",n)))).ToArray();
			campFireView.AnimationDuration = 1.75;
			campFireView.AnimationRepeatCount = 0;
			campFireView.StartAnimating();
			this.Add(campFireView);
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

