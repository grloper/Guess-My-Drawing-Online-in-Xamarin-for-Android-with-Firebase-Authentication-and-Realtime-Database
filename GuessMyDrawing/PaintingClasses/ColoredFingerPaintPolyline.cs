using Android.App;
using Android.Content;
using Android.Graphics;
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
     class ColoredFingerPaintPolyline : FingerPaintPolyline
    {
        public ColoredFingerPaintPolyline(Color color) : base()
        {
            Color = color;
        }
    }
}