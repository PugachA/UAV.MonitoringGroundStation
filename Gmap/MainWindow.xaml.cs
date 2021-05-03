using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Device.Location;
using System.Windows.Threading;
using System.Configuration;
using System.Net.NetworkInformation;

namespace Gmap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeoCoordinateWatcher watcher;

        private GMapRoute gMapRoute;
        private List<GMapMarker> wayMarkerList;

        private PointLatLng planePoint = new PointLatLng(55.582000, 38.080810);
        private GMapMarker planeMarker;

        private PointLatLng stationPoint;
        private GMapMarker stationMarker;

        public MainWindow()
        {
            InitializeComponent();

            watcher = new GeoCoordinateWatcher();
            watcher.StatusChanged += Watcher_StatusChanged;
            watcher.Start();

            var timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Tick += UpdateMap;
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Start();
        }

        private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Ready)
            {
                if (!watcher.Position.Location.IsUnknown)
                {
                    stationPoint.Lat = watcher.Position.Location.Latitude;
                    stationPoint.Lng = watcher.Position.Location.Longitude;
                }
            }
        }

        private void UpdateMap(object obj, EventArgs args)
        {
            //if gps valid
            planePoint.Lat += 0.000100;

            if (stationMarker != null)
                stationMarker.Position = stationPoint;

            if (planeMarker != null)
                planeMarker.Position = planePoint;

            if (gMapRoute is null)
            {
                var list = new List<PointLatLng> { planePoint, planePoint };
                gMapRoute = new GMapRoute(list);
                gMapRoute.Shape = new Path() { Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 3, ToolTip = "Plane route" };

                gMap.Markers.Add(gMapRoute);
            }
            else
            {
                gMapRoute.Points.Add(planePoint);
            }

            gMap.Position = planePoint;
        }

        private void GMap_Loaded(object sender, RoutedEventArgs e)
        {
            gMap.Bearing = 0;
            //Перетаскивание карты
            gMap.CanDragMap = true;
            //Перетаскивание карты левой кнопки мыши
            gMap.DragButton = MouseButton.Left;
            gMap.MaxZoom = 18;
            gMap.MinZoom = 2;
            //Курсор мыши в центре карты
            gMap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            //Скрытие внешней сетки карты
            gMap.ShowTileGridLines = false;
            //Начальный зум
            gMap.Zoom = 16;
            //Убрать красный крестик
            gMap.ShowCenter = false;
            gMap.MapProvider = GMapProviders.GoogleHybridMap;

            if (!PingNetwork("pingtest.com"))
            {
                GMaps.Instance.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection available, going to CacheOnly mode.",
                    "GMap.NET - Demo.WindowsPresentation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
                GMaps.Instance.Mode = AccessMode.ServerOnly;

            if (!stationPoint.IsEmpty)
                gMap.Position = new PointLatLng(stationPoint.Lat, stationPoint.Lng);

            stationMarker = new GMapMarker(stationPoint);
            stationMarker.Shape = new Ellipse
            {
                Width = 10,
                Height = 10,
                Stroke = Brushes.Blue,
                Fill = Brushes.Blue,
                StrokeThickness = 1.5,
                ToolTip = "Station"
            };
            gMap.Markers.Add(stationMarker);

            planeMarker = new GMapMarker(stationPoint);
            planeMarker.Shape = new Ellipse
            {
                Width = 7,
                Height = 7,
                Stroke = Brushes.Red,
                Fill = Brushes.Red,
                StrokeThickness = 1.5,
                ToolTip = "Plane"
            };
            gMap.Markers.Add(planeMarker);

            var wayPoints = (ConfigurationManager.GetSection("WayPointSettings/WayPoints") as System.Collections.Hashtable)
                .Cast<System.Collections.DictionaryEntry>()
                .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());

            wayMarkerList = new List<GMapMarker>();
            foreach (var wayPoint in wayPoints)
            {
                var splitted = wayPoint.Value.Split(';');
                var point = new PointLatLng(Convert.ToDouble(splitted[0]), Convert.ToDouble(splitted[1]));

                var marker = new GMapMarker(point);
                marker.Shape = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Brushes.Green,
                    Fill = Brushes.Green,
                    StrokeThickness = 1.5,
                    ToolTip = wayPoint.Key
                };

                wayMarkerList.Add(marker);
                gMap.Markers.Add(marker);
            }

            gMap.Position = stationPoint;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            gMap.Markers?.Clear();
            gMapRoute?.Clear();

            gMap.Markers.Add(stationMarker);
            gMap.Markers.Add(planeMarker);

            foreach (var marker in wayMarkerList)
                gMap.Markers.Add(marker);
        }

        private bool PingNetwork(string hostNameOrAddress)
        {
            bool pingStatus;

            using (var p = new Ping())
            {
                var buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                int timeout = 4444; // 4s

                try
                {
                    var reply = p.Send(hostNameOrAddress, timeout, buffer);
                    pingStatus = reply.Status == IPStatus.Success;
                }
                catch (Exception)
                {
                    pingStatus = false;
                }
            }

            return pingStatus;
        }
    }
}
