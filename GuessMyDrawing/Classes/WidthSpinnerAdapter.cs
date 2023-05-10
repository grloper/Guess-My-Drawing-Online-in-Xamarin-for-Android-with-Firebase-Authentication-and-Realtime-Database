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
    public class WidthSpinnerAdapter : ArrayAdapter<float>
    {
        private Context _context;
        private List<float> _widths;

        public WidthSpinnerAdapter(Context context, int textViewResourceId, List<float> widths) : base(context, textViewResourceId, widths)
        {
            _context = context;
            _widths = widths;
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
            label.Text = _widths[position].ToString();
            label.SetTextColor(Color.Transparent);

            ShapeDrawable rectangle = new ShapeDrawable(new RectShape());
            rectangle.Paint.Color = Color.Black;
            rectangle.SetIntrinsicHeight((int)_widths[position] * 3); // Set the intrinsic height based on the width value
            rectangle.SetIntrinsicWidth(80);

            label.SetCompoundDrawablesWithIntrinsicBounds(null, null, rectangle, null);

            return row;
        }
    }
}