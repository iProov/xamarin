using Android.App;
using Android.Widget;
using Android.OS;

using iProov.APIClient;
using iProov.Android;
using System;

namespace WaterlooBank.Droid
{

    [Activity(Label = "Waterloo Bank", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {

        APIClient apiClient = new APIClient(
            "https://eu.rp.secure.iproov.me/api/v2",
            "<Your API Key>",   // TODO: Add your API key here
            "<Your Secret>",    // TODO: Add your Secret here
            "com.iproov.xamarin");

        private IProovListener listener = new IProovListener();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += async delegate {
                var userId = Guid.NewGuid().ToString(); // Generate a random User ID
                var token = await apiClient.GetToken(ClaimType.enrol, userId);
                IProov.Launch(this, token, listener);
            };
        }

        private class IProovListener : Java.Lang.Object, IProov.IListener
        {

            public void OnCancelled()
            {
                Console.WriteLine("Cancelled");
            }

            public void OnError(IProovException error)
            {
                Console.WriteLine("Error: " + error.Message);
            }

            public void OnFailure(string reason, string feedbackCode)
            {
                Console.WriteLine("Failure: " + reason);
            }

            public void OnProcessing(double progress, string message)
            {
                Console.WriteLine(message);
            }

            public void OnSuccess(string token)
            {
                Console.WriteLine("Success");
            }

        }

    }

 
}

