// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Hello_MultiScreen_iPhone
{
	[Register ("CountMeInScreen")]
	partial class CountMeInScreen
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblCounter { get; set; }

		[Action ("btnSubtract:")]
		partial void btnSubtract (MonoTouch.Foundation.NSObject sender);

		[Action ("btnAdd:")]
		partial void btnAdd (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblCounter != null) {
				lblCounter.Dispose ();
				lblCounter = null;
			}
		}
	}
}
