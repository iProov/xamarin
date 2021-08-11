![iProov: Flexible authentication for identity assurance](images/banner.jpg)

# iProov Biometrics Xamarin SDK

## Table of contents

- [Introduction](#introduction)
- [Repository contents](#repository-contents)
- [Upgrading from earlier versions](#upgrading-from-earlier-versions)
- [Registration](#registration)
- [Xamarin.iOS](#xamarin--ios)
- [Xamarin.Android](#xamarin--android)
- [API Client](#api-client)
- [Sample code](#sample-code)

## Introduction

The iProov Xamarin SDK enables you to integrate iProov into your Xamarin.iOS or Xamarin.Android project. This SDK wraps iProov's existing native [iOS](https://github.com/iProov/ios) (Swift) and [Android](https://github.com/iProov/android) (Java) SDKs behind a .NET interface for use from within your Xamarin app.

We also provide a .NET API Client written in C# to call our [REST API v2](https://eu.rp.secure.iproov.me/docs.html) from a .NET Standard Library, which can be used from your Xamarin app to request tokens directly from the iProov API (note that this is not a secure way of getting tokens, and should only be used for demo/debugging purposes).

### Xamarin.Forms

We are examining the possibility of providing cross-platform support for Xamarin.Forms. For the time being, you are able to use the Xamarin.iOS and Xamarin.Android SDKs to produce cross-platform apps by writing the relevant platform-specific code.

If you are interested in support for Xamarin.Forms, please [contact us](mailto:support@iproov.com).

## Repository contents

The iProov Xamarin SDK is provided via this repository, which contains the following:

- **README.md** - This document
- **NuGet Packages** - Directory containing the NuGet packages for Xamarin.iOS & Xamarin.Android
- **APIClient** - C# project with the source code for the .NET API Client
- **Example** - Sample code demonstrating use of the Xamarin.iOS & Xamarin.Android bindings together with the .NET API Client

## Upgrading from earlier versions

If you're already using an older version of the Xamarin SDK, consult the [Upgrade Guide](https://github.com/iProov/xamarin/wiki/Upgrade-Guide) for detailed information about how to upgrade your app.

## Registration

You can obtain API credentials by registering on the [iProov Partner Portal](https://www.iproov.net).

## Xamarin.iOS

1. Using the NuGet Package Manager, add the [iProov.iOS](https://www.nuget.org/packages/iProov.iOS/) package to your Xamarin project. For further instructions on how to do this, [see here](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough?toc=%2Fnuget%2Ftoc.json&view=vsmac-2019#find-and-install-a-package).

2. Add a "Privacy - Camera Usage Description" entry to your Info.plist file with the reason why your app requires camera access (e.g. "To iProov you in order to verify your identity.")

3. Import the package into your project with `using iProov.iOS;`.

4. Once you have obtained a token (either via the .NET API Client or other means), you can launch the iProov iOS SDK as follows:

	```csharp
	IProov.LaunchWithStreamingURL("https://eu.rp.secure.iproov.me/", token, new IPOptions(), // Substitute streaming URL as appropriate
		    connecting: () =>
		    {
				// The SDK is connecting to the server. You should provide an indeterminate progress indicator
				// to let the user know that the connection is taking place.
		    },
		    connected: () =>
		    {
				// The SDK has connected, and the iProov user interface will now be displayed. You should hide
				// any progress indication at this point.
		    },
		    processing: (progress, message) =>
		    {
				// The SDK will update your app with the progress of streaming to the server and authenticating
				// the user. This will be called multiple time as the progress updates.
		    },
		    success: (result) =>
		    {
				// The user was successfully verified/enrolled and the token has been validated.
				// You can access the following properties:
				var token = result.Token; // The token passed back will be the same as the one passed in to the original call
				var frame = result.Frame; // An optional image containing a single frame of the user, if enabled for your service provider
		    },
		    cancelled: () =>
		    {
				// The user cancelled iProov, either by pressing the close button at the top right, or sending
				// the app to the background.
		    },
		    failure: (result) =>
		    {
				// The user was not successfully verified/enrolled, as their identity could not be verified,
				// or there was another issue with their verification/enrollment. A reason (as a string)
				// is provided as to why the claim failed, along with a feedback code from the back-end.
				var feedbackCode = result.FeedbackCode;
				var reason = result.Reason;
		    },
		    error: (error) =>
		    {
				// The user was not successfully verified/enrolled due to an error (e.g. lost internet connection).
				// You will be provided with an NSError. You can check the error code against the IPErrorCode constants
				// to determine the type of error.
				// It will be called once, or never.
		    }
	);
	```
	
👉 You should now familiarise yourself with the [iProov iOS SDK documentation](https://github.com/iProov/ios) which provides comprehensive details about the available customization options and other important details regarding the iOS SDK usage.

## Xamarin.Android

1. Using the NuGet Package Manager, add the [iProov.Android](https://www.nuget.org/packages/iProov.Android/) package to your Xamarin project. For further instructions on how to do this, [see here](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough?toc=%2Fnuget%2Ftoc.json&view=vsmac-2019#find-and-install-a-package).

2. Import the package into your project with `using iProov.Android;`.

3. Create a private class which implements `IProov.IListener` to handle the callbacks from the Android SDK:

	```csharp
	private IProovListener listener = new IProovListener();
	
	private class IProovListener : Java.Lang.Object, IProov.IListener
	{
	
		public void OnConnected()
		{
	   		// Called when the SDK is connecting to the server. You should provide an indeterminate
	   		// progress indication to let the user know that the connection is being established.
		}

		public void OnConnecting()
		{
	   		// The SDK has connected, and the iProov user interface will now be displayed. You
	   		// should hide any progress indication at this point.
		}
            
		public void OnCancelled()
		{
	   		// The user cancelled iProov, either by pressing the close button at the top right, or pressing
	   		// the home button.
		}
		
		public void OnError(IProovException error)
		{
			// The user was not successfully verified/enrolled due to an error (e.g. lost internet connection)
			// along with an IProovException.
			// It will be called once, or never.
		}
		
		public void OnFailure(IProov.FailureResult result)
		{
			// The user was not successfully verified/enrolled, as their identity could not be verified,
			// or there was another issue with their verification/enrollment. A reason (as a string)
			// is provided as to why the claim failed, along with a feedback code from the back-end.
			
			var feedbackCode = result.FeedbackCode;
			var reason = result.Reason;
		}
		
		public void OnProcessing(double progress, string message)
		{
			// The SDK will update your app with the progress of streaming to the server and authenticating
			// the user. This will be called multiple time as the progress updates.
		}
		
		public void OnSuccess(IProov.SuccessResult result)
		{
			// The user was successfully verified/enrolled and the token has been validated.
			// The token passed back will be the same as the one passed in to the original call.
			
			var token = result.Token;
		}
	
	}
	
	```
	
	> Alternatively you could just implement `IProov.IListener` on your `Activity` class.
	
4. You must register the iProov listener when your Activity is created:

	```csharp
	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		IProov.RegisterListener(listener);
		
		// ...continue your activity setup ...
	}
	```
	
	...and unregister it when destroyed:
	
	```csharp
	protected override void OnDestroy()
	{
		IProov.UnregisterListener(listener);
		base.OnDestroy();
	}
	```

5. You can now launch iProov by calling:

	```csharp
	IProov.Launch(this, "https://eu.rp.secure.iproov.me/", token, new IProov.Options()); // Substitute the streaming URL as appropriate
	```
	
👉 You should now familiarise yourself with the [iProov Android SDK documentation](https://github.com/iProov/android) which provides comprehensive details about the available customization options and other important details regarding the iOS SDK usage.

## API Client

The .NET API client provides a convenient wrapper to call iProov's REST API v2 from a .NET Standard Library. It is a useful tool to assist with testing, debugging and demos, but should not be used in production mobile apps. You could also adapt this code to run on your back-end to perform server-to-server calls.

> ⚠️ **SECURITY NOTICE:** Use of the .NET API Client requires providing it with your API secret. **You should never embed your API secret within a production app.**

### Functionality

The .NET API Client supports the following functionality:

- `GetToken()` - Get an enrol/verify token
- `EnrolPhoto()` - Perform a photo enrolment (either from an electronic or optical image). The image must be provided in JPEG format.
- `Validate()` - Validate an existing token against the provided User ID.
- `EnrolPhotoAndGetVerifyToken()` - A convenience method which first gets an enrolment token, then enrols the photo against that token, and then gets a verify token for the user to iProov against.

### Installation

To add the .NET API Client to your project, add it as a sub-project to your solution, and then [add a reference](https://docs.microsoft.com/en-us/visualstudio/mac/managing-references-in-a-project?view=vsmac-2019) to the **APIClient** project from your app project.

You will also need to [add](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough?view=vsmac-2019) the **Newtonsoft.Json** NuGet package to your project as well.

You can now import the API Client with `using iProov.APIClient;`.

### Usage examples

We will now run through a couple of common use-cases with the API Client. All the API Client source code is provided, so you can understand how it works and adapt it accordingly.

#### Getting a token

The most basic thing you can do with the API Client is get a token to either enrol or verify a user, using either iProov's Genuine Presence Assurance or Liveness Assurance.

This is achieved as follows:

```csharp
var token = await apiClient.GetToken(AssuranceType.GenuinePresence, ClaimType.Enrol, "{{ user id }}");
```

You can then launch the iProov SDK with this token.

#### Performing a photo enrol (on iOS)

To photo enrol a user, you would first generate an enrolment token, then enrol the photo against the user, then generate a verification token.

Fortunately the .NET API Client provides a helper method which wraps all three calls into one convenience method.

The first thing you will need to do is convert your iOS native `UIImage` into a .NET `byte[]` which can be handled by the cross-platform API Client:

```csharp
var uiImage = UIImage.FromBundle("image.png");  // (For example)
var jpegData = uiImage.AsJPEG();
byte[] jpegBytes = new byte[jpegData.Length];
Marshal.Copy(jpegData.Bytes, jpegBytes, 0, Convert.ToInt32(jpegData.Length));
```

You can now pass the `jpegBytes` to the `EnrolPhotoAndGetVerifyToken()` method:

```csharp
string token = await apiClient.EnrolPhotoAndGetVerifyToken(guid, jpegBytes, PhotoSource.oid);
```

You can now launch the iProov SDK with this token to complete the photo enrolment.

## Sample code

For a simple iProov experience that is ready to run out-of-the-box, check out the [Example  project](/iProov/xamarin/tree/master/Example) for Xamarin.iOS and Xamarin.Android which also makes use of the .NET API Client.

### Usage

1. Open the Example solution in Visual Studio.
2. Right click the root project and "Restore NuGet Packages" to ensure all NuGet packages are ready for usage.
3. Add your API key & secret to Credentials.cs in the Shared project.
4. Run the iOSExample or AndroidExample project on a supported iOS or Android device respectively.

> NOTE: iProov is not supported on the iOS or Android simulator, you must use a physical device in order to iProov.