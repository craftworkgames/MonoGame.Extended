using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Demo.Features;

namespace Demo.Android
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

            var game = new GameMain(new PlatformConfig { IsFullScreen = true });
            var view = (View)game.Services.GetService(typeof(View));

            HideSystemUi();

            SetContentView(view);
            game.Run();
        }

        private void HideSystemUi()
        {
            // https://developer.android.com/training/system-ui/immersive.html

            if (Build.VERSION.SdkInt >= (BuildVersionCodes)19)
            {
                var decorView = Window.DecorView;
                var uiVisibility = (int)decorView.SystemUiVisibility;
                var options = uiVisibility;

                options |= (int)SystemUiFlags.LowProfile;
                options |= (int)SystemUiFlags.Fullscreen;
                options |= (int)SystemUiFlags.HideNavigation;
                options |= (int)SystemUiFlags.ImmersiveSticky;

                decorView.SystemUiVisibility = (StatusBarVisibility)options;
            }
        }
    }
}

