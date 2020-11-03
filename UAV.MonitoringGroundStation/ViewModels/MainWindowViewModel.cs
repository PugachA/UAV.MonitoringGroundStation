using RealTimeGraphX;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UAV.MonitoringGroundStation.ViewModels
{
    public class MainWindowViewModel
    {
        private string _background = "Red";
        public string BackColor
        {
            get
            {
                return _background;
            }

            set
            {
                _background = value;
            }
        }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller1 { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller2 { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller3 { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller4 { get; set; }
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller5 { get; set; }

        public MainWindowViewModel()
        {
            Controller = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller.Range.MinimumY = 0;
            Controller.Range.MaximumY = 1080;
            Controller.Range.MaximumX = TimeSpan.FromSeconds(10);
            Controller.Range.AutoY = true;
            Controller.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });

            Controller1 = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller1.Range.MinimumY = 0;
            Controller1.Range.MaximumY = 1080;
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

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var y = System.Windows.Forms.Cursor.Position.Y;

                    var x = watch.Elapsed;

                    Controller.PushData(x, y);
                    Controller1.PushData(x, y);
                    Controller2.PushData(x, y);
                    Controller3.PushData(x, y);
                    Controller4.PushData(x, y);
                    Controller5.PushData(x, y);
                    Thread.Sleep(10);
                }
            });
        }
    }
}
