using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpi3_Mbot
{
    public class Ultrasonic : Sensor
    {
        public override byte DEVICE_ID
        {
            get
            {
                return 0x01;
            }
        }

        public Ultrasonic(Port port) : base(port)
        {
        }

        public override double Read()
        {
            return PollSensor();
        }
    }
}
