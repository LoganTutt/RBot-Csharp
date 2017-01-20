using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.Devices.SerialCommunication;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Microsoft.Azure.Devices.Client;
using System.Threading;
using Newtonsoft.Json;
using Windows.Foundation;
using System.Net.NetworkInformation;

/// <summary>
/// The software for imagine cup Version: V1.2
/// </summary>


namespace Rpi3_Mbot
{

    public partial class MainPage : Page
    {
        
    }
    /*
    public class Motor
    {
        public static bool MotorToIothubFlag = false;
        public static int InitMotorFlag = 0;
        public static DataWriter DataWriteObject = null;
        private static SerialDevice device;
        private static int MaxMotorSpeed = 240;
        private static int MinMotorSpeed = 100;
        static byte[] WData = new byte[] { 0xff, 0x55, 0x07, 0x00, 0x02, 0x05, 0x00, 0x00, 0x00, 0x00 };

        public static int InitMotor()
        {
            int flag = 1;
            InitMotordev();
            return flag;
        }
        public static async void InitMotordev()
        {
            try
            {
                var infoCollection = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());
                InitMotorFlag = 0;

                foreach (DeviceInformation info in infoCollection)
                {
                    device = await SerialDevice.FromIdAsync(info.Id);
                    if (device != null)
                    {
                        DataWriteObject = new DataWriter(device.OutputStream);
                        device.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                        device.BaudRate = 115200;//57600;
                        device.Parity = SerialParity.None;
                        device.StopBits = SerialStopBitCount.One;
                        device.DataBits = 8;
                        break;
                    }
                }

                InitMotorFlag = 1;
            }
           catch (Exception ex)
            {
                Debug.WriteLine("Init Motor failed"+ex.Message);
            }
        }

        private static int JudgeSpeed(int speed)
        {
            if (speed > MaxMotorSpeed)
            {
                speed = MaxMotorSpeed;
            }
            if ((speed < MinMotorSpeed)&&(speed != 0))
            {
                speed = MinMotorSpeed;
            }
            return speed;
        }

        public enum ID {LeftMotor = 0, RightMotor }
        public enum Dir {Forward = 0, Backward}

        public static void SetMotorCmd(ID motorid, Dir direction, int speed)
        {   
           
            //Calculate the car running distance
            //if(IotHubLocalService.EnableTransfer==1)
            //{
            //    CarRunStatus(motorid, direction, speed);
            //}
            
            if ((int)motorid == (int)direction)
            {
                speed *= -1;
            }

            WData[6 + (int)motorid * 2] = (byte)(speed);
            WData[7 + (int)motorid * 2] = (byte)(speed >> 8);
            
        }
        public static int DoMotorFlag = 0;
        
        public static async void DoMotorCmd()
        {
            //Do the motor command
           
            DoMotorFlag = 1;
           
            if (DataWriteObject != null)
            {
              DataWriteObject.WriteBytes(WData);
              await DataWriteObject.StoreAsync();
              await Task.Delay(10);

            }
                
             //Clear the motor command
             ResetCmd();
             DoMotorFlag = 0;

        }

        private static void ResetCmd()
        {
            WData[6] = 0x00;
            WData[7] = 0x00;
            WData[8] = 0x00;
            WData[9] = 0x00;
        }

        //public static int RunTime = 0;
        //public static int RunDistance = 0;
        //private static DateTime FirstRunTime;
        //private static DateTime LastRunTime;
        //private static int RunSpeed = 0;
        //private static int Div = 8;
        //private static int FirstSpeed = 0;
        //private static int LastSpeed = 0;
        //private enum WDir { Forward = 0, Backward, Stop }
        //private static WDir LeftDir = WDir.Stop;
        //private static WDir RightDir = WDir.Stop;

        //private static void CarRunStatus(ID motorid, Dir direction, int speed)
        //{
        //    if ((motorid == ID.LeftMotor) && (direction == Dir.Forward) && (speed != 0))
        //        LeftDir = WDir.Forward;
        //    if ((motorid == ID.LeftMotor) && (direction == Dir.Backward) && (speed != 0))
        //        LeftDir = WDir.Backward;
        //    if ((motorid == ID.LeftMotor)&& (speed == 0))
        //        LeftDir = WDir.Stop;
        //    if ((motorid == ID.RightMotor) && (direction == Dir.Forward) && (speed != 0))
        //        RightDir = WDir.Forward;
        //    if ((motorid == ID.RightMotor) && (direction == Dir.Backward) && (speed != 0))
        //        RightDir = WDir.Backward;
        //    if ((motorid == ID.RightMotor) && (speed == 0))
        //        RightDir = WDir.Stop;
            
        //    if (LeftDir == RightDir)
        //        CarRuntime(speed);
        //}

        //private static void CarRuntime(int speed)
        //{
        //    FirstSpeed = speed;

        //    if((FirstSpeed != 0)&&(LastSpeed == 0))
        //    {
        //        FirstRunTime = DateTime.Now;
        //        RunSpeed = FirstSpeed;
        //    }
        //    if((FirstSpeed == 0) && (LastSpeed != 0))
        //    {
        //        LastRunTime = DateTime.Now;
        //        TimeSpan ts = LastRunTime - FirstRunTime;
        //        RunTime += ts.Seconds;
        //        RunDistance += RunSpeed / Div * ts.Seconds;

        //        Debug.WriteLine("Car--RunTime ={0}s,RunDistance={1}cm\n", RunTime, RunDistance);

        //    }

        //    LastSpeed = FirstSpeed;
        //}
    }
    */

    //public class Shoot
    //{
    //    public static bool ShootToIothubFlag = false;
    //    public static int KickballCmdFlag = 0;
    //    private static int MaxKickAngle = 60;
    //    private static int MinKickAngle = 5;
    //    public static async void KickballCmd()
    //    {
    //        int angle = 0;
    //        byte[] BallData = new byte[] { 0xff, 0x55, 0x06, 0x00, 0x02, 0x0b, 0x01, 0x01, 0x00 };

    //        if ((Motor.DoMotorFlag == 0) && (KickballCmdFlag == 0))
    //        {
    //            KickballCmdFlag = 1;

    //            //Set Maxangle for kick ball
    //            angle = MaxKickAngle;
    //            BallData[8] = (byte)angle;
    //            if (Motor.DataWriteObject != null)
    //            {
    //                Motor.DataWriteObject.WriteBytes(BallData);
    //                await Motor.DataWriteObject.StoreAsync();
    //                await Task.Delay(140);
    //            }

    //            //Set Minangle for kick ball
    //            angle = MinKickAngle;
    //            BallData[8] = (byte)angle;
    //            if (Motor.DataWriteObject != null)
    //            {
    //                Motor.DataWriteObject.WriteBytes(BallData);
    //                await Motor.DataWriteObject.StoreAsync();
    //                await Task.Delay(100);
    //            }
    //            KickballCmdFlag = 0;
    //        }
    //    }
    //}
    


    //public class IotHubLocalService
    //{
       
    //    private string BoardId = "";
    //    private string IotHubUri = "PEImagineCUPIOTHub.azure-devices.net";
    //    private string DeviceKey = "";

    //    private string TeamName = "";
    //    private DeviceClient deviceClient;
    //    private bool SendMotorStaF = false;
    //    private bool SendXboxStaF = false;
    //    private bool SendShootStaF = false;
    //    private bool LoginF = false;
    //    public static int EnableTransfer = 0;
    //    private bool IotHubConnectFlag = false;
    //    private bool NowNetSta = false;

    //    private DateTime SendRunDataDelay;

    //    static DateTime LastTime;
    //    static DateTime NowTime;

    //    static void InitTimer()
    //    {
    //        LastTime = DateTime.Now;

    //    }
    //    static bool CheckTimerStrike(double time_value)
    //    {
    //        TimeSpan ts = DateTime.Now - LastTime;
    //        if (ts.TotalSeconds > time_value)
    //        {
    //            LastTime = DateTime.Now;
    //            return true;
    //        }
    //        else
    //            return false;
    //    }

    //    static bool CheckTimerStrikeSecond(DateTime time_moment, double time_value)
    //    {
    //        TimeSpan ts = DateTime.Now - time_moment;
    //        if (ts.TotalSeconds > time_value)
    //        {
    //            return true;
    //        }
    //        else
    //            return false;
    //    }


    //    public IotHubLocalService(string name, string id, string key)
    //    {
    //        BoardId = id;
    //        TeamName = name;
    //        DeviceKey = key;

    //        if (TeamName.Length == 0)
    //        {
    //            Debug.WriteLine(">>>>----Please fill in your Teamname\n");
    //            return;
    //        }
    //        if (BoardId.Length == 0)
    //        {
    //            Debug.WriteLine(">>>>----Please fill in your CarId\n");
    //            return;
    //        }
    //        if (DeviceKey.Length == 0)
    //        {
    //            Debug.WriteLine(">>>>----Please fill in your DeviceKey\n");
    //            return;
    //        }

    //        IotHubLocalService.InitTimer();
    //        SendRunDataDelay = DateTime.Now;

    //        MainLoop();
    //    }

    //    private async Task LogInIotHub()
    //    {
    //        TypeJson command_json = new TypeJson();

    //        if (NowNetSta == false)
    //            return;
    //        if (IotHubConnectFlag == false)
    //            return;

    //        if (TeamName == null)
    //        {
    //            Debug.WriteLine("Please don't let your teamname void when you call methods 'new IotHubLocalService(xx,xx,xx)'");
    //            await Task.Delay(TimeSpan.FromSeconds(4));
    //        }
    //        else
    //        {

    //            if (LoginF == false)
    //            {
    //                try
    //                {
    //                    Debug.WriteLine("Start to log in\r\n");

    //                    LoginInJson login_msg = new LoginInJson();
    //                    login_msg.deviceId = BoardId;
    //                    login_msg.name = TeamName;

    //                    var messageString = JsonConvert.SerializeObject(login_msg);
    //                    command_json.Type = 1;
    //                    command_json.Command = messageString;
    //                    messageString = JsonConvert.SerializeObject(command_json);
    //                    var message = new Message(Encoding.ASCII.GetBytes(messageString));
    //                    IAsyncAction Action = deviceClient.SendEventAsync(message);
    //                    await Action;
    //                    if (Action.Status == AsyncStatus.Completed)
    //                    {
    //                        Debug.WriteLine("Log in Success");
    //                        LoginF = true;
    //                    }
    //                    else
    //                    {
    //                        Debug.WriteLine("Log in Fail");
    //                        await Task.Delay(TimeSpan.FromSeconds(10));
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    Debug.WriteLine("Login iot hub fail,please check your teamname and internet" + ex.Message);
    //                    await Task.Delay(TimeSpan.FromSeconds(10));
    //                }

    //            }
    //        }
    //    }
    //    private async Task MainLoop()
    //    {
    //        while (true)
    //        {

    //            if (NetworkInterface.GetIsNetworkAvailable())
    //            {
    //                NowNetSta = true;
    //            }
    //            else
    //            {
    //                NowNetSta = false;
    //                await Task.Delay(TimeSpan.FromSeconds(2));
    //            }


    //            if (NowNetSta == true)
    //            {
    //                if (IotHubConnectFlag == false)
    //                {
    //                    try
    //                    {
    //                        Debug.WriteLine("Start to Connect \r\n");
    //                        deviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(BoardId, DeviceKey));

    //                        if (deviceClient != null)
    //                        {
    //                            Debug.WriteLine("Connect Success\r\n");
    //                            IotHubConnectFlag = true;
    //                        }
    //                        else
    //                        {
    //                            await Task.Delay(TimeSpan.FromSeconds(10));
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        Debug.WriteLine("Create iot client fail,please check your id and internet" + ex.Message);
    //                        await Task.Delay(TimeSpan.FromSeconds(10));
    //                    }
    //                }
    //                else
    //                {
    //                    await LogInIotHub();
    //                    await SendModelDataToIot();
    //                    await ReciveCommandFromIot();
    //                }
    //            }
    //        }
    //    }
    //    private async Task SendModelDataToIot()
    //    {
    //        TypeJson command_json = new TypeJson();

    //        if (NowNetSta == false)
    //            return;
    //        if (IotHubConnectFlag == false)
    //            return;

    //        if (LoginF == true)
    //        {
               
    //            if (EnableTransfer == 1)
    //            {
    //                if (CheckTimerStrikeSecond(SendRunDataDelay, 5))
    //                {
    //                    SendRunDataDelay = DateTime.Now;

    //                    try
    //                    {

    //                        SendMotorRunStatusJson car_run_msg = new SendMotorRunStatusJson();
    //                        car_run_msg.deviceId = BoardId;
    //                        car_run_msg.RunTime = Motor.RunTime;
    //                        car_run_msg.RunDistance = Motor.RunDistance;

    //                        var messageString = JsonConvert.SerializeObject(car_run_msg);
    //                        command_json.Type = 6;
    //                        command_json.Command = messageString;
    //                        messageString = JsonConvert.SerializeObject(command_json);
    //                        var message = new Message(Encoding.ASCII.GetBytes(messageString));
    //                        await deviceClient.SendEventAsync(message);
    //                        Debug.WriteLine("Send car run data ={1}", 1, messageString);

    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        Debug.WriteLine("Send Run Data error" + ex.Message); 
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            await Task.Delay(TimeSpan.FromMilliseconds(30));
    //        }
    //    }

    //    private int ParsingCommand(int type, string command_str)
    //    {
    //        switch (type)
    //        {
    //            case 7://Control Motor
    //                var enable_transfer = JsonConvert.DeserializeObject<EnableTransferJson>(command_str);
    //                Debug.WriteLine("Id ={0} Transfer ={1}", enable_transfer.deviceId, enable_transfer.EnableTransfer);
    //                if (((enable_transfer.EnableTransfer == 1) || (enable_transfer.EnableTransfer == 0)) && (enable_transfer.deviceId == BoardId))
    //                {
    //                    Motor.RunDistance = 0;
    //                    Motor.RunTime = 0;
    //                    EnableTransfer = enable_transfer.EnableTransfer;
    //                }
    //                else
    //                {
    //                    Debug.WriteLine("Msg Wrong");
    //                }
    //                break;
    //            default:

    //                break;
    //        }
    //        return 1;
    //    }

    //    private async Task ReciveCommandFromIot()
    //    {
    //        Message receivedMessage;
    //        string messageData;

    //        if (NowNetSta == false)
    //            return;
    //        if (IotHubConnectFlag == false)
    //            return;

    //        if (IotHubConnectFlag == true)
    //        {
    //            try
    //            {
    //                receivedMessage = await deviceClient.ReceiveAsync();

    //                if (receivedMessage != null)
    //                {
    //                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
    //                    Debug.WriteLine("Msg={1}", 1, messageData);
    //                    var result = JsonConvert.DeserializeObject<TypeJson>(messageData);
    //                    if (result.Type != 0)
    //                    {
    //                        ParsingCommand(result.Type, result.Command);
    //                    }
    //                    //Debug.WriteLine("Speed ={0},Direction={1}", result.Speed, result.Direction);
    //                    await deviceClient.CompleteAsync(receivedMessage);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Debug.WriteLine("Receive Iot Hub Error" + ex.Message);
    //                await Task.Delay(TimeSpan.FromSeconds(10));
    //            }

    //        }

    //    }
    //}
}
