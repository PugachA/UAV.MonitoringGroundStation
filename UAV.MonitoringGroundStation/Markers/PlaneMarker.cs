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
        private readonly Bitmap _planeBitmap;
        private readonly string _toolTip;

        public PlaneMarker(PointLatLng pos, string imagePath, float initialAngel, string toolTip = "Plane") : base(pos)
        {
            _planeBitmap = new Bitmap(System.IO.Path.GetFullPath(imagePath));
            _planeBitmap = RotateBitmap(_planeBitmap, initialAngel);

            _toolTip = toolTip;

            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(_planeBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(_planeBitmap.Width, _planeBitmap.Height));

            this.Shape = new System.Windows.Controls.Image
            {
                Width = _planeBitmap.Width,
                Height = _planeBitmap.Height,
                Source = bitmapSource,
                ToolTip = _toolTip,
            };

            this.Offset = new System.Windows.Point(-(double)_planeBitmap.Width / 2, -(double)_planeBitmap.Height / 2);
        }

        public void RotateMarker(float angel)
        {
            var bitmap = RotateBitmap(_planeBitmap, angel);

            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));

            this.Shape = new System.Windows.Controls.Image
            {
                Width = bitmap.Width,
                Height = bitmap.Height,
                Source = bitmapSource,
                ToolTip = _toolTip,
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
