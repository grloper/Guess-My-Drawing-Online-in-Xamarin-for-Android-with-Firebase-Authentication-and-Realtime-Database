using Android.App;
using Android.OS;
using Android.Widget;
using Android.Text.Method;
using Android.Graphics;
using Android.Gms.Tasks;
using System.Text.RegularExpressions;
using Android.Content;
using FbAuthentication;
using System.Net.Mail;
using Android.Content.PM;

namespace OfekVentura_Project
{
    [Activity(Label = "RegisterActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class RegisterActivity : Activity, IOnCompleteListener
    {

        private User user;
        private EditText etPass1, etPass2, etUsername, etEmail;
        private Button btnRegister, btnEye;
        private TextView tvLogin;
        private bool EyeClosed = false;
        public static string name2;
        private FB_Data fbd;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.register_layout);
            etPass1 = FindViewById<EditText>(Resource.Id.etPassReg1);
            etPass2 = FindViewById<EditText>(Resource.Id.etPassReg2);
            etUsername = FindViewById<EditText>(Resource.Id.etUserReg);
            etEmail = FindViewById<EditText>(Resource.Id.etEmailReg);
            tvLogin = FindViewById<TextView>(Resource.Id.tvLogin);
            //if clicked on register
            btnRegister = FindViewById<Button>(Resource.Id.btnSubmitReg);
            btnRegister.Click += BtnRegister_Click;
            //if clicked on "show pass:
            btnEye = FindViewById<Button>(Resource.Id.btnEyeReg);
            btnEye.Click += BtnEye_Click;
            etPass1.TextChanged += EtPass1_TextChanged;
            //if clicked on login
            tvLogin.Click += TvLogin_Click;
            //return to Main
            fbd = new FB_Data();
        }

        private void TvLogin_Click(object sender, System.EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }
        public override void OnBackPressed()
        {
        }
        private void BtnEye_Click(object sender, System.EventArgs e)
        {
            if (EyeClosed)
            {
                btnEye.Text = "Show Password";
                etPass1.TransformationMethod = PasswordTransformationMethod.Instance;
                etPass2.TransformationMethod = PasswordTransformationMethod.Instance;
                EyeClosed = !EyeClosed;
            }
            else
            {
                btnEye.Text = "Hide Password";
                etPass1.TransformationMethod = HideReturnsTransformationMethod.Instance;
                etPass2.TransformationMethod = HideReturnsTransformationMethod.Instance;
                EyeClosed = !EyeClosed;
            }
        }
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                Intent i = new Intent();
                i.PutExtra(General.KEY_NAME, user.Name);
                i.PutExtra(General.KEY_MAIL, user.Mail);
                i.PutExtra(General.KEY_PWD, user.Pwd);
                SetResult(Result.Ok, i);
                Finish();
            }
            else
            {
                Toast.MakeText(this, task.Exception.Message, ToastLength.Short).Show(); // יקבל את הסיבה לכישלון שהגיעה מהפיירבייס
            }
        }
        private void BtnRegister_Click(object sender, System.EventArgs e)
        {
            user = new User(etUsername.Text, etEmail.Text, etPass1.Text, false);
            if (user.Name != string.Empty && user.Pwd != string.Empty && IsValidPass(etPass1.Text) && etPass1.Text.Equals(etPass2.Text)&&IsValidMail(etEmail.Text))
                fbd.CreateUser(user.Mail, user.Pwd).AddOnCompleteListener(this);
            else
                Toast.MakeText(this, "Enter all values", ToastLength.Short).Show();
        }

        private void EtPass1_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var tvHasNumber = FindViewById<TextView>(Resource.Id.anReg);
            var tvUpperChar = FindViewById<TextView>(Resource.Id.ucReg);
            var tvMin8 = FindViewById<TextView>(Resource.Id.min8Reg);
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniChars = new Regex(@".{8,}");
            if (!hasNumber.IsMatch(etPass1.Text))
                tvHasNumber.SetTextColor(Color.Red);
            else
                tvHasNumber.SetTextColor(Color.Green);
            if (!hasUpperChar.IsMatch(etPass1.Text))
                tvUpperChar.SetTextColor(Color.Red);
            else
                tvUpperChar.SetTextColor(Color.Green);
            if (!hasMiniChars.IsMatch(etPass1.Text))
                tvMin8.SetTextColor(Color.Red);
            else
                tvMin8.SetTextColor(Color.Green);
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
        private bool IsValidPass(string s)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniChars = new Regex(@".{8,}");
            return hasMiniChars.IsMatch(s) && hasUpperChar.IsMatch(s) && hasNumber.IsMatch(s);

        }

    }
}