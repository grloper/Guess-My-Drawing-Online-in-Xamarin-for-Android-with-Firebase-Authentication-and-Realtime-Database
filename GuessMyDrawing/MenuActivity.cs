using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Renderscripts;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using FbAuthentication;
using Firebase.Auth;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Org.W3c.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static Android.Content.ClipData;

namespace OfekVentura_Project
{
    [Activity(Label = "MenuActivity", ScreenOrientation = ScreenOrientation.Portrait)]

    public class MenuActivity : Activity
    {
        IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "Gq3wvgf3S61XTy92Dhg5fDdh5qpDdmGW0ERpnDn4",
            BasePath = "https://ofekventuraproject-69677-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        private Button btnCreate, btnJoin,btnLogout;
        private EditText etCode;
        private TextView tvName;
        private string genCode;
        private ProgressDialog _progressDialog;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.menu_layout);
            // Create your application here
            Toast.MakeText(this, "User id: " + MainActivity.user.Uid, ToastLength.Short).Show();
            tvName = FindViewById<TextView>(Resource.Id.menu_tvName);
            tvName.Text = "Name: " + MainActivity.user.Name;
            btnCreate = FindViewById<Button>(Resource.Id.menu_btnCreate);
            btnCreate.Click += BtnCreate_Click;
            btnJoin = FindViewById<Button>(Resource.Id.menu_btnJoin);
            btnJoin.Click += BtnJoin_Click;
            btnLogout = FindViewById<Button>(Resource.Id.menu_btnLogOut);
            btnLogout.Click += BtnLogout_Click;
            etCode = FindViewById<EditText>(Resource.Id.menu_etCode);
            etCode.InputType = InputTypes.TextFlagCapCharacters;
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {
                Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
            }
           

        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            FB_Data fB_ = new FB_Data();
            fB_.SignOut();
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        // This method is called when the create button is clicked. It generates a new room code and creates a new room in Firebase Realtime Database using FireSharp with the specified settings. It also adds the user as the host of the room and starts the LobbyActivity.
        private async void BtnCreate_Click(object sender, EventArgs e)
        {
            _progressDialog = new ProgressDialog(this);
            _progressDialog.SetMessage("Loading...");
            _progressDialog.Show();
            FirebaseResponse response = client.Get("RoomCodes/");
            Dictionary<string, PrivateCode> data = JsonConvert.DeserializeObject<Dictionary<string, PrivateCode>>(response.Body.ToString());
            UploadCode(data);
            PrivateCode privateCode = new PrivateCode
            {
                myCode = genCode,
                rounds = 1,
                drawTimeSeconds = 30,
                drawCount = 1,
                isPlay = false,
                word = "",
                nextRound = false
            };
            var code = "";
            MainActivity.user.OwnerUid = MainActivity.user.Uid;
            try
            {
                await client.SetAsync("RoomCodes/" + MainActivity.user.OwnerUid, privateCode);
                playersOnline po = new playersOnline { name = MainActivity.user.Name, uid = MainActivity.user.Uid, role = "host", status = true };
                PushResponse pushResponse = await client.PushAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/players", po);
                po = new playersOnline { name = MainActivity.user.Name, uid = MainActivity.user.Uid, role = "host",pushKey= pushResponse.Result.Name, status = true }; 
                await client.UpdateAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/players/" + po.pushKey+"/", po);
                MainActivity.user.Role = "host";
                MainActivity.user.PushCode = pushResponse.Result.Name;
                response = client.Get("RoomCodes/" + MainActivity.user.OwnerUid + "/players/");
                code = response.ResultAs<string>();
            }
            catch
            { }
            Intent intent = new Intent(this, typeof(LobbyActivity));
            intent.PutExtra("genCode", genCode);
            intent.PutExtra("pushCode", code);
            intent.PutExtra("isHost", true);
            await client.SetAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/image/image", "");
            StartActivity(intent);
            // Dismiss the loading dialog
            _progressDialog.Dismiss();
        }


        // This method is called when the join button is clicked. It retrieves the list of room codes from Firebase Realtime Database using FireSharp and checks if the entered code matches any existing room codes.
        private void BtnJoin_Click(object sender, EventArgs e)
        {
            _progressDialog = new ProgressDialog(this);
            _progressDialog.SetMessage("Loading...");
            _progressDialog.Show();
            FirebaseResponse response = client.Get(@"RoomCodes");
            Dictionary<string, PrivateCode> data = JsonConvert.DeserializeObject<Dictionary<string, PrivateCode>>(response.Body.ToString());
            CheckCode(data);

        }
        public override void OnBackPressed()
        {
        }
        // This method checks if the entered code matches any existing room codes. If a match is found, it starts the LobbyActivity and updates the user's information in Firebase Realtime Database using FireSharp. If no match is found, it displays a toast message.
        private async void CheckCode(Dictionary<string, PrivateCode> data)
        {
            string uid = "";
            bool found = false;
            foreach (var item in data)
            {
                if (etCode.Text==item.Value.myCode)
                {
                    uid = item.Key;
                    Intent intent = new Intent(this, typeof(LobbyActivity));
                    intent.PutExtra("genCode", etCode.Text);
                    intent.PutExtra("roomUid", item.Key);
                    StartActivity(intent);
                    MainActivity.user.OwnerUid = uid;
                    try
                    {
                        playersOnline po = new playersOnline { name = MainActivity.user.Name, uid = MainActivity.user.Uid, role = "guesser",status=false };
                        PushResponse pushResponse = await client.PushAsync("RoomCodes/" + uid + "/players", po);
                        MainActivity.user.PushCode = pushResponse.Result.Name;
                        po.pushKey = pushResponse.Result.Name;
                        await client.UpdateAsync("RoomCodes/" + uid + "/players/"+po.pushKey, po);
                        MainActivity.user.Role = "guesser";

                    }
                    catch
                    { }
                    found = true;
                }
            }
            if (!found)
            Toast.MakeText(this, "Code Not Found!", ToastLength.Short).Show();
            // Dismiss the loading dialog
            _progressDialog.Dismiss();
        }


 

        private void UploadCode(Dictionary<string, PrivateCode> data)
        {
            genCode=GenCode();
            foreach (var item in data)
                if (genCode == item.Value.myCode)
                    UploadCode(data);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }
        // This method generates a random 6-character string consisting of uppercase letters and numbers.
        private string GenCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
    }
   
}