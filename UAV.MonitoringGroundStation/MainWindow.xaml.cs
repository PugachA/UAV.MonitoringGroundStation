using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using PrimaryFlightDisplay;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Device.Location;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using UAV.MonitoringGroundStation.Markers;
using UAV.MonitoringGroundStation.Models;
using UAV.MonitoringGroundStation.ViewModels;

namespace UAV.MonitoringGroundStation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeoCoordinateWatcher watcher;

        private GMapRoute gMapRoute;
        private List<GMapMarker> wayMarkerList;

        private PointLatLng planePoint;
        private PlaneMarker planeMarker;

        private PointLatLng stationPoint;
        private GMapMarker stationMarker;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            InitializeGMap();

            watcher = new GeoCoordinateWatcher();
            watcher.StatusChanged += Watcher_StatusChanged;
            watcher.Start();

            var timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Tick += UpdateMap;
            timer.Interval = TimeSpan.FromMilliseconds(100);
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
            var viewModel = (MainWindowViewModel)DataContext;

            if (viewModel.FlightData.GpsValid == 1)
            {
                //planePoint.Lat += 0.000100;

                planePoint.Lat = viewModel.FlightData.Latitude;
                planePoint.Lng = viewModel.FlightData.Longitude;

                if (stationMarker != null)
                    stationMarker.Position = stationPoint;

                if (planeMarker != null)
                {
                    planeMarker.Position = planePoint;
                    planeMarker.RotateMarker((float)viewModel.FlightData.GpsCourse);
                }

                if (gMapRoute is null)
                {
                    var list = new List<PointLatLng> { planePoint, planePoint };
                    gMapRoute = new GMapRoute(list);
                    gMapRoute.Shape = new System.Windows.Shapes.Path() { Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 3, ToolTip = "Plane route" };

                    gMap.Markers.Add(gMapRoute);
                }
                else
                {
                    gMapRoute.Points.Add(planePoint);
                }

                gMap.Position = planePoint;
            }
        }

        private void InitializeGMap()
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
                Stroke = System.Windows.Media.Brushes.Blue,
                Fill = System.Windows.Media.Brushes.Blue,
                StrokeThickness = 1.5,
                ToolTip = "Station"
            };
            gMap.Markers.Add(stationMarker);

            planeMarker = new PlaneMarker(stationPoint, "Images\\plane_course_yellow.png", -90);
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
                marker.Shape = new System.Windows.Controls.Image
                {
                    Width = 16,
                    Height = 16,
                    Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("Images/marker.png"))),
                    ToolTip = wayPoint.Key
                };
                marker.Offset = new System.Windows.Point(-8, -8);

                wayMarkerList.Add(marker);
                gMap.Markers.Add(marker);
            }

            gMap.Position = stationPoint;
        }

        private void ComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.PortNames.Clear();
            viewModel.PortNames.AddRange(SerialPort.GetPortNames());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.OmegaXController.Clear();
            viewModel.OmegaYController.Clear();
            viewModel.OmegaZController.Clear();
            viewModel.VelocityYController.Clear();
            viewModel.BaroAltitudeController.Clear();
            viewModel.OmegaTurnController.Clear();
            viewModel.NzController.Clear();
            viewModel.AirSpeedController.Clear();
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            gMap.Markers?.Clear();
            gMapRoute?.Clear();
            gMapRoute = null;

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
