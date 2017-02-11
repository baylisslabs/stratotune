
using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Hello_MultiScreen_iPhone
{
	public class TableSource : UITableViewSource
	{
		private List<TableItem> _tableItems;
		private string _cellIdentifier = "TableCell";

		public TableSource (TableItem[] items)
		{
			_tableItems = new List<TableItem>(items);
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return _tableItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (_cellIdentifier);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, _cellIdentifier);
			}
			var item = _tableItems[indexPath.Row];
			cell.TextLabel.Text = item.Text;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.ImageView.Image = item.Image;
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var item = _tableItems [indexPath.Row];
			if (item.RowSelected != null) {
				item.RowSelected();
			}
			tableView.DeselectRow(indexPath, true);
		}
	}
}