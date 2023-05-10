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
    [Activity(Label = "InstructionActivity")]
    public class InstructionActivity : Activity
    {

        private Button inst_btnBack;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.instruction_layout);
            inst_btnBack = FindViewById<Button>(Resource.Id.inst_btnBack);
            inst_btnBack.Click += Inst_btnBack_Click;
            // Create your application here
        }

        private void Inst_btnBack_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}