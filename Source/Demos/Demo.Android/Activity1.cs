using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Platformer2D
{
    [Activity(
        Label = "Demo.Android",
        MainLauncher = true, 
        Icon = "@drawable/icon", 
        Theme = "@style/Theme.Splash", 
        AlwaysRetainTaskState = true, 
        LaunchMode = LaunchMode.SingleInstance, 
        ScreenOrientation = ScreenOrientation.SensorLandscape, 
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var g = new GameMain();
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }
    }
}

