using Android.Graphics;
using OfekVentura_Project;

namespace OfekVentura_Project
{
    class FingerPaintPolyline
    {
        public FingerPaintPolyline()
        {
            Path = new Path();
        }

        public Color Color { set; get; }

        public float StrokeWidth { set; get; }

        public Path Path { private set; get; }
    }
}