using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using FreePIE.Core.Contracts;
using FreePIE.Core.Plugins.MyArduino;

namespace FreePIE.Core.Plugins
{
    [GlobalType(Type = typeof(MyArduinoGlobal))]
    public class MyArduinoPlugin : ComDevicePlugin
    {
        private SerialPort _serialPort;
        private bool _looping;
        private Thread _serialPortReadingThread;
        private string _receivedSingleMessage;
        private Vector3 _latestPosition;
        private DofData _latestData;
        private const string COMMA = ",";

        public override object CreateGlobal()
        {
            return new MyArduinoGlobal(this);
        }

        protected override int DefaultBaudRate
        {
            get { return 921600; }
        }

        protected override void Init(SerialPort serialPort)
        {
            // Initialize the member variable.
            _serialPort = serialPort;

            // (optional) wait for IMU
            //Thread.Sleep(3000); 

            // Set the timeout i.e. if 2000 ms passed and no data is received, throw an exception.
            _serialPort.ReadTimeout = 2000;

            // (optional) Here you can send a signal to the arduino telling it to start sending data.
            // You might not need it though.
            //serialPort.WriteLine(START_COMMAND);     
            _looping = true;
            _serialPortReadingThread = new Thread(SerialPortReadingThread);
            _serialPortReadingThread.Start();
        }
        /// <summary>
        /// Reads the serial port continuously.
        /// </summary>
        private void SerialPortReadingThread()
        {
            while (_looping)
            {                
                try
                {
                    _receivedSingleMessage = _serialPort.ReadLine();
                }
                catch (Exception e)
                {
                    if (e is TimeoutException)
                    {
                        Console.WriteLine("TimeoutException inside MyArduinoPlugin.cs");
                        return;
                    }
                    else
                        throw;

                }
                ParseData();
            }
        }
        /// <summary>
        /// Extract positional data from the message received from the arduino.
        /// </summary>
        private void ParseData()
        {
            // Extract position and oriantation.
            // sample received message 
            // pos: 1.00000,0.00000,1.00000 rot: 0.99985,-0.00356,-0.00462,-0.01626 
            // which follows the format --> pos: x, y, z rot: pitch, roll, yaw
            var stringCollection = _receivedSingleMessage.Replace("rot:", COMMA)
                                             .Replace("pos:", COMMA)                                             
                                             .Split(',');
            // We omit stringCollection[0] because it does not contain positional data and is empty.
            float posX = Convert.ToSingle(stringCollection[1]);
            float posY = Convert.ToSingle(stringCollection[2]);
            float posZ = Convert.ToSingle(stringCollection[3]);

            float pitch = Convert.ToSingle(stringCollection[4]);
            float roll  = Convert.ToSingle(stringCollection[5]);
            float yaw   = Convert.ToSingle(stringCollection[6]);

            _latestPosition = new MyArduino.Vector3(posX, posY, posZ);
            _latestData = new DofData();
        }

        protected override void Read(SerialPort serialPort)
        {
            Position = _latestPosition;
            Data = _latestData;
            Thread.Sleep(1);
        }

        protected override string BaudRateHelpText
        {
            get { return "Baud rate, default on MyArduino should be 921600"; }
        }
        
        public override string FriendlyName
        {
            get { return "MyArduino Plugin"; }
        }

        //private float ReadFloat(SerialPort port, byte[] buffer)
        //{
        //    port.Read(buffer, 0, buffer.Length);
        //    var value = BitConverter.ToSingle(buffer, 0);
        //    return value;
        //}

        public override void Stop()
        {
            // (optional) Stop arduino.
            //_serialPort.WriteLine(STOP_COMMAND);
            //Thread.Sleep(200);
            
            // Stop the reading thread and wait for it.
            _looping = false; 
            Thread.Sleep(100);

            _serialPortReadingThread.Join();
            _serialPortReadingThread.Abort();
            base.Stop();
        }
    }

    [Global(Name = "myArduino")]
    public class MyArduinoGlobal : DofGlobal<MyArduinoPlugin>
    {
        public MyArduinoGlobal(MyArduinoPlugin plugin) : base(plugin) { }
    }
}
