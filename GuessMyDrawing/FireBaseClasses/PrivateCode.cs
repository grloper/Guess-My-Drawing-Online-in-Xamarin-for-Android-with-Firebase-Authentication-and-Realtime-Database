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

namespace OfekVentura_Project
{
    public class PrivateCode
    {
        public string myCode { get; set; }
        public string word { get; set; }
        public int rounds { get; set; }
        public int drawCount { get; set; }
        public bool isPlay { get; set; }
        public bool nextRound { get; set; }
        public int drawTimeSeconds { get; set; }
        public string customWords { get; set; }

    }
}