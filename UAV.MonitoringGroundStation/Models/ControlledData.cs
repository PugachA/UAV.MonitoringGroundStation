using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAV.MonitoringGroundStation.Models
{
    public class ControlledData<T>
    {
        public T CurrentValue { get; set; }
        public T DesiredValue { get; set; }
        public double Kp { get; set; }
        public double Ki { get; set; }
    }
}
