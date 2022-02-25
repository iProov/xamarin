using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Content;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;

using AndroidHUD;

using Shared;
using iProov.APIClient;
using iProov.Android;
using Android.Content.PM;

namespace AndroidExample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {

        APIClient apiClient = new APIClient(
            Credentials.BASE_URL,
            Credentials.API_KEY,
            Credentials.SECRET,
            "com.iproov.xamarin");

        private IProovListener listener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            listener = new IProovListener(this);
            IProov.RegisterListener(listener);

            AppCompatButton launchButton = FindViewById<AppCompatButton>(Resource.Id.launchButton);
            launchButton.Click += async delegate {
                var userId = Guid.NewGuid().ToString(); // Generate a random User ID
                var token = await apiClient.GetToken(
                    AssuranceType.GenuinePresence, // Choose between GenuinePresence or Liveness
                    ClaimType.Enrol, // Choose between Enrol or Verify
                    userId); // Pass the User ID

                var options = new IProov.Options();
                options.Ui.FloatingPromptEnabled = true;

                IProov.Launch(this, Credentials.BASE_URL, token, options);
            };
        }

        protected override void OnDestroy()
        {
            IProov.UnregisterListener(listener);
            base.OnDestroy();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private class IProovListener : Java.Lang.Object, IProov.IListener
        {

            private Context context;

            public IProovListener(Context context)
            {
                this.context = context;
            }

            public void OnConnected()
            {
                AndHUD.Shared.Show(context, "Connecting...");
            }

            public void OnConnecting()
            {
                AndHUD.Shared.Dismiss();
            }

            public void OnCancelled()
            {
                AndHUD.Shared.Dismiss(context);
            }

            public void OnError(IProovException error)
            {
                AndHUD.Shared.ShowErrorWithStatus(context, error.Reason, timeout: TimeSpan.FromSeconds(1));
            }

            public void OnFailure(IProov.FailureResult result)
            {
                AndHUD.Shared.ShowErrorWithStatus(context, result.Reason, timeout: TimeSpan.FromSeconds(1));
            }

            public void OnProcessing(double progress, string message)
            {
                AndHUD.Shared.Show(context, message, (int) (progress * 100));
            }

            public void OnSuccess(IProov.SuccessResult result)
            {
                AndHUD.Shared.ShowSuccess(context, "Success!", timeout: TimeSpan.FromSeconds(1));
            }
        }
    }
}
