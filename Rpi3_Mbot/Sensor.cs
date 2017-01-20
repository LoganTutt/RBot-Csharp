using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpi3_Mbot
{
    /// <summary>
    /// Abstract class for all sensors
    /// </summary>
    public abstract class Sensor
    {
        /// <summary>
        /// Ports on the robot that can have sensors attached
        /// </summary>
        public enum Port
        {
            Port1 = 1,
            Port2 = 2,
            Port3 = 3,
            Port4 = 4
        }

        public abstract byte DEVICE_ID { get; }

        private Port port;

        /// <summary>
        /// Constructor for a new sensor
        /// </summary>
        /// <param name="port">Port to create the sensor on</param>
        public Sensor(Port port)
        {
            this.port = port;
        }

        /// <summary>
        /// Reads the sensor and returns the double value of the sensor
        /// </summary>
        /// <returns>The value of the sensor</returns>
        public abstract double Read();

        /// <summary>
        /// Polls the sensor over serial
        /// </summary>
        /// <param name="slot">The slot of the sensor if needed</param>
        /// <returns>The value of the sensor</returns>
        protected virtual double PollSensor(byte slot = 0)
        {
            return SerialDispatcher.GetDouble(DEVICE_ID, (byte)port, slot);
        }


    }
}
