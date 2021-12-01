using Foundation;
using System;
using UIKit;

using BigTed;

using Shared;
using iProov.APIClient;
using iProov.iOS;

namespace iOSExample
{
    public partial class ViewController : UIViewController
    {

        APIClient apiClient = new APIClient(
            Credentials.BASE_URL,
            Credentials.API_KEY,
            Credentials.SECRET,
            "com.iproov.xamarin");

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            launchButton.TouchUpInside += async delegate
            {
                var userId = Guid.NewGuid().ToString(); // Generate a random User ID
                var token = await apiClient.GetToken(
                    AssuranceType.GenuinePresence, // Choose between GenuinePresence or Liveness
                    ClaimType.Enrol, // Choose between Enrol or Verify
                    userId); // Pass the User ID

                IProov.LaunchWithStreamingURL(Credentials.BASE_URL, token, new IPOptions(),
                    connecting: () =>
                    {
                        BTProgressHUD.Show("Connecting...", maskType: MaskType.Black);
                    },
                    connected: () =>
                    {
                        BTProgressHUD.Dismiss();
                    },
                    processing: (progress, message) =>
                    {
                        BTProgressHUD.Show(message, (float) progress, maskType: MaskType.Black);
                    },
                    success: (result) =>
                    {
                        BTProgressHUD.ShowSuccessWithStatus("Success!", maskType: MaskType.Black);
                    },
                    cancelled: () =>
                    {
                        BTProgressHUD.Dismiss();
                    },
                    failure: (result) =>
                    {
                        BTProgressHUD.ShowErrorWithStatus(result.Reason, maskType: MaskType.Black);
                    },
                    error: (error) =>
                    {
                        BTProgressHUD.ShowErrorWithStatus(error.LocalizedDescription, maskType: MaskType.Black);
                    });
            };
        }
    }
}
