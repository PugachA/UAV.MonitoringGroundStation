using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAV.MonitoringGroundStation.Models
{
    public class FlightDataExtractor
    {
        private readonly Dictionary<string, int> _dataMapping;

        public FlightDataExtractor(Dictionary<string, int> dataMapping)
        {
            if (dataMapping is null)
                throw new ArgumentNullException($"{nameof(dataMapping)} cannot be null");

            _dataMapping = dataMapping;
        }

        public FlightData Extract(string data)
        {
            if (data is null)
                throw new ArgumentNullException($"{nameof(data)} cannot be null");

            var splittedData = data.Split(';');

            if (splittedData.Length != _dataMapping.Count)
                throw new ArgumentException($"Received data count '{splittedData.Length}' not equal with mapping '{_dataMapping.Count}'. Data:{data}");

            var timeStamp = int.Parse(splittedData[_dataMapping[nameof(FlightData.TimeStamp)]]);

            //TODO Добавить TryParse
            var omegaXCurrent = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaXCurrent)]]);
            var omegaXDesired = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaXDesired)]]);
            var omegaXKp = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaXKp)]]);
            var omegaXKi = 0.01 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaXKi)]]);

            var omegaYCurrent = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaYCurrent)]]);
            var omegaYDesired = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaYDesired)]]);
            var omegaYKp = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaYKp)]]);
            var omegaYKi = 0.01 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaYKi)]]);

            var omegaZCurrent = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaZCurrent)]]);
            var omegaZDesired = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaZDesired)]]);
            var omegaZKp = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaZKp)]]);
            var omegaZKi = 0.01 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaZKi)]]);

            var baroAltitudeCurrent = 0.01 * int.Parse(splittedData[_dataMapping[nameof(FlightData.BaroAltitudeCurrent)]]);

            var velocityYCurrent = 0.01 * int.Parse(splittedData[_dataMapping[nameof(FlightData.VelocityYCurrent)]]);
            var velocityYDesired = 0.01 * int.Parse(splittedData[_dataMapping[nameof(FlightData.VelocityYDesired)]]);
            var velocityYKp= 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.VelocityYKp)]]);

            var beta = int.Parse(splittedData[_dataMapping[nameof(FlightData.Beta)]]);

            var modePwm = int.Parse(splittedData[_dataMapping[nameof(FlightData.ModePwm)]]);
            var ersMode = int.Parse(splittedData[_dataMapping[nameof(FlightData.ErsMode)]]);

            var fligthData = new FlightData
            {
                TimeStamp = TimeSpan.FromMilliseconds(timeStamp),

                OmegaXCurrent = omegaXCurrent,
                OmegaXDesired = omegaXDesired,
                OmegaXKp = omegaXKp,
                OmegaXKi = omegaXKi,

                OmegaYCurrent = omegaYCurrent,
                OmegaYDesired = omegaYDesired,
                OmegaYKp = omegaYKp,
                OmegaYKi = omegaYKi,

                OmegaZCurrent = omegaZCurrent,
                OmegaZDesired = omegaZDesired,
                OmegaZKp = omegaZKp,
                OmegaZKi = omegaZKi,

                VelocityYCurrent = velocityYCurrent,
                VelocityYDesired = velocityYDesired,
                VelocityYKp = velocityYKp,

                BaroAltitudeCurrent = baroAltitudeCurrent,
                Beta = beta,
                ModePwm = modePwm,
                ErsMode = ersMode
            };

            return fligthData;
        }
    }
}
