using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAV.MonitoringGroundStation.Models
{
    public class FlightData
    {
        public TimeSpan TimeStamp { get; set; }
        public double OmegaXCurrent { get; set; }
        public double OmegaXDesired { get; set; }
        public double OmegaYCurrent { get; set; }
        public double OmegaYDesired { get; set; }
        public double OmegaZCurrent { get; set; }
        public double OmegaZDesired { get; set; }
        public double BaroAltitudeCurrent { get; set; }
        public double BaroAltitudeDesired { get; set; }
        public double VelocityYCurrent { get; set; }
        public double VelocityYDesired { get; set; }
        public double AirSpeed { get; set; }
        public double OmegaXKp { get; set; }
        public double OmegaXKi { get; set; }
        public double OmegaYKp { get; set; }
        public double OmegaYKi { get; set; }
        public double OmegaZKp { get; set; }
        public double OmegaZKi { get; set; }
        public double VelocityYKp { get; set; }
        public double Pitch { get; set; }
        public double Roll { get; set; }
        public double Yaw { get; set; }
        public double Nz { get; set; }
        public double OmegaTurn { get; set; }
        public double RollDesired { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double GpsSpeed { get; set; }
        public double GpsCourse { get; set; }
        public int GpsValid { get; set; }
        public int ModePwm { get; set; }
        public string Mode { get; set; }
        public double BatteryVoltage { get; set; }
        public int ErsMode { get; set; }
    }
}
