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
using Firebase;
using Firebase.Auth;

namespace OfekVentura_Project
{
    internal class FB_Data
    {
        private readonly FirebaseApp app;
        private readonly FirebaseAuth auth;
        public FB_Data()
        {
            app = FirebaseApp.InitializeApp(Application.Context);
            if (app is null)
            {
                FirebaseOptions options = GetMyOptions();
                app = FirebaseApp.InitializeApp(Application.Context, options);
            }
            auth = FirebaseAuth.Instance;
        }

        public Android.Gms.Tasks.Task CreateUser(string email, string password)
        {
            return auth.CreateUserWithEmailAndPassword(email, password);
        }


        public Android.Gms.Tasks.Task SignInAnonymously()
        {
            return auth.SignInAnonymously();
            
        }
        public void SignOut()
        {
          auth.SignOut();
        }
        public void DelteUser()
        {
            auth.CurrentUser.DeleteAsync();
        }
        public Android.Gms.Tasks.Task SignIn(string email, string password)
        {
            return auth.SignInWithEmailAndPassword(email, password);
        }

        public Android.Gms.Tasks.Task ResetPassword(string email)
        {
            return auth.SendPasswordResetEmail(email);
        }

        private FirebaseOptions GetMyOptions()
        {
            return new FirebaseOptions.Builder()
                .SetProjectId("ofekventuraproject-69677")
                .SetApplicationId("ofekventuraproject-69677")
                .SetApiKey("AIzaSyAF-dtvViwx20XS8nTSMjQWd-KRcM0y4Fg")
                .SetStorageBucket("ofekventuraproject-69677.appspot.com").Build();
        }
    }
}