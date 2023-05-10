using Android.App;
using Android.OS;
using Android.Widget;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using Android.Graphics;
using System.Text;
using Android.Views;
using System.Text.RegularExpressions;
using Firebase.Database;
using Firebase;
using Android.Media;
using System.Threading.Tasks;
using System.Threading;
using Android.Animation;
using Android.Content;
using System.Collections.Generic;
using Java.Lang;
using System.Linq;
using System.IO;
using Google.Android.Material.Canvas;
using Newtonsoft.Json;
using Android.Content.PM;
using OfekVentura_Project.Classes;

namespace OfekVentura_Project
{
    [Activity(Label = "GameActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameActivity : Activity
    {
        FingerPaintCanvasView fingerPaintCanvasView;
        private Spinner colorSpinner;
        IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "Gq3wvgf3S61XTy92Dhg5fDdh5qpDdmGW0ERpnDn4",
            BasePath = "https://ofekventuraproject-69677-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        private Button btnBack, btnGuess;
        private ImageButton btnClear;
        private TextView tvGuess, tvHint;
        private EditText etGuess;
        private SeekBar _seekBar;
        private int progress = 0, time = 100, timeTaken;
        int score = MainActivity.user.Score;
        Handler handler;
        Runnable runnable;

        private string word;
        private bool alertDialogDisplayed, isHost;
        [Obsolete]
        private ProgressDialog pd;
        private FirebaseApp app;
        private List<string> randomWords;
        private Spinner widthSpinner;

        public override void OnBackPressed()
        {
        }

        [Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        protected async override void OnCreate(Bundle savedInstanceState)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.game_layout);

            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {
                Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
            }
            app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseResponse res = await client.GetAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/drawTimeSeconds");
            time = res.ResultAs<int>();
            time *= 10;
            _seekBar = FindViewById<SeekBar>(Resource.Id.seekBar);
            _seekBar.Max = 100;
            _seekBar.Enabled = false;
            handler = new Handler();
            runnable = new Runnable(UpdateSeekBar);
            _seekBar.ProgressChanged += (s, e) =>
            {
                if (e.FromUser)
                {
                    progress = e.Progress;
                }
            };
            handler.PostDelayed(runnable, 1000);
            handler.PostDelayed(MyThread, 1000);
            handler.PostDelayed(MyThread2, 1000);
            isDisplayed = true;

            if (app is null)
            {
                Firebase.FirebaseOptions options = GetMyOptions();
                app = FirebaseApp.InitializeApp(Application.Context, options);
            }
            // Create your application here
            fingerPaintCanvasView = FindViewById<FingerPaintCanvasView>(Resource.Id.canvasView);

            // Set up the Spinner to select stroke color

            colorSpinner = FindViewById<Spinner>(Resource.Id.colorSpinner);
            colorSpinner.ItemSelected += ColorSpinner_ItemSelected; ;

            /*  var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.colors_array, Android.Resource.Layout.SimpleSpinnerItem);
              adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
              colorSpinner.Adapter = adapter;*/

            List<string> colors = new List<string> { "#000000", "#8B4513", "#FF0000", "#FFA500", "#FFFF00", "#008000", "#0000FF", "#EE82EE", "#ffffff", "#434744" };
            ColorSpinnerAdapter colorAdapter = new ColorSpinnerAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, colors);
            colorSpinner.Adapter = colorAdapter;
            // Set up the Spinner to select stroke width
            widthSpinner = FindViewById<Spinner>(Resource.Id.widthSpinner);
            widthSpinner.ItemSelected += WidthSpinner_ItemSelected; ;
            List<float> widths = new List<float> { 1, 3, 5, 8, 12 };
            WidthSpinnerAdapter widthAdapter = new WidthSpinnerAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, widths);
            widthSpinner.Adapter = widthAdapter;


            // Set up the Clear button
            btnClear = FindViewById<ImageButton>(Resource.Id.clearButton);
            btnClear.Click += BtnClear_Click;
            btnBack = FindViewById<Button>(Resource.Id.game_btnBack);
            btnGuess = FindViewById<Button>(Resource.Id.game_btnGuess);
            btnGuess.Click += BtnGuess_Click;
            tvGuess = FindViewById<TextView>(Resource.Id.game_tvGuess);
            tvHint = FindViewById<TextView>(Resource.Id.game_tvHint);
            etGuess = FindViewById<EditText>(Resource.Id.game_etGuess);
            btnBack.Click += BtnBack_Click;
            //checks if isHost
            if (MainActivity.user.Role == "host")
            {
                isHost = true;
            }
            if (isHost)
            {
                fingerPaintCanvasView.Enabled = true;
                //fullyhidding promots
                tvGuess.Visibility = ViewStates.Gone;
                tvGuess.Alpha = 0;

                btnGuess.Visibility = ViewStates.Gone;
                btnGuess.Alpha = 0;

                etGuess.Visibility = ViewStates.Gone;
                etGuess.Alpha = 0;
                if (!alertDialogDisplayed)
                {
                    // Generate 3 random words
                    randomWords = PictionaryWordGenerator.GetRandomWords();
                    // Create an alert dialog builder
                    var builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Select a word");
                    builder.SetCancelable(false);
                    builder.SetSingleChoiceItems(randomWords.ToArray(), -1, WordSelected);
                    var dialog = builder.Create();
                    dialog.Show();
                    alertDialogDisplayed = true;
                    _ = Task.Run(async () => await UploadAsync());
                }
            }
            else
            {
                fingerPaintCanvasView.Enabled = false;
                pd = new ProgressDialog(this);
                pd.SetMessage("Wait For Drawer to choose a word");
                pd.Indeterminate = true;
                pd.SetCancelable(false);
                pd.Show();


                widthSpinner.Visibility = ViewStates.Gone;
                widthSpinner.Alpha = 0;

                colorSpinner.Visibility = ViewStates.Gone;
                colorSpinner.Alpha = 0;

                btnClear.Visibility = ViewStates.Gone;
                btnClear.Alpha = 0;



            }
        }


        // This method checks if the user is not a host and retrieves the word from Firebase Realtime Database. If the word is not empty and different from the current word, it updates the UI and stops the thread.
        private bool isStopped = false;
        private bool flag = false;
        [Obsolete]
        private void MyThread2()
        {
            if (isStopped2)
            {
                return;
            }
            RunOnUiThread(async () =>
            {
                FirebaseResponse response = await client.GetAsync(@"RoomCodes/" + MainActivity.user.OwnerUid + "/players");
                Dictionary<string, playersOnline> data = JsonConvert.DeserializeObject<Dictionary<string, playersOnline>>(response.Body.ToString());
                CheckDone(data);
                if (flag)
                {
                    GameEnd();
                    isStopped2 = true;
                    handler.RemoveCallbacks(MyThread2);
                }
            });

            // Reschedule the task to run again after the delay
            handler.PostDelayed(MyThread2, 1000);
        }
        private bool isStopped2 = false;

        [Obsolete]
        //make sure displayed once
        private bool isDisplayed;
        private void MyThread()
        {
            if (isStopped)
            {
                return;
            }
            RunOnUiThread(async () =>
            {
                if (!isHost)
                {
                    FirebaseResponse res = await client.GetAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/word");
                    if (res.ResultAs<string>() != "" && res.ResultAs<string>() != word)
                    {
                        pd.Dismiss();
                        isStopped = true;
                        word = res.ResultAs<string>();
                        handler.RemoveCallbacks(MyThread);
                        SetLengthHint(res.ResultAs<string>());
                    }
                }
            });

            // Reschedule the task to run again after the delay
            handler.PostDelayed(MyThread, 1500);
        }
        [Obsolete]
        // The SetLengthHint method sets the hint text to display the number of letters in each word and underscores for each letter. The RevealLetters method reveals a specified number of random letters in the hint text.




        // This method updates the progress of the seek bar and checks if the game is over. If the game is over, it displays an alert dialog with the score and recreates the activity.
        private void UpdateSeekBar()
        {
            progress++;
            timeTaken--;
            if (isStopped2)
            {
                progress--;
            }
            if (!isHost)
            {
                RecieveAsync();
            }
            _seekBar.Progress = progress;

            if (progress >= 100)
            {
                GameEnd();
            }
            handler.PostDelayed(runnable, time);
        }
        private void CheckDone(Dictionary<string, playersOnline> data)
        {
            bool flag2 = true;

            foreach (var item in data)
            {
                if (item.Value.status == false)
                    flag2 = false;
            }
            if (flag2)
            {
                flag = true;
            }

        }

        private void GameEnd()
        {
            if (isHost)
            {
                score += (time / 10) + timeTaken;
            }
            if (isDisplayed)
            {
                isDisplayed = false;
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Game Ended!");
                alert.SetCancelable(false);
                alert.SetMessage("You Gained: " + score);
                alert.SetPositiveButton("Go Back To Lobby", (senderAlert, args) =>
                {
                    isStopped = true;
                    isStopped2 = true;
                    handler.RemoveCallbacks(runnable);
                    handler.Dispose();
                    runnable.Dispose();
                    Intent intent = new Intent(this, typeof(MenuActivity));
                    StartActivity(intent);
                });
                alert.Show();
            }
           
        }
        // This method uploads a base64 encoded image of the current canvas to Firebase Realtime Database using FireSharp.
        private async System.Threading.Tasks.Task UploadAsync()
        {
            // Get the width and height of the canvas
            int width = fingerPaintCanvasView.Width;
            int height = fingerPaintCanvasView.Height;
            // Create a new Bitmap object with the same width and height as the canvas
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            // Create a new Canvas object using the Bitmap
            Canvas canvas = new Canvas(bitmap);
            // Draw the contents of the fingerPaintCanvasView onto the canvas
            fingerPaintCanvasView.Draw(canvas);
            MemoryStream stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            byte[] bytes = stream.ToArray();
            string base64 = Convert.ToBase64String(bytes);
            // Save Base64 string to Firebase Realtime Database using FireSharp
            var data = new
            {
                image = base64
            };
            await client.SetAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/image", data);
        }
        // This method retrieves a base64 encoded image from Firebase Realtime Database using FireSharp, converts it to a Bitmap object and sets it in the FingerPaintCanvasView.
        private async void RecieveAsync()
        {
            // Retrieve Base64 string from Firebase Realtime Database using FireSharp
            var response = await client.GetAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/image/image");
            string base64 = response.ResultAs<string>();
            // Convert Base64 string to byte array and create a Bitmap object
            byte[] bytes = Convert.FromBase64String(base64);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
            // Set the Bitmap object in the FingerPaintCanvasView
            fingerPaintCanvasView.SetBitmap(bitmap);
        }

        [Obsolete]

        // This method is called when a word is selected from the dialog. It uploads the selected word and updates the hint text to display the selected word.
        private void WordSelected(object sender, DialogClickEventArgs e)
        {
            var selectedWord = randomWords[e.Which];
            UploadWord(selectedWord);
            tvHint.Text = selectedWord;

            var dialog = (AlertDialog)sender;
            dialog.Dismiss();
        }

        private Firebase.FirebaseOptions GetMyOptions()
        {
            return new Firebase.FirebaseOptions.Builder()
                .SetProjectId("ofekventuraproject-69677")
                .SetApplicationId("ofekventuraproject-69677")
                .SetApiKey("AIzaSyAF-dtvViwx20XS8nTSMjQWd-KRcM0y4Fg")
                .SetStorageBucket("ofekventuraproject-69677.appspot.com").Build();
        }



        private void BtnClear_Click(object sender, EventArgs e)
        {
            fingerPaintCanvasView.ClearAll();
        }

        // This method is called when the guess button is clicked. It checks if the entered guess is correct and updates the score and guess text accordingly. It also plays a sound indicating if the guess was right or wrong.
        private async void BtnGuess_Click(object sender, EventArgs e)
        {

            if (etGuess.Text.ToUpper() == word)
            {
                score += (time / 10) + timeTaken;
                MainActivity.user.Score = score;
                tvGuess.SetTextColor(Color.Green);
                tvGuess.Text = "Right Guess!\nScore: " + score;
                btnGuess.Enabled = false;
                MediaPlayer player = MediaPlayer.Create(this, Resource.Raw.right);
                player.Start();
                await client.SetAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/players/" + MainActivity.user.PushCode + "/status", true);
            }
            else
            {
                tvGuess.SetTextColor(Color.Red);
                tvGuess.Text = "Wrong Guess!";
                MediaPlayer player = MediaPlayer.Create(this, Resource.Raw.wrong);
                player.Start();
            }
            etGuess.Text = "";
        }

        private void SetLengthHint(string word)
        {
            System.Text.StringBuilder hint = new System.Text.StringBuilder();
            string[] words = Regex.Split(word, @"\s+");
            for (int i = 0; i < words.Length; i++)
            {
                hint.Append(words[i].Length + "- ");
                for (int j = words[i].Length - 1; j >= 0; j--)
                {
                    hint.Append("__ ");
                }

            }
            hint.Remove(hint.Length - 1, 1);
            tvHint.Text = hint.ToString();
        }

        private async void UploadWord(string word)
        {
            await client.SetAsync("RoomCodes/" + MainActivity.user.Uid + "/word", word);
        }

        private void ColorSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string strColor = (string)spinner.GetItemAtPosition(e.Position);
            Color strokeColor = Color.ParseColor(strColor);
            fingerPaintCanvasView.StrokeColor = strokeColor;
            //spinner.SetBackgroundColor(strokeColor); // Set the background color of the Spinner
        }

        private void WidthSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            float strokeWidth = new float[] { 2, 5, 10, 20, 50 }[e.Position];
            fingerPaintCanvasView.StrokeWidth = strokeWidth;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            GameEnd();
        }
    }
}
