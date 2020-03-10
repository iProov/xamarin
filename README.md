# iProov Xamarin SDK (Preview)

## 🤳 Introduction

The iProov Xamarin SDK enables you to integrate iProov into your Xamarin.iOS or Xamarin.Android project. This SDK wraps iProov's existing native [iOS](https://github.com/iProov/ios) (Swift) and [Android](https://github.com/iProov/android) (Java) SDKs behind a .NET interface for use from within your Xamarin app.

We also provide a .NET API Client written in C# to call our [REST API v2](https://eu.rp.secure.iproov.me/docs.html) from a .NET Standard Library, which can be used from your Xamarin app to request tokens directly from the iProov API (note that this is not a secure way of getting tokens, and should only be used for demo/debugging purposes).

### Preview

The iProov Xamarin SDK is currently a customer preview, which means that there may be missing/broken functionality, and the API is still subject to change. Please contact [support@iproov.com](mailto:support@iproov.com) to provide your feedback regarding the iProov Xamarin SDK preview.

## 📖 Contents

The iProov Xamarin SDK is provided via this repository, which contains the following:

- **README.md** - This document
- **NuGet Packages** - Directory containing the NuGet packages for Xamarin.iOS & Xamarin.Android
- **APIClient** - C# project with the source code for the .NET API Client
- **WaterlooBank** - Sample code demonstrating use of the Xamarin.iOS & Xamarin.Android bindings together with the .NET API Client

## 🍏 iOS

1. Add the "NuGet Packages" directory to your Visual Studio Nuget package sources. For further information, [see here](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough?toc=%2Fnuget%2Ftoc.json&view=vsmac-2019#adding-package-sources).

2. Add the **iProov.iOS** NuGet package to your Xamarin project. For further information, [see here](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough?toc=%2Fnuget%2Ftoc.json&view=vsmac-2019#find-and-install-a-package).

3. Add a "Privacy - Camera Usage Description" entry to your Info.plist file with the reason why your app requires camera access (e.g. "To iProov you in order to verify your identity.")

4. Import the package into your project with `using iProov.iOS;`.

5. Once you have obtained a token (either via the .NET API Client or other means), you can launch the iProov iOS SDK as follows:

	```csharp
	IProov.LaunchWithStreamingURL("https://eu.rp.secure.iproov.me/", token, new Options(),
		    processing: (progress, message) =>
		    {
				// The SDK will update your app with the progress of streaming to the server and authenticating
				// the user. This will be called multiple time as the progress updates.
		    },
		    success: (theToken) =>
		    {
				// The user was successfully verified/enrolled and the token has been validated.
				// The token passed back will be the same as the one passed in to the original call.
		    },
		    cancelled: () =>
		    {
				// The user cancelled iProov, either by pressing the close button at the top right, or sending
				// the app to the background.
		    },
		    failure: (reason) =>
		    {
				// The user was not successfully verified/enrolled, as their identity could not be verified,
				// or there was another issue with their verification/enrollment. A reason (as a string)
				// is provided as to why the claim failed, along with a feedback code from the back-end.
		    },
		    error: (error) =>
		    {
				// The user was not successfully verified/enrolled due to an error (e.g. lost internet connection)
				// along with an `iProovError` with more information about the error (NSError in Objective-C).
				// It will be called once, or never.
		    }
	);
	```
	
	> If you wish to stream to a back-end other than our EU platform, you should pass the appropriate streaming URL as the first parameter.
	
👉 You should now familiarise yourself with the [iProov iOS SDK documentation](https://github.com/iProov/ios) which provides comprehensive details about the available customization options and other important details regarding the iOS SDK usage.

## 🤖 Android

1. Add the "NuGet Packages" directory to your Visual Studio Nuget package sources. For further information, [see here](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough?toc=%2Fnuget%2Ftoc.json&view=vsmac-2019#adding-package-sources).

2. Add the **iProov.Android** NuGet package to your Xamarin project. For further information, [see here](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough?toc=%2Fnuget%2Ftoc.json&view=vsmac-2019#find-and-install-a-package).

3. Open your project's .csproj file in a text editor and add the following inside each `<PropertyGroup>` block:

	```xml
	<AndroidDexTool>d8</AndroidDexTool>
	```
	
	This will enable the use of Java 8 language features which are required by iProov.Android.

4. Import the package into your project with `using iProov.Android;`.

5. Create a private class which implements `IProov.IListener` to handle the callbacks from the Android SDK:

	```csharp
	private IProovListener listener = new IProovListener();
	
	private class IProovListener : Java.Lang.Object, IProov.IListener
	{
	
		public void OnCancelled()
		{
	   		// The user cancelled iProov, either by pressing the close button at the top right, or pressing
	   		// the home button.
		}
		
		public void OnError(IProovException error)
		{
			// The user was not successfully verified/enrolled due to an error (e.g. lost internet connection)
			// along with an `iProovError` with more information about the error (NSError in Objective-C).
			// It will be called once, or never.
		}
		
		public void OnFailure(string reason, string feedbackCode)
		{
			// The user was not successfully verified/enrolled, as their identity could not be verified,
			// or there was another issue with their verification/enrollment. A reason (as a string)
			// is provided as to why the claim failed, along with a feedback code from the back-end.
		}
		
		public void OnProcessing(double progress, string message)
		{
			// The SDK will update your app with the progress of streaming to the server and authenticating
			// the user. This will be called multiple time as the progress updates.
		}
		
		public void OnSuccess(string token)
		{
			// The user was successfully verified/enrolled and the token has been validated.
			// The token passed back will be the same as the one passed in to the original call.
		}
	
	}
	
	```
	
	> Alternatively you could just implement `IProov.IListener` on your `Activity` class.
	
6. You can now launch iProov by calling:

	```csharp
	IProov.Launch(this, token, listener);
	```
	
	> This will by default use iProov's EU platform for streaming. You can stream to an alternative endpoint by passing a streaming URL as the second parameter.
	
👉 You should now familiarise yourself with the [iProov Android SDK documentation](https://github.com/iProov/android) which provides comprehensive details about the available customization options and other important details regarding the iOS SDK usage.

## 🌎 API Client

The .NET API client provides a convenient wrapper to call iProov's REST API v2 from a .NET Standard Library. It is a useful tool to assist with testing, debugging and demos, but should not be used in production apps.

⚠️ Use of the .NET API Client requires providing it with your API secret. **You should never embed your API secret within a production app.**

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

### Usage

We will now run through a couple of common use-cases with the API Client. All the API Client source code is provided, so you can understand how it works and adapt it accordingly.

#### Getting a token

The most basic thing you can do with the API Client is get a token to either enrol or verify a user. This is achieved as follows:

```csharp
var token = await apiClient.GetToken(ClaimType.enrol, "{{ user id }}");
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

## 🏦 Waterloo Bank sample code

For a simple iProov experience that is ready to run out-of-the-box, check out the [Waterloo Bank sample project](/iProov/xamarin/tree/master/WaterlooBank) for Xamarin.iOS and Xamarin.Android which also makes use of the .NET API Client.

### Usage

1. Open the Waterloo Bank solution in Visual Studio.
2. Right click the root project and "Restore NuGet Packages" to ensure all NuGet packages are ready for usage.
3. Add your API key & secret to ViewController.cs (for WaterlooBank.iOS) and MainActivity.cs (for WaterlooBank.Droid).
4. Run the WaterlooBank.iOS or WaterlooBank.Droid project on a supported iOS or Android device respectively.

> NOTE: iProov is not supported on the iOS or Android simulator, you must use a physical device in order to iProov.