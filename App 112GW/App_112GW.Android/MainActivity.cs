using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
[assembly: Application(Debuggable = false)]
namespace App_112GW.Droid
{
    [Activity (Label = "121GW", 
        Icon = "@mipmap/ic_launcher", 
        Theme="@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		void HandleAndroidException(object sender, RaiseThrowableEventArgs e)
		{
			e.Handled = true;
			Console.Write(e.Exception.ToString());
		}
		static void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine(e.ExceptionObject.ToString());
		}


		protected override void OnCreate (Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate (bundle);

			AppDomain.CurrentDomain.UnhandledException += HandleExceptions;
			AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;

			global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new App_112GW.App ());
		}
	}
}