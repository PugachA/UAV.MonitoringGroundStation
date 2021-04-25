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
            var velocityYKp = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.VelocityYKp)]]);

            var airSpeed = 0.01 * int.Parse(splittedData[_dataMapping[nameof(FlightData.AirSpeed)]]);

            var pitch = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.Pitch)]]);
            var roll = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.Roll)]]);
            var rollDesired = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.RollDesired)]]);
            var yaw = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.Yaw)]]);
            var nz = 0.001 * int.Parse(splittedData[_dataMapping[nameof(FlightData.Nz)]]);
            var omegaTurn = 0.1 * int.Parse(splittedData[_dataMapping[nameof(FlightData.OmegaTurn)]]);

            var modePwm = int.Parse(splittedData[_dataMapping[nameof(FlightData.ModePwm)]]);
            var batteryVoltage = 0.001 * int.Parse(splittedData[_dataMapping[nameof(FlightData.BatteryVoltage)]]);
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
                AirSpeed = airSpeed,

                Pitch = pitch,
                Roll = roll,
                RollDesired = rollDesired,
                Yaw = yaw,
                Nz = nz,
                OmegaTurn = omegaTurn,

                ModePwm = modePwm,
                Mode = GetModeName(modePwm),
                BatteryVoltage = batteryVoltage,
                ErsMode = ersMode
            };

            return fligthData;
        }

        private string GetModeName(int modePwm)
        {
            switch (modePwm)
            {
                case int pwm when pwm < 1000: return "DARM";
                case int pwm when pwm >= 1000 && pwm < 1100: return "DIRECT";
                case int pwm when pwm >= 1100 && pwm < 1300: return "OMEGA_STAB";
                case int pwm when pwm >= 1300 && pwm < 1400: return "OMEGA_STAB K_TUNE";
                case int pwm when pwm >= 1400 && pwm < 1500: return "OMEGA_STAB";
                case int pwm when pwm >= 1500 && pwm < 1700: return "OMEGA_STAB I_TUNE";
                case int pwm when pwm >= 1700 && pwm < 1800: return "COMMAND";
                case int pwm when pwm >= 1800 && pwm < 1900: return "VY_STAB K_TUNE";
                case int pwm when pwm >= 1900 && pwm < 1950: return "DIRECT FLAPS";
                case int pwm when pwm > 2000: return "ERS";
                default: return "UNKNOWN MODE";
            }
        }
    }
}
