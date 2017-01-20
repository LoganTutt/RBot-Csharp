using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpi3_Mbot
{
    /// <summary>
    /// Motor class for interacting with a single motor on the robot
    /// </summary>
    public class Motor
    {
        /// <summary>
        /// Motor ports on mCore board
        /// </summary>
        public enum MotorPort
        {
            M1=0x09,
            M2=0x0a
        }

        public const byte DEVICE_ID = 10;
        public const short MAX_SPEED = 255;
        public const short MAX_NEG_SPEED = -MAX_SPEED;
        public const short STOPPED = 0;

        private MotorPort port;


        public bool IsInverted { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">The motor port the robot is connected to</param>
        public Motor(MotorPort port)
        {
            this.port = port;
            IsInverted = false;
        }

        /// <summary>
        /// Set the speed of the motor.
        /// Speed will be clamped to between MAX_SPEED and MAX_NEG_SPEED
        /// </summary>
        /// <param name="speed">Speed to set the motor to</param>
        public void SetSpeed(int speed)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            if (IsInverted)
                speed *= -1;
            
            byte[] byteArr = BitConverter.GetBytes(ClampSpeed(speed));
            //System.Diagnostics.Debug.WriteLine("Calcs, " + port + ":" + watch.ElapsedMilliseconds);
            SerialDispatcher.SendMsg(DEVICE_ID, (byte)port,byteArr[0],byteArr[1]);
            System.Diagnostics.Debug.WriteLine("MotorWrite, " + port + ":" + watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Stops the motor
        /// </summary>
        public void Stop()
        {
            SetSpeed(STOPPED);
        }

        /// <summary>
        /// Clamps the motor speed to between MAX_SPEED and MAX_NEG_SPEED
        /// </summary>
        /// <param name="speed">The value to be clamped</param>
        /// <returns>The speed after being clamped</returns>
        public short ClampSpeed(int speed)
        {
            return (speed > MAX_SPEED) ? MAX_SPEED : (speed < MAX_NEG_SPEED) ? MAX_NEG_SPEED : (short) speed;
        }

    }
}
