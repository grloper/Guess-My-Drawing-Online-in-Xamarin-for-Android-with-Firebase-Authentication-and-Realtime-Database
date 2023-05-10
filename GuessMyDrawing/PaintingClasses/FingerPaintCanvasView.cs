using System;
using System.Collections.Generic;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Graphics;
using Android.Widget;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.EventStreaming;
using Newtonsoft.Json;
using Android.Media;
using Android.Gms.Tasks;
using System.Threading.Tasks;
using FireSharp.Response;
using Android.OS;
using Java.Interop;
using System.Threading;
using System.Linq;
using Java.Lang;
using System.IO;
using System.ComponentModel;

namespace OfekVentura_Project
{
    public class FingerPaintCanvasView : View
    {
        Context context;
        public Bitmap bitmap;
        // Two collections for storing polylines
        private Dictionary<int, FingerPaintPolyline> inProgressPolylines = new Dictionary<int, FingerPaintPolyline>();
        private List<FingerPaintPolyline> completedPolylines = new List<FingerPaintPolyline>();
        private Paint paint = new Paint();

        IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "Gq3wvgf3S61XTy92Dhg5fDdh5qpDdmGW0ERpnDn4",
            BasePath = "https://ofekventuraproject-69677-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        // This method uploads a base64 encoded image of the current canvas to Firebase Realtime Database using FireSharp.
        private async System.Threading.Tasks.Task UploadAsync()
        {
            // Get the width and height of the canvas
            int width = this.Width;
            int height = this.Height;
            // Create a new Bitmap object with the same width and height as the canvas
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            // Create a new Canvas object using the Bitmap
            Canvas canvas = new Canvas(bitmap);
            // Draw the contents of the fingerPaintCanvasView onto the canvas
            this.Draw(canvas);
            MemoryStream stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            byte[] bytes = stream.ToArray();
            string base64 = Convert.ToBase64String(bytes);
            // Save Base64 string to Firebase Realtime Database using FireSharp
            var data = new
            {
                image = base64
            };
            await client.SetAsync("RoomCodes/" + MainActivity.user.OwnerUid + "/image", data);
        }

        public FingerPaintCanvasView(Context context) : base(context)
        {
            this.context = context;
            Initialize();
        }
        public FingerPaintCanvasView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            this.context = context;
            Initialize();
        }

        void Initialize()
        {
             
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {
            }
        }

        // External interface accessed from MainActivity
        public Color StrokeColor { set; get; } = Color.Red;
        public float StrokeWidth { set; get; } = 2;
        public void ClearAll()
        {
            //TRUE FALSE
            completedPolylines.Clear();
            Invalidate();
        }

        // This method handles touch events on the canvas and updates the polylines accordingly.
        public override bool OnTouchEvent(MotionEvent args)
        {
            
            // Get the pointer index
            int pointerIndex = args.ActionIndex;
            // Get the id to identify a finger over the course of its progress
            int id = args.GetPointerId(pointerIndex);

            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.ActionMasked)
            {
                
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:

                    FingerPaintPolyline polyline = new FingerPaintPolyline
                    {
                        Color = StrokeColor,
                        StrokeWidth = StrokeWidth
                    };
                    //adding the polyline to the dictionary
                    inProgressPolylines.Add(id, polyline);
                    inProgressPolylines[id].Path.MoveTo(args.GetX(pointerIndex), args.GetY(pointerIndex));
                    break;

                case MotionEventActions.Move:

                    // Multiple Move events are bundled, so handle them differently
                    for (pointerIndex = 0; pointerIndex < args.PointerCount; pointerIndex++)
                    {

                        id = args.GetPointerId(pointerIndex);
                        inProgressPolylines[id].Path.LineTo(args.GetX(pointerIndex),
                                                            args.GetY(pointerIndex));
                    }
             
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    completedPolylines.Add(inProgressPolylines[id]);
                    inProgressPolylines.Remove(id);

                    break;
                case MotionEventActions.Cancel:
                    inProgressPolylines.Remove(id);
                    break;
            } 
            // Invalidate to update the view
            Invalidate();
            System.Threading.Tasks.Task.Run(async () => await UploadAsync());
            // Request continued touch input
            return true;
        }
        public void SetBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            Invalidate();
        }
        // This method draws completed and in-progress polylines on the canvas and a bitmap if it exists.
        protected override void OnDraw(Canvas canvas)
        {
            try
            {
                base.OnDraw(canvas);
                paint.SetStyle(Paint.Style.Stroke);
                paint.StrokeCap = Paint.Cap.Round;
                paint.StrokeJoin = Paint.Join.Round;

                // Draw the completed polylines
                foreach (FingerPaintPolyline polyline in completedPolylines)
                {

                    paint.Color = polyline.Color;
                    paint.StrokeWidth = polyline.StrokeWidth;
                    canvas.DrawPath(polyline.Path, paint);
                }

                // Draw the in-progress polylines
                foreach (FingerPaintPolyline polyline in inProgressPolylines.Values)
                {
                    paint.Color = polyline.Color;
                    paint.StrokeWidth = polyline.StrokeWidth;
                    canvas.DrawPath(polyline.Path, paint);
                }
                if (bitmap != null)
                {
                    canvas.DrawBitmap(bitmap, 0, 0, paint);
                }
            }
            catch (System.Exception) { }
        }
  
    }
}