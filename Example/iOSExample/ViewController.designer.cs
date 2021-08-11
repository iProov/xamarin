// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace iOSExample
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton launchButton { get; set; }

		[Outlet]
		UIKit.UILabel progressLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (launchButton != null) {
				launchButton.Dispose ();
				launchButton = null;
			}

			if (progressLabel != null) {
				progressLabel.Dispose ();
				progressLabel = null;
			}
		}
	}
}
