// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Hello_MultiScreen_iPhone
{
	[Register ("HomeScreen")]
	partial class HomeScreen
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tblMenu { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblMenu != null) {
				tblMenu.Dispose ();
				tblMenu = null;
			}
		}
	}
}
