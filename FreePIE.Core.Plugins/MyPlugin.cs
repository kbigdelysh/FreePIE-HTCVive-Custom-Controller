using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using FreePIE.Core.Contracts;

namespace FreePIE.Core.Plugins
{
    [GlobalType(Type = typeof(MyGlobal))]
    public class MyPlugin : ComDevicePlugin
    {
        private byte[] buffer;

        public override object CreateGlobal()
        {
            return new MyGlobal(this);
        }

        protected override int DefaultBaudRate
        {
            get { return 57600; }
        }

        protected override void Init(SerialPort serialPort)
        {
			// (optional) wait for IMU
            //Thread.Sleep(3000); 
            serialPort.ReadExisting();
			// (optional) Here you can send a signal to the arduino telling it to start sending data.
			// You might not need it though.
            //serialPort.WriteLine(START_COMMAND);         
            
            
            

            

           
           
            while (serialPort.BytesToRead < sync.Length && !serialPort.ReadLine().Contains(sync))
            {
                if (stopwatch.ElapsedMilliseconds > 100)
                    throw new Exception(string.Format("No hardware connected to port {0} with AHRS IMU protocol", port));
            }
            stopwatch.Stop();
            serialPort.ReadExisting(); //Sometimes there are garbage after the syncsignal
            buffer = new byte[4];
        }

        protected override void Read(SerialPort serialPort)
        {
            while (serialPort.BytesToRead >= 12)
            {
                var data = Data;
                data.Yaw = ReadFloat(serialPort, buffer);
                data.Pitch = ReadFloat(serialPort, buffer);
                data.Roll = ReadFloat(serialPort, buffer);

                Data = data;
                newData = true;
            }

            Thread.Sleep(1);
        }

        protected override string BaudRateHelpText
        {
            get { return "Baud rate, default on AHRS should be 57600"; }
        }
        
        public override string FriendlyName
        {
            get { return "AHRS IMU"; }
        }

        private float ReadFloat(SerialPort port, byte[] buffer)
        {
            port.Read(buffer, 0, buffer.Length);
            var value = BitConverter.ToSingle(buffer, 0);
            return value;
        }
    }

    [Global(Name = "My")]
    public class MyGlobal : DofGlobal<MyPlugin>
    {
        public MyGlobal(MyPlugin plugin) : base(plugin) { }
    }
}
