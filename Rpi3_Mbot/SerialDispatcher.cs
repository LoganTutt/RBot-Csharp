using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace Rpi3_Mbot
{
    /// <summary>
    /// Dispatcher to handle all comunication between the Rasberry Pi and the mBot over serial
    /// </summary>
    public static class SerialDispatcher
    {

        /// <summary>
        /// Types of actions that can be taken
        /// </summary>
        private enum Action
        {
            GET = 1,
            SET = 2,
            RESET = 4,
            STOP = 5
        }

        /// <summary>
        /// Data type of the return type of the message
        /// </summary>
        private enum ReturnType
        {
            BYTE = 1,
            FLOAT = 2,
            SHORT = 3,
            STRING = 4,
            DOUBLE = 5,
        }

        /// <summary>
        /// Number of bytes for each possible return type
        /// </summary>
        private static Dictionary<ReturnType, int> byteSizes = new Dictionary<ReturnType, int>()
        {
            {ReturnType.BYTE, 1 },
            {ReturnType.FLOAT, 4 },
            {ReturnType.SHORT, 2 },
            {ReturnType.STRING, -1 },
            {ReturnType.DOUBLE, 8 }
        };


        private static DataWriter DataWriteObject = null;
        private static DataReader DataReadObject = null;
        private static SerialDevice device;

        private static Queue<byte[]> messages;

        /// <summary>
        /// Initialize the Dispatcher
        /// </summary>
        public static async Task InitDispatcherTask()
        {
            messages = new Queue<byte[]>();
            try
            {
                var infoCollection = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

                foreach (DeviceInformation info in infoCollection)
                {
                    device = await SerialDevice.FromIdAsync(info.Id);
                    if (device != null)
                    {
                        DataWriteObject = new DataWriter(device.OutputStream);
                        DataReadObject = new DataReader(device.InputStream);
                        device.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                        device.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                        device.BaudRate = 115200;//57600;
                        device.Parity = SerialParity.None;
                        device.StopBits = SerialStopBitCount.One;
                        device.DataBits = 8;
                        break;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Init Dispatcher failed" + ex.Message);
            }
           //await Listen().ConfigureAwait(false);
        }


        /// <summary>
        /// Write message to robot
        /// </summary>
        /// <param name="device">Device to write too</param>
        /// <param name="data">Bytes of data to write</param>
        public static void SendMsg(byte device, params byte[] data)
        {
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            byte[] msg = BuildMsg(Action.SET, device, data);
            WriteMsg(msg);
            //Debug.WriteLine("SendMsg Length: " + watch.ElapsedMilliseconds);

        }

        /// <summary>
        /// Poll a device that returns a double
        /// This method is blocking, and waits for a response from the device
        /// </summary>
        /// <param name="device">The device to be polled</param>
        /// <param name="port">The port the device is located on</param>
        /// <param name="slot">The slot of the device to be polled</param>
        /// <returns>Returns the double value from the device</returns>
        public static double GetDouble(byte device, byte port, byte slot = 0)
        {
            byte[] outMsg = BuildMsg(Action.GET, device, port, slot);
            WriteMsg(outMsg);
            byte[] returnMsg = ReadMsg();
            if (returnMsg.Count() == 8)
                return BitConverter.ToDouble(returnMsg, 0);
            if (returnMsg.Count() == 4)
                return BitConverter.ToSingle(returnMsg, 0);

            return 0;
        }

        /// <summary>
        /// Poll a device that returns a byte
        /// This method is blocking, and waits for a response from the device
        /// </summary>
        /// <param name="device">The device to be polled</param>
        /// <param name="port">The port the device is located on</param>
        /// <param name="slot">The slot of the device to be polled</param>
        /// <returns>Returns the byte value from the device</returns>
        public static byte GetByte(byte device, byte port, byte slot = 0)
        {
            byte[] outMsg = BuildMsg(Action.GET, device, port, slot);
            WriteMsg(outMsg);
            byte[] returnMsg = Task.Run(() => ReadMsg()).Result;
            if (returnMsg.Count() == 1)
                return returnMsg[0];
            return 0;
        }

        /// <summary>
        /// Poll a device that returns a short
        /// This method is blocking, and waits for a response from the device
        /// </summary>
        /// <param name="device">The device to be polled</param>
        /// <param name="port">The port the device is located on</param>
        /// <param name="slot">The slot of the device to be polled</param>
        /// <returns>Returns the short value from the device</returns>
        public static short GetShort(byte device, byte port, byte slot = 0)
        {
            byte[] outMsg = BuildMsg(Action.GET, device, port, slot);
            WriteMsg(outMsg);
            byte[] returnMsg = Task.Run(() => ReadMsg()).Result;
            if (returnMsg.Count() == 1)
                return BitConverter.ToInt16(returnMsg, 0);
            return 0;
        }

        /// <summary>
        /// Poll a device that returns a string
        /// This method is blocking, and waits for a response from the device
        /// </summary>
        /// <param name="device">The device to be polled</param>
        /// <param name="port">The port the device is located on</param>
        /// <param name="slot">The slot of the device to be polled</param>
        /// <returns>Returns the short value from the device</returns>
        public static string GetString(byte device, byte port, byte slot = 0)
        {
            byte[] outMsg = BuildMsg(Action.GET, device, port, slot);
            WriteMsg(outMsg);
            byte[] returnMsg = Task.Run(() => ReadMsg()).Result;
            if (returnMsg.Count() == 1)
                return Encoding.UTF8.GetString(returnMsg);
            return null;
        }


        #region helper methods
        /// <summary>
        /// Create a message to send
        /// </summary>
        /// <param name="action">Type of action to send</param>
        /// <param name="device">The device that the action will be performed on</param>
        /// <param name="data">Data to send to the device</param>
        /// <returns></returns>
        private static byte[] BuildMsg(Action action, byte device, params byte[] data)
        {
            /**************************************************
            ff    55     len idx action device data...
            0     1      2   3   4      5      6
            0xff  0x55   0x4 0x3 0x1    0x1    0x0
            ***************************************************/

            byte[] msgHeader = { 0xff, 0x55, 0x00, 0x00, (byte)action, device };
            msgHeader[2] = (byte)(3 + data.Count());
            byte[] msg = msgHeader.Concat(data).ToArray();
            return msg;
        }

        /// <summary>
        /// Send a message to the robot
        /// </summary>
        /// <param name="msg">Message to send</param>
        private static async void WriteMsg(byte[] msg)
        {
            //Debug.WriteLine("Sent msg: " + BitConverter.ToString(msg));
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (DataWriteObject != null)
            {
                DataWriteObject.WriteBytes(msg);
                //Debug.WriteLine("Wrote Bytes: " + watch.ElapsedMilliseconds);
                await DataWriteObject.StoreAsync();
                //Debug.WriteLine("stored: " + watch.ElapsedMilliseconds);
                await Task.Delay(1);
            }
            Debug.WriteLine("Finished write: " + BitConverter.ToString(msg));
        }

        /// <summary>
        /// Recieve a new message
        /// </summary>
        /// <returns>The byte array of the message</returns>
        private static byte[] ReadMsg()
        {
            /**************************************************
            ff    55     idx type data endline 
            0     1      2   3    4    n      
            0xff  0x55   0x4 0x3  0x1  0x0D 0x0A 
            ***************************************************/
            //Debug.WriteLine("Started Read");
            //while(messages.Count == 0)
            //{
            //    await Task.Delay(1);
            //}

            //return messages.Dequeue();

            byte[] data = null;
            if (DataReadObject != null)
            {
                DataReadObject.InputStreamOptions = InputStreamOptions.Partial;

                uint headerLength = 4;
                byte[] header = new byte[headerLength];
                while (header[0] != 0xff || header[1] != 0x55 || header[2] == 0x0D)
                {
                    //Get header (first 4 bytes)
                    //This runs the async methods synchronously as we want to wait for the readings


                    var task = DataReadObject.LoadAsync(headerLength).AsTask();
                    uint length = task.Result;
                    //Debug.WriteLine("header length:" + length);
                    DataReadObject.ReadBytes(header);
                    //Debug.Write("Recieved: " + BitConverter.ToString(header) + "-");
                }


                //Debug.Write("Recieved: " + BitConverter.ToString(header) + "-");


                ReturnType msgType = (ReturnType)header[3];
                int byteCount = byteSizes[msgType];

                //if the byte count is variable (ie. strings) get the next byte for the byte length
                if (byteCount == -1)
                {
                    var temp = DataReadObject.LoadAsync(1).AsTask().Result;
                    byteCount = DataReadObject.ReadByte();
                }

                //get the data, plus the two extra return characters
                var dataLength = DataReadObject.LoadAsync((uint)byteCount + 2).AsTask().Result;
                data = new byte[dataLength];
                DataReadObject.ReadBytes(data);
                //Debug.WriteLine(BitConverter.ToString(data));

                //Take only the actual reading
                data = data.Take(byteCount).ToArray();


            }

            return data;
        }

        private static async Task Listen()
        {
            while (true)
            {
                Debug.WriteLine("listening...");
                await Task.Delay(50);
                //await ReadAsync();
            }
        }

        private static async Task ReadAsync()
        {
            Debug.WriteLine("ReadAsync");
            Task<UInt32> loadAsyncTask;

            uint headerLength = 4;

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            DataReadObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            loadAsyncTask = DataReadObject.LoadAsync(headerLength).AsTask();

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;
            Debug.WriteLine("GotMessage "+ bytesRead);
            if (bytesRead == headerLength)
            {
                
                byte[] header = new byte[headerLength];
                DataReadObject.ReadBytes(header);

                if (header[0] != 0xFF || header[1] != 0x55 || header[2] == 0x0D)
                    return;
                //Debug.Write("Recieved: " + BitConverter.ToString(header) + "-");


                ReturnType msgType = (ReturnType)header[3];
                int byteCount = byteSizes[msgType];

                //if the byte count is variable (ie. strings) get the next byte for the byte length
                if (byteCount == -1)
                {
                    var temp = await DataReadObject.LoadAsync(1).AsTask();
                    byteCount = DataReadObject.ReadByte();
                }

                //get the data, plus the two extra return characters
                var dataLength = await DataReadObject.LoadAsync((uint)byteCount + 2).AsTask();
                byte[] data = new byte[dataLength];
                DataReadObject.ReadBytes(data);
                //Debug.WriteLine(BitConverter.ToString(data));

                //Take only the actual reading
                data = data.Take(byteCount).ToArray();

                messages.Enqueue(data);
            }
        }
        #endregion
    }
}
