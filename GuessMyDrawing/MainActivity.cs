using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Gms.Tasks;
using Android.Content;
using System;
using FbAuthentication;
using Android.Text.Method;
using Firebase.Auth;
using Android.Media;
using AlertDialog = Android.App.AlertDialog;
using Android.Views;
using Java.Interop;
using System.Net.Mail;
using Firebase;
using Microsoft;
using Android.Content.PM;
using Android.Preferences;

namespace OfekVentura_Project
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, IOnCompleteListener
    {
        private BroadcastBattery broadcastBattery;
        private EditText etEmail, etPass,etUsername;
        private Button btnLog, btnEye;
        private CheckBox cbPassword;
        private bool EyeClosed = false;
        private TextView tvRegister, tvGuest, tvReset, tvBattery;
        public static User user;
        private ISharedPreferences sp;
        FB_Data fbd;
        Task tskLogin, tskReset,tskGuest;
        public static float Volume = 0.5f; // default volume at 50%

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            sp = this.GetSharedPreferences("details", FileCreationMode.Private);
            Volume = sp.GetFloat("volume", Volume);
            Intent intent = new Intent(this, typeof(MusicService));
            LayoutInflater inflater = LayoutInflater.From(this);
            StartService(intent);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            //Linking Assets
            tvBattery = FindViewById<TextView>(Resource.Id.tvBatteryStat);
            btnLog = FindViewById<Button>(Resource.Id.btnLog);
            btnEye = FindViewById<Button>(Resource.Id.btnEye);
            etEmail = FindViewById<EditText>(Resource.Id.etEmail);
            etPass = FindViewById<EditText>(Resource.Id.etPass);
            etUsername = FindViewById<EditText>(Resource.Id.etUsername);
            tvRegister = FindViewById<TextView>(Resource.Id.tvRegister);
            tvGuest = FindViewById<TextView>(Resource.Id.tvGuest);
            tvReset = FindViewById<TextView>(Resource.Id.tvReset);
            cbPassword = FindViewById<CheckBox>(Resource.Id.cbPassword);
            //Checkers
            user = new User(this);
            fbd = new FB_Data();
            if (user.Exist)
                ShowUserData();
            //Clicks
            btnLog.Click += BtnLog_Click;
            btnEye.Click += BtnEye_Click;
            tvRegister.Click += TvRegister_Click;
            tvReset.Click += TvReset_Click;
            tvGuest.Click += TvGuest_Click;
            //Signout
             fbd.SignOut();
            broadcastBattery = new BroadcastBattery(tvBattery);
        }
        protected override void OnResume()
        {
            base.OnResume();
            RegisterReceiver(broadcastBattery, new IntentFilter(Intent.ActionBatteryChanged));
        }
        protected override void OnPause()
        {
            UnregisterReceiver(broadcastBattery);
            base.OnPause();
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // set the menu layout on Main Activity  
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_Logout)
            {
                Toast.MakeText(this, "You selected Logout!", ToastLength.Long).Show();
                fbd.SignOut();
                
            }
            if(item.ItemId==Resource.Id.action_musiccontroller)
            {
                ShowVolumeControlDialog();

            }
            if (item.ItemId==Resource.Id.action_insturcation)
            {
                Intent intent = new Intent(this, typeof(InstructionActivity));
                StartActivity(intent);

            }
            return false;
        }


        // This method displays a custom volume control dialog that allows the user to adjust the volume of the music. It retrieves the current volume from shared preferences and updates it when the seek bar is changed.

        private void ShowVolumeControlDialog()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            LayoutInflater inflater = LayoutInflater.From(this);
            View view = inflater.Inflate(Resource.Layout.custom_music_controller, null);
            builder.SetView(view);
            TextView volumeTextView = view.FindViewById<TextView>(Resource.Id.tvVol);
            Button btn = view.FindViewById<Button>(Resource.Id.btnCloseVol);
            SeekBar volumeSeekBar = view.FindViewById<SeekBar>(Resource.Id.skVol);
            int volumeProgress = sp.GetInt("volumeProgress", 50); // default volume at 50%
            volumeSeekBar.Progress = volumeProgress;
            volumeTextView.Text = "Volume: " + volumeProgress; // update text view with current volume

            volumeSeekBar.ProgressChanged += (sender, e) =>
            {
                int progress = volumeSeekBar.Progress;
                ISharedPreferencesEditor editor = sp.Edit();
                editor.PutInt("volumeProgress", progress);
                editor.Apply(); // save the volume change to shared preferences
                volumeTextView.Text = "Volume: " + progress;

                float volume = progress / 100f;
                MusicService.mediaPlayer.SetVolume(volume, volume);
            };

         

            AlertDialog dialog = builder.Create();
            dialog.Show();
            btn.Click += (sender, e) => {
                // Save the volume change to shared preferences when the dialog is closed
                ISharedPreferencesEditor editor = sp.Edit();
                editor.PutInt("volumeProgress", volumeSeekBar.Progress);
                editor.Apply();
                dialog.Dismiss();
            };
        }


        // This method displays a custom dialog that allows the user to login anonymously by entering a username. If the entered username is valid, it signs in the user anonymously using Firebase Authentication.
        private void TvGuest_Click(object sender, EventArgs e)
        {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            View view = LayoutInflater.Inflate(Resource.Layout.custom_dialog_layout, null);
            dialog.SetView(view);
            AlertDialog alert = dialog.Create();
            var userdata = view.FindViewById<EditText>(Resource.Id.CostumDia_etDia);
            alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
            alert.Window.GetJniTypeName();
            alert.SetTitle("Login Anonymously");
            alert.SetMessage("Please Mention Your Username");
            alert.SetButton("Yes!", (c, ev) =>
            {
                user.Name = userdata.Text;
                if (ValidName(user.Name))
                {
                    tskGuest = fbd.SignInAnonymously();
                    tskGuest.AddOnCompleteListener(this);
                }
                else
                {
                    Toast.MakeText(this, "Name Rules: words or numbers", ToastLength.Long).Show();
                }

            });
            alert.SetButton3("No!", (c, ev) => { });
            alert.Show();

        }

        private bool ValidName(string name)
        {
            if (name == "")
                return false;
            char[] valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890 ".ToCharArray();
            char[] chars = name.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                for (int j = 0; j < valid.Length; j++)
                    if (!(Array.IndexOf(valid, chars[i]) > -1))
                        return false;
            return true;
        }


        private void TvReset_Click(object sender, EventArgs e)
        {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            View view = LayoutInflater.Inflate(Resource.Layout.custom_dialog_layout, null);
            dialog.SetView(view);
            AlertDialog alert = dialog.Create();
            var userdata = view.FindViewById<EditText>(Resource.Id.CostumDia_etDia);
            alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
            alert.Window.GetJniTypeName();
            alert.SetTitle("Reset Password?");
            alert.SetMessage("Please Mention Your Email Address");
            alert.SetButton("Yes!", (c, ev) =>
            {
                if (IsValidMail(userdata.Text))
                {
                    tskReset = fbd.ResetPassword(userdata.Text);
                    tskReset.AddOnCompleteListener(this);
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Reset Successful");
                    alert.SetMessage("We Sent a Password Reset Link To: "+userdata.Text);
                    alert.SetButton("Ok", (c, ev) =>{});
                    alert.Show();
                }
                else
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Reset Failed");
                    alert.SetMessage("Please Mention A Valid Email Address");
                    alert.SetButton("Ok", (c, ev) =>{});
                    alert.Show();
                }
            });
            alert.Show();

        }

        private void TvRegister_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivityForResult(intent, General.REQUEST_REGISTER);
        }

        private void BtnEye_Click(object sender, EventArgs e)
        {
            if (EyeClosed)
            {
                btnEye.Text = "Show Password";
                etPass.TransformationMethod = PasswordTransformationMethod.Instance;
                EyeClosed = !EyeClosed;
            }
            else
            {
                btnEye.Text = "Hide Password";
                etPass.TransformationMethod = HideReturnsTransformationMethod.Instance;
                EyeClosed = !EyeClosed;
            }

        }

        private void ShowUserData()
        {
            etEmail.Text = user.Mail;
            etUsername.Text= user.Name;
            etPass.Text = user.Pwd;
            cbPassword.Checked = (user.Pwd != string.Empty);

        }
        public override void OnBackPressed()
        {
        }
        private void BtnLog_Click(object sender, System.EventArgs e)
        {
                if (etPass.Text != string.Empty && etEmail.Text != string.Empty && etUsername.Text != string.Empty)
                {
                    tskLogin = fbd.SignIn(etEmail.Text, etPass.Text);
                    tskLogin.AddOnCompleteListener(this);
                    user.Pwd = cbPassword.Checked ? etPass.Text : string.Empty;
                    if (!user.Save())
                        Toast.MakeText(this, "Error", ToastLength.Long).Show();
                }
                else
                    Toast.MakeText(this, "Enter All Values", ToastLength.Long).Show();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        public void OnComplete(Task task)
        {
            string msg = string.Empty;
            if (task.IsSuccessful)
            {
                if (task == tskLogin)
                {
                    msg = "Login successful";
                    Intent intent = new Intent(this, typeof(MenuActivity));
                    FirebaseAuth auth = FirebaseAuth.Instance;
                    user.Uid = auth.Uid;
                    StartActivity(intent);
                }
                else if (task == tskGuest)
                {
                    msg = "Login Anonymously successful";
                    Intent intent = new Intent(this, typeof(MenuActivity));
                    FirebaseAuth auth = FirebaseAuth.Instance;
                    user.Uid = auth.Uid;
                    user.IsGuest = true;
                    StartActivity(intent);
                    fbd.DelteUser();
                }
                else if (task == tskReset)
                    msg = "Reset successful";
            }
            else
                msg = task.Exception.Message;
            Toast.MakeText(this, msg, ToastLength.Short).Show();

        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == General.REQUEST_REGISTER)
                if (resultCode == Result.Ok)
                {
                    user.Name = data.GetStringExtra(General.KEY_NAME);
                    user.Mail = data.GetStringExtra(General.KEY_MAIL);
                    user.Pwd = data.GetStringExtra(General.KEY_PWD);
                    ShowUserData();
                    user.Pwd = string.Empty;
                    if (!user.Save())
                        Toast.MakeText(this, "Error", ToastLength.Long).Show();
                    cbPassword.Checked = false;
                }
        }
        private static bool IsValidMail(string email)
        {
            var valid = true;
            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }
            return valid;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}