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
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBatteryChanged })]

    public class BroadcastBattery : BroadcastReceiver
    {
        TextView tvBattery;
        public BroadcastBattery()
        {

        }
        public BroadcastBattery(TextView tvBattery)
        {
            this.tvBattery = tvBattery;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            int battery = intent.GetIntExtra("level", 0);
            tvBattery.Text = "" + battery + "%";



        }

    }
}