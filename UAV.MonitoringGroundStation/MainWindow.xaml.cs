using PrimaryFlightDisplay;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
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
using UAV.MonitoringGroundStation.ViewModels;

namespace UAV.MonitoringGroundStation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
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
            viewModel.AirSpeedController.Clear();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.PitchController.Clear();
            viewModel.RollController.Clear();
            viewModel.YawController.Clear();
            viewModel.NzController.Clear();
        }
    }
}
