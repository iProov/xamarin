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
using Xamarin.Essentials;

namespace AndroidExample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, IProovCallbackLauncher.IListener
    {

        APIClient apiClient = new APIClient(
            Credentials.API_CLIENT_URL,
            Credentials.API_KEY,
            Credentials.SECRET,
            "com.iproov.xamarin");

        private IProovCallbackLauncher iProovLauncher = new IProovCallbackLauncher();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            iProovLauncher.Listener = this;

            AppCompatButton launchButton = FindViewById<AppCompatButton>(Resource.Id.launchButton);
            launchButton.Click += launchIProov;
        }

        protected override void OnDestroy()
        {
            iProovLauncher.Listener = null;
            base.OnDestroy();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async void launchIProov(object sender, EventArgs eventArgs)
        {
            var guid = Guid.NewGuid().ToString();

            var token = await apiClient.GetToken(AssuranceType.GenuinePresence, ClaimType.Enrol, guid);

            var options = new IProov.Options();
            options.EnableScreenshots = true;

            iProovLauncher.Launch(this, Credentials.BASE_URL, token, options);
        }

        public void OnConnected()
        {
            AndHUD.Shared.Show(this, "Connecting...");
        }

        public void OnConnecting()
        {
            AndHUD.Shared.Dismiss();
        }

        public void OnCanceled(IProov.Canceler canceler)
        {
            var canceledBy = canceler.Name();
            AndHUD.Shared.Dismiss(this);
        }

        public void OnError(IProovException error)
        {
            AndHUD.Shared.ShowErrorWithStatus(this, error.Reason, timeout: TimeSpan.FromSeconds(1));
        }

        public void OnFailure(IProov.FailureResult result)
        {
            var feedbackCode = result.Reason.FeedbackCode;
            var reason = this.GetString(result.Reason.Description);
            AndHUD.Shared.ShowErrorWithStatus(this, reason, timeout: TimeSpan.FromSeconds(1));
        }

        public void OnProcessing(double progress, string message)
        {
            AndHUD.Shared.Show(this, message, (int) (progress * 100));
        }

        public void OnSuccess(IProov.SuccessResult result)
        {
            AndHUD.Shared.ShowSuccess(this, "Success!", timeout: TimeSpan.FromSeconds(1));
        }
       
    }
}
