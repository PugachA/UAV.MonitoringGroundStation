﻿using PrimaryFlightDisplay;
using RealTimeGraphX;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.WPF;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using UAV.MonitoringGroundStation.Models;

namespace UAV.MonitoringGroundStation.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainController PFDController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> OmegaXController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> OmegaYController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> OmegaZController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> VelocityYController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> BaroAltitudeController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> AirSpeedController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> OmegaTurnController { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> NzController { get; set; }

        public string PortName
        {
            get { return serialPort.PortName; }
            set
            {
                if (value != null)
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();

                    serialPort.PortName = value;
                }

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

        private FlightData _flightData;
        public FlightData FlightData
        {
            get => _flightData;
            set
            {
                if (value is null)
                    return;

                _flightData = value;

                OnPropertyChanged(nameof(FlightData));
            }
        }

        private SerialPort serialPort;
        private FlightDataExtractor flightDataExtractor;

        public MainWindowViewModel()
        {
            _flightData = new FlightData();

            var dataMapping = (ConfigurationManager.GetSection("MappingSettings/FlightDataMappings") as System.Collections.Hashtable)
                 .Cast<System.Collections.DictionaryEntry>()
                 .ToDictionary(n => n.Key.ToString(), n => Convert.ToInt32(n.Value));

            flightDataExtractor = new FlightDataExtractor(dataMapping);

            SerialPortInitialize();
            ControllersInitialize();

            var timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Tick += UpdatePfd;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();

            Task.Factory.StartNew(() =>
            {
                string message = null;
                TimeSpan x;
                var player = new MediaPlayer();
                var flightData = new FlightData();
                //message = "212819;9;57;-11;-50;15;-38;7636;0;220;1624;47;525;52;525;48;525;85;91;-14;1246;2510;0;0;38080810;55582000;1624;476;1;1750;700;0";
                while (true)
                {
                    try
                    {
                        if (!serialPort.IsOpen)
                            serialPort.Open();

                        message = serialPort.ReadLine();

                        flightData = flightDataExtractor.Extract(message);

                        if (flightData.Mode != FlightData.Mode)
                            SoundMode(player, flightData.Mode);

                        FlightData = flightData;
                        x = FlightData.TimeStamp;

                        OmegaXController.PushData(new TimeSpanDataPoint[] { x, x }, new DoubleDataPoint[] { FlightData.OmegaXDesired, FlightData.OmegaXCurrent });
                        OmegaYController.PushData(new TimeSpanDataPoint[] { x, x }, new DoubleDataPoint[] { FlightData.OmegaYDesired, FlightData.OmegaYCurrent });
                        OmegaZController.PushData(new TimeSpanDataPoint[] { x, x }, new DoubleDataPoint[] { FlightData.OmegaZDesired, FlightData.OmegaZCurrent });
                        VelocityYController.PushData(new TimeSpanDataPoint[] { x, x }, new DoubleDataPoint[] { FlightData.VelocityYDesired, FlightData.VelocityYCurrent });
                        BaroAltitudeController.PushData(new TimeSpanDataPoint[] { x, x }, new DoubleDataPoint[] { FlightData.BaroAltitudeDesired, FlightData.BaroAltitudeCurrent });
                        OmegaTurnController.PushData(x, FlightData.OmegaTurn);

                        AirSpeedController.PushData(x, FlightData.GpsSpeed);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        private async void UpdatePfd(object obj, EventArgs args)
        {
            PFDController.Roll = (-1)*FlightData.Roll;
            PFDController.Pitch = FlightData.Pitch;
            PFDController.Heading = FlightData.Yaw;
            PFDController.Altitude = FlightData.BaroAltitudeCurrent;
            PFDController.IAS = FlightData.GpsSpeed;
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

            var strokeThickness = 2;
            OmegaXController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            OmegaXController.Range.MaximumX = TimeSpan.FromSeconds(10);
            OmegaXController.Range.AutoY = true;
            OmegaXController.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            OmegaXController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "OmegaX Desired",
                Stroke = Colors.Green,
                StrokeThickness = 3
            });
            OmegaXController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "OmegaX Current",
                Stroke = Colors.Red,
                StrokeThickness = strokeThickness
            });

            OmegaYController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            OmegaYController.Range.MaximumX = TimeSpan.FromSeconds(10);
            OmegaYController.Range.AutoY = true;
            OmegaYController.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            OmegaYController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "OmegaY Desired",
                Stroke = Colors.Green,
                StrokeThickness = 3
            });
            OmegaYController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "OmegaY Current",
                Stroke = Colors.Red,
                StrokeThickness = strokeThickness
            });

            OmegaZController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            OmegaZController.Range.MaximumX = TimeSpan.FromSeconds(10);
            OmegaZController.Range.AutoY = true;
            OmegaZController.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            OmegaZController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "OmegaZ Desired",
                Stroke = Colors.Green,
                StrokeThickness = 3
            });
            OmegaZController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "OmegaZ Current",
                Stroke = Colors.Red,
                StrokeThickness = strokeThickness
            });

            VelocityYController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            VelocityYController.Range.MaximumX = TimeSpan.FromSeconds(10);
            VelocityYController.Range.AutoY = true;
            VelocityYController.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            VelocityYController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "VelocityY Desired",
                Stroke = Colors.Green,
                StrokeThickness = 3
            });
            VelocityYController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "VelocityY Current",
                Stroke = Colors.Red,
                StrokeThickness = strokeThickness
            });

            BaroAltitudeController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            BaroAltitudeController.Range.MaximumX = TimeSpan.FromSeconds(10);
            BaroAltitudeController.Range.AutoY = true;
            BaroAltitudeController.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            BaroAltitudeController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Altitude Desired",
                Stroke = Colors.Green,
                StrokeThickness = 3
            });
            BaroAltitudeController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Baro Altitude",
                Stroke = Colors.Red,
                StrokeThickness = strokeThickness
            });

            OmegaTurnController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            OmegaTurnController.Range.MaximumX = TimeSpan.FromSeconds(10);
            OmegaTurnController.Range.AutoY = true;
            OmegaTurnController.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            OmegaTurnController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "OmegaTurn",
                Stroke = Colors.Red,
                StrokeThickness = strokeThickness
            });

            AirSpeedController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            AirSpeedController.Range.MaximumX = TimeSpan.FromSeconds(10);
            AirSpeedController.Range.AutoY = true;
            AirSpeedController.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            AirSpeedController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "GpsSpeed",
                Stroke = Colors.Red,
                StrokeThickness = strokeThickness
            });

            NzController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            NzController.Range.MaximumX = TimeSpan.FromSeconds(10);
            NzController.Range.AutoY = true;
            NzController.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            NzController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "nz",
                Stroke = Colors.Red,
                StrokeThickness = strokeThickness
            });
        }

        private void SoundMode(MediaPlayer player, string mode)
        {
            string parseMode = mode;
            if (string.IsNullOrEmpty(mode))
                parseMode = "UNKNOWN MODE";

            if (mode == "OMEGA_STAB K_TUNE" || mode == "OMEGA_STAB I_TUNE")
                parseMode = "OMEGA_STAB";

            if (mode == "VY_STAB K_TUNE")
                parseMode = "COMMAND";

            player.Open(new Uri($"Sounds\\{parseMode}.mp3", UriKind.Relative));
            player.Play();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
