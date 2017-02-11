// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Hello_MultiScreen_iPhone
{
	[Register ("TunerScreen")]
	partial class TunerScreen
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblValue1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView vwReadout { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblValue1 != null) {
				lblValue1.Dispose ();
				lblValue1 = null;
			}

			if (vwReadout != null) {
				vwReadout.Dispose ();
				vwReadout = null;
			}
		}
	}
}
