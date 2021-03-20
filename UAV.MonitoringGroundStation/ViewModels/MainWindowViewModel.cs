using PrimaryFlightDisplay;
using RealTimeGraphX;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using UAV.MonitoringGroundStation.Models;

namespace UAV.MonitoringGroundStation.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainController PFDController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> OmegaXController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller1 { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller2 { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller3 { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller4 { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller5 { get; set; }

        public string PortName
        {
            get { return serialPort.PortName; }
            set
            {
                if (value != null)
                    serialPort.PortName = value;

                OnPropertyChanged(nameof(PortName));
            }
        }
        public ObservableCollection<string> PortNames { get; set; }

        public int BaudRate
        {
            get { return serialPort.BaudRate; }
            set
            {
                serialPort.BaudRate = value;
                OnPropertyChanged(nameof(BaudRate));
            }
        }
        public ObservableCollection<int> BaudRates { get; set; }

        private SerialPort serialPort;
        private FlightDataExtractor flightDataExtractor;

        public MainWindowViewModel()
        {
            var dataMapping = (ConfigurationManager.GetSection("MappingSettings/FlightDataMappings") as System.Collections.Hashtable)
                 .Cast<System.Collections.DictionaryEntry>()
                 .ToDictionary(n => n.Key.ToString(), n => Convert.ToInt32(n.Value));

            flightDataExtractor = new FlightDataExtractor(dataMapping);
            flightDataExtractor.Extract("10005;1800;175;170;1600;1500;1400;15000;110;120;50;1;15;2;13;3;10;50;1700;0");

            SerialPortInitialize();
            ControllersInitialize();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var y = System.Windows.Forms.Cursor.Position.Y;

                    var x = watch.Elapsed;

                    OmegaXController.PushData(x, y);
                    Controller1.PushData(x, y);
                    Controller2.PushData(x, y);
                    Controller3.PushData(x, y);
                    Controller4.PushData(x, y);
                    Controller5.PushData(x, y);
                    Thread.Sleep(10);
                }
            });
        }

        private void SerialPortInitialize()
        {
            serialPort = new SerialPort();
            PortNames = new ObservableCollection<string>(SerialPort.GetPortNames());
            BaudRates = new ObservableCollection<int>(new int[] { 9600, 19200, 38400, 57600, 74880, 115200 });
            BaudRate = 57600;
        }

        private void ControllersInitialize()
        {
            PFDController = new MainController();
            PFDController.Draw();

            OmegaXController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            OmegaXController.Range.MaximumX = TimeSpan.FromSeconds(10);
            OmegaXController.Range.AutoY = true;
            OmegaXController.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            OmegaXController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });

            OmegaXController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 2",
                Stroke = Colors.Red,
            });

            Controller1 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller1.Range.MaximumX = TimeSpan.FromSeconds(10);
            Controller1.Range.AutoY = true;
            Controller1.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            Controller1.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });

            Controller2 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller2.Range.MinimumY = 0;
            Controller2.Range.MaximumY = 1080;
            Controller2.Range.MaximumX = TimeSpan.FromSeconds(10);
            Controller2.Range.AutoY = true;
            Controller2.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            Controller2.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });

            Controller3 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller3.Range.MinimumY = 0;
            Controller3.Range.MaximumY = 1080;
            Controller3.Range.MaximumX = TimeSpan.FromSeconds(10);
            Controller3.Range.AutoY = true;
            Controller3.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            Controller3.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });

            Controller4 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller4.Range.MinimumY = 0;
            Controller4.Range.MaximumY = 1080;
            Controller4.Range.MaximumX = TimeSpan.FromSeconds(10);
            Controller4.Range.AutoY = true;
            Controller4.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            Controller4.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });

            Controller5 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller5.Range.MinimumY = 0;
            Controller5.Range.MaximumY = 1080;
            Controller5.Range.MaximumX = TimeSpan.FromSeconds(10);
            Controller5.Range.AutoY = true;
            Controller5.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            Controller5.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
