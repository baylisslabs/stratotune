using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Hello_MultiScreen_iPhone
{
	public struct TableItem
	{
		public string Text;
		public Action RowSelected;
		public UIImage Image;
	}
}