// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Rpi3_Mbot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private DispatcherTimer timer;
        private bool dir = true;

        private double goalDist = 20;
        private double distanceP = 10;

        private Motor left;
        private Motor right;
        private Ultrasonic sonic;

        Stopwatch watch;

        public MainPage()
        {
            Debug.WriteLine("Starting program");
            this.InitSystemComponent();

            Debug.WriteLine("Finished Initialization");

            // Fill in your TeamName KitID and keys.
            //IotHubLocalService LocalService = new IotHubLocalService("TeamName", "KitID", "keys");

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Loop;
            timer.Start();

          
        }

        public void InitSystemComponent()
        {
            this.InitializeComponent();
            var Task = SerialDispatcher.InitDispatcherTask();
            left = new Motor(Motor.MotorPort.M1);
            left.IsInverted = true;
            right = new Motor(Motor.MotorPort.M2);
            sonic = new Ultrasonic(Sensor.Port.Port1);
            watch = new Stopwatch();
            
        }





        private void Loop(object sender, object e)
        {
            watch.Reset();
            watch.Start();
            double distance = sonic.Read();
            //Debug.WriteLine("Distance: " + distance);
            Debug.WriteLine("Read Time: " + watch.ElapsedMilliseconds);
            int val = (int)(distanceP*(distance - goalDist));
            left.SetSpeed(val);
            Debug.WriteLine("Left Write: " + watch.ElapsedMilliseconds);
            right.SetSpeed(val);
            Debug.WriteLine("Total Loop time: " + watch.ElapsedMilliseconds);

            //System.Diagnostics.Debug.Write("Setting motor to: ");
            //if (dir)
            //{
            //    Debug.WriteLine("forward");
            //    left.SetSpeed(Motor.MAX_SPEED);
            //    right.SetSpeed(Motor.MAX_SPEED);
            //    //Motor.SetMotorCmd(Motor.ID.LeftMotor, Motor.Dir.Forward, 160);
            //    //Motor.SetMotorCmd(Motor.ID.RightMotor, Motor.Dir.Backward, 160);
            //}
            //else
            //{
            //    Debug.WriteLine("backward");
            //    //left.SetSpeed(Motor.MAX_NEG_SPEED);
            //    //right.SetSpeed(Motor.MAX_NEG_SPEED);
            //    //Motor.SetMotorCmd(Motor.ID.LeftMotor, Motor.Dir.Backward, 100);
            //    //Motor.SetMotorCmd(Motor.ID.RightMotor, Motor.Dir.Forward, 100);
            //}
            ////Motor.DoMotorCmd();
            //dir = !dir;
        }


    }

}