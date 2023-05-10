using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FbAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfekVentura_Project
{
     class SP_data
    {
        private readonly ISharedPreferences sp;

        public SP_data(Context ctx)
        {
            sp = ctx.GetSharedPreferences(General.SP_FILE_NAME, FileCreationMode.Private);
        }

        public string GetStringValue(string key)
        {
            return sp.GetString(key, string.Empty);
        }

        public bool PutStringValue(string key, string value)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutString(key, value);
            return editor.Commit();
        }
    }
}