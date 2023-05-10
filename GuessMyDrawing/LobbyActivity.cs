using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireSharp.Interfaces;
using FireSharp.Response;
using FireSharp.Config;
using Newtonsoft.Json;
using FireSharp.EventStreaming;
using Firebase.Database;
using Firebase;
using Android.Content.PM;

namespace OfekVentura_Project
{
    [Activity(Label = "LobbyActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class LobbyActivity : Activity
    {
        IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "Gq3wvgf3S61XTy92Dhg5fDdh5qpDdmGW0ERpnDn4",
            BasePath = "https://ofekventuraproject-69677-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        private TextView tvCode,tvPlayers;
        private string genCode;
        Handler handler;
        private ProgressDialog _progressDialog;
        private Button btnStart,btnBack;
        private string spDrawValue, spRoundsValue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.lobby_layout);
            // Create your application here
            handler = new Handler();
            handler.PostDelayed(MyThread, 1000);
            tvCode = FindViewById<TextView>(Resource.Id.lobby_tvCode);
            tvPlayers = FindViewById<TextView>(Resource.Id.lobby_tvPlayers);
            btnStart = FindViewById<Button>(Resource.Id.lobby_btnStart);
            btnBack = FindViewById<Button>(Resource.Id.lobby_btnBack);
            btnBack.Click += BtnBack_Click;
            //drawing spinner
            var spDraw = FindViewById<Spinner>(Resource.Id.spDraw);
            spDraw.ItemSelected += new System.EventHandler<AdapterView.ItemSelectedEventArgs>(SpDraw_ItemSelected);
            var drawAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.draw_time_second_array, Android.Resource.Layout.SimpleSpinnerItem);
            drawAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spDraw.Adapter=drawAdapter;
            //rounds spinner
            var spRounds = FindViewById<Spinner>(Resource.Id.spRounds);
            spRounds.ItemSelected += new System.EventHandler<AdapterView.ItemSelectedEventArgs>(SpRounds_ItemSelected);
            var roundsAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.rounds_array, Android.Resource.Layout.SimpleSpinnerItem);
            roundsAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spRounds.Adapter = roundsAdapter;
            if (Intent.GetBooleanExtra("isHost",false)==false)
            {
                spRounds.Enabled = false;
                spDraw.Enabled = false;
                btnStart.Enabled= false;

            }
            else
            {
                btnStart.Click += BtnStart_Click;
            }
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {

                Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
            }

            tvCode.Text = "Your code is: " + Intent.GetStringExtra("genCode");
            genCode= Intent.GetStringExtra("genCode");
        
        }
        private bool isStopped = false;
        // This method retrieves the list of players and the game status from Firebase Realtime Database using FireSharp. If the game has started, it stops the thread and starts the GameActivity.
        private void MyThread()
        {
            if (isStopped)
            {
                return;
            }
            RunOnUiThread(async () =>
            {
                FirebaseResponse response = await client.GetAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/players");
                Dictionary<string, playersOnline> data = JsonConvert.DeserializeObject<Dictionary<string, playersOnline>>(response.Body.ToString());
                CheckCode(data);
                 FirebaseResponse res = await client.GetAsync("RoomCodes/"+MainActivity.user.OwnerUid+"/isPlay");
                bool isPlay = res.ResultAs<bool>();
                if (isPlay)
                {
                    isStopped = true;
                    handler.RemoveCallbacks(MyThread);
                    Intent intent = new Intent(this, typeof(GameActivity));
                    intent.PutExtra("code", Intent.GetStringExtra("genCode"));
                    StartActivity(intent);

                }
            });

            // Reschedule the task to run again after the delay
            handler.PostDelayed(MyThread, 1000);
        }
        public override void OnBackPressed()
        {
        }
        // This method updates the players text to display the list of players in the game.
        private void CheckCode(Dictionary<string, playersOnline> data)
        {
            tvPlayers.Text = "Players List:\n";
            bool first = true;
            foreach (var item in data)
            {
                if (first)
                {
                    tvPlayers.Text += " " + item.Value.name;
                    first = false;
                }
                else
                {
                    tvPlayers.Text += ", " + item.Value.name;
                }
            }
        }

        private void SpRounds_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = sender as Spinner;
            spRoundsValue = (string)spinner.GetItemAtPosition(e.Position);

        }

        private void SpDraw_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = sender as Spinner;
           spDrawValue=(string)spinner.GetItemAtPosition(e.Position);
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MenuActivity));
            StartActivity(intent);
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            // Show the loading dialog
            _progressDialog = new ProgressDialog(this);
            _progressDialog.SetMessage("Loading...");
            _progressDialog.Show();
            PrivateCode privateCode = new PrivateCode
            {
                myCode = genCode,
                rounds = int.Parse(spRoundsValue),
                drawTimeSeconds=int.Parse(spDrawValue),
                word=""
               
             };
            try
            {
                await client.UpdateAsync("RoomCodes/" + MainActivity.user.OwnerUid, privateCode);

            }
            catch
            { }
            await client.SetAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/isPlay", true);
            // Dismiss the loading dialog
            _progressDialog.Dismiss();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

     
    }

 
}