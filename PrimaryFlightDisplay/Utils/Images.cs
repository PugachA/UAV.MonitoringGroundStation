using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PrimaryFlightDisplay
{
    public static class Images
    {
       public static readonly BitmapImage PitchIndicatorImage = new BitmapImage(new Uri("pack://application:,,,/PrimaryFlightDisplay;component/Images/pitch_indicator.png", UriKind.Absolute));
       public static readonly BitmapImage PitchOpacityMaskImage = new BitmapImage(new Uri("pack://application:,,,/PrimaryFlightDisplay;component/Images/pitch_opacity_mask.png", UriKind.Absolute));
        public static readonly BitmapImage PitchScaleImage = new BitmapImage(new Uri("pack://application:,,,/PrimaryFlightDisplay;component/Images/pitch_scale.png", UriKind.Absolute));
        public static readonly BitmapImage RollIndicatorImage = new BitmapImage(new Uri("pack://application:,,,/PrimaryFlightDisplay;component/Images/roll_indicator.png", UriKind.Absolute));
        public static readonly BitmapImage RollScaleImage = new BitmapImage(new Uri("pack://application:,,,/PrimaryFlightDisplay;component/Images/roll_scale.png", UriKind.Absolute));
    }
}
