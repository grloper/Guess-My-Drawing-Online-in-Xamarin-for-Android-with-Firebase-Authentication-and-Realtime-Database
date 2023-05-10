using Android.App;
using Android.Content;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;

namespace OfekVentura_Project.Classes
{
    public class ColorSpinnerAdapter : ArrayAdapter<string>
    {
        private Context _context;
        private List<string> _colors;

        public ColorSpinnerAdapter(Context context, int textViewResourceId, List<string> colors) : base(context, textViewResourceId, colors)
        {
            _context = context;
            _colors = colors;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return GetCustomView(position, convertView, parent);
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            return GetCustomView(position, convertView, parent);
        }

        private View GetCustomView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
            View row = inflater.Inflate(Android.Resource.Layout.SimpleSpinnerItem, parent, false);
            TextView label = (TextView)row.FindViewById(Android.Resource.Id.Text1);
            label.Gravity = GravityFlags.Center;
            label.Text = _colors[position];
            label.SetTextColor(Color.Transparent);

            ShapeDrawable circle = new ShapeDrawable(new OvalShape());
            circle.Paint.Color = Color.ParseColor(_colors[position]);
            circle.SetIntrinsicHeight(80);
            circle.SetIntrinsicWidth(80);

            label.SetCompoundDrawablesWithIntrinsicBounds(null, null, circle, null);

            return row;
        }
    }
}