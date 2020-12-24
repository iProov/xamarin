using System;

using UIKit;
using iProov.APIClient;
using iProov.iOS;

namespace WaterlooBank.iOS
{
    public partial class ViewController : UIViewController
    {

        APIClient apiClient = new APIClient(
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
                var token = await apiClient.GetToken(AssuranceType.GenuinePresence, ClaimType.Enrol, userId);

                IProov.LaunchWithStreamingURL("https://eu.rp.secure.iproov.me/", token, new Options(),
                    connecting: () =>
                    {
                        Console.WriteLine("Connecting...");
                    },
                    connected: () =>
                    {
                        Console.WriteLine("Connected");
                    },
                    processing: (progress, message) =>
                    {
                        Console.WriteLine(progress);
                    },
                    success: (result) =>
                    {
                        Console.WriteLine("Success");
                    },
                    cancelled: () =>
                    {
                        Console.WriteLine("Cancelled");
                    },
                    failure: (result) =>
                    {
                        Console.WriteLine(result.Reason);
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
