using System;

using UIKit;
using APIClient;
using iProov.iOS;

namespace WaterlooBank.iOS
{
    public partial class ViewController : UIViewController
    {

        APIClient.APIClient apiClient = new APIClient.APIClient(
            "https://eu.rp.secure.iproov.me/api/v2",
            "<Your API Key>",  // TODO: Add your API key here
            "<Your Secret>",   // TODO: Add your Secret here
            "com.iproov.xamarin");

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            Button.AccessibilityIdentifier = "myButton";
            Button.TouchUpInside += async delegate
            {
                var userId = Guid.NewGuid().ToString(); // Generate a random User ID
                var token = await apiClient.GetToken(ClaimType.enrol, userId);

                IProov.LaunchWithStreamingURL("https://eu.rp.secure.iproov.me/", token, new Options(),
                processing: (progress, message) =>
                {
                    Console.WriteLine(progress);
                },
                success: (theToken) =>
                {
                    Console.WriteLine("Success");
                },
                cancelled: () =>
                {
                    Console.WriteLine("Cancelled");
                },
                failure: (reason) =>
                {
                    Console.WriteLine(reason);
                },
                error: (error) =>
                {
                    Console.WriteLine(error);
                });
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }
}
