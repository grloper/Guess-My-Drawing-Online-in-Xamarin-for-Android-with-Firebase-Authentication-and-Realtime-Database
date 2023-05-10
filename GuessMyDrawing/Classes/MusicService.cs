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
using Android.Media;
using AndroidX.Core.App;

namespace OfekVentura_Project
{
    [Service]
    public class MusicService : Service
    {
        private ISharedPreferences sp;
        //bool isPlaying;
        public static MediaPlayer mediaPlayer;
        public void SetVolume(int volume)
        {
            float log1 = (float)(Math.Log(100 - volume) / Math.Log(100));
            mediaPlayer.SetVolume(1 - log1, 1 - log1);
        }

        public override IBinder OnBind(Intent intent) { return null; }
        public override void OnCreate()
        {
            base.OnCreate();
            mediaPlayer = MediaPlayer.Create(this, Resource.Raw.Theme);
            mediaPlayer.Looping = true;
            mediaPlayer.SetVolume(100, 100);
            mediaPlayer.Completion += SongEnded;
            sp = this.GetSharedPreferences("details", FileCreationMode.Private);
        }

        private void SongEnded(object sender, EventArgs e)
        {
            this.OnDestroy();
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            int position = sp.GetInt("position", 0); //פקודת המשך מאיפה שנעצר
            mediaPlayer.SeekTo(position);
            mediaPlayer.Start();
            return base.OnStartCommand(intent, flags, startId);
        }

        public void RestartSong()
        {
            mediaPlayer.SeekTo(0);
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutBoolean("reset", false);
            editor.Commit();

        }

        public override void OnDestroy()
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt("position", mediaPlayer.CurrentPosition);
            editor.Commit();
            mediaPlayer.Stop();
            mediaPlayer.Release();
            base.OnDestroy();

        }
    }
}