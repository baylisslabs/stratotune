
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Hello_MultiScreen_iPhone
{
	public partial class HomeScreen : UIViewController
	{
		private LazyInit<HelloWorldScreen> _helloWorldScreen = new LazyInit<HelloWorldScreen>();
		private LazyInit<HelloUniverseScreen> _helloUniverseScreen = new LazyInit<HelloUniverseScreen>();
		private LazyInit<MinutesToMidnightScreen> _minutesToMidnightScreen = new LazyInit<MinutesToMidnightScreen>();
		private LazyInit<BonfireScreen> _bonfireScreen = new LazyInit<BonfireScreen>();
		private LazyInit<OpenUrlScreen> _openUrlScreen = new LazyInit<OpenUrlScreen>();
		private LazyInit<CountMeInScreen> _countMeInScreen = new LazyInit<CountMeInScreen>();
	

		public HomeScreen () : base ("HomeScreen", null)
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

			this.tblMenu.Source = new TableSource(new [] { 
				new TableItem { Text = "Hello, World", RowSelected = ()=>{
						this.NavigationController.PushViewController(_helloWorldScreen.Obj,true);
					}
				}
				,new TableItem { Text="What's my IP?", RowSelected = ()=>{
						this.NavigationController.PushViewController(_helloUniverseScreen.Obj,true);
					}
				}
				,new TableItem { Text="Minutes to Midnight", RowSelected = ()=>{
						this.NavigationController.PushViewController(_minutesToMidnightScreen.Obj,true);
					}
				}
				,new TableItem { Text="Bonfire", RowSelected = ()=>{
						this.NavigationController.PushViewController(_bonfireScreen.Obj,true);
					}, Image = UIImage.FromFile("Images/campfire.png")
				}
				,new TableItem { Text="Open URL", RowSelected = ()=>{
						this.NavigationController.PushViewController(_openUrlScreen.Obj,true);
					}
				}
				,new TableItem { Text="Count me in", RowSelected = ()=>{
						this.NavigationController.PushViewController(_countMeInScreen.Obj,true);
					}, Image = UIImage.FromFile("Images/icons/countMeInIcon.png")
				}			
			});
		}			

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear(animated);
			this.NavigationController.SetNavigationBarHidden(true,animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear(animated);
			this.NavigationController.SetNavigationBarHidden(false,animated);
		}

		public void OnTimer ()
		{
			_minutesToMidnightScreen.IfNotNull((screen)=>{screen.UpdateCountDown();});			
		}
	}
}

