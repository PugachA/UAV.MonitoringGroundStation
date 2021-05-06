using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace UAV.MonitoringGroundStation.Markers
{
    public class PlaneMarker : GMapMarker
    {
        private readonly Bitmap _bitmap;

        public PlaneMarker(PointLatLng pos, float initialAngel) : base(pos)
        {
            _bitmap = new Bitmap(System.IO.Path.GetFullPath("Images\\plane.png"));
            _bitmap = RotateBitmap(_bitmap, initialAngel);

            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(_bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(_bitmap.Width, _bitmap.Height));

            this.Shape = new System.Windows.Controls.Image
            {
                Width = _bitmap.Width,
                Height = _bitmap.Height,
                Source = bitmapSource,
                ToolTip = "Plane",
            };

            this.Offset = new System.Windows.Point(-(double)_bitmap.Width / 2, -(double)_bitmap.Height / 2);
        }

        public void RotateMarker(float angel)
        {
            var bitmap = RotateBitmap(_bitmap, angel);

            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));

            this.Shape = new System.Windows.Controls.Image
            {
                Width = bitmap.Width,
                Height = bitmap.Height,
                Source = bitmapSource,
                ToolTip = "Plane",
            };
        }

        private Bitmap RotateBitmap(Bitmap bitmap, float angel)
        {
            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            newBitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
            using (var g = Graphics.FromImage(newBitmap))
            {
                g.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2);
                g.RotateTransform(angel);
                g.TranslateTransform(-(float)bitmap.Width / 2, -(float)bitmap.Height / 2);
                g.DrawImage(bitmap, new System.Drawing.Point(0, 0));

                return newBitmap;
            }
        }
    }
}
