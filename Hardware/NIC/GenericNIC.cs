using System;
using System.Globalization;
using System.Management;

namespace OpenHardwareMonitor.Hardware.NIC
{
    internal class GenericNIC : Hardware
    {
        private readonly string wmiName;
        private readonly Sensor[] sensors = new Sensor[3];

        public GenericNIC(string name, int index, ISettings settings)
            : base(name, new Identifier("nic", index.ToString(CultureInfo.InvariantCulture)), settings)
        {
            wmiName = name;

            sensors[0] = new Sensor("Bytes received per sec", 0, SensorType.DataRate, this, settings);
            sensors[1] = new Sensor("Bytes sent per sec", 1, SensorType.DataRate, this, settings);
            sensors[2] = new Sensor("Total bytes per sec", 2, SensorType.DataRate, this, settings);

            foreach (Sensor sensor in sensors)
                ActivateSensor(sensor);
        }
        
        public override HardwareType HardwareType
        {
            get { return HardwareType.NIC; }
        }

        public override void Update()
        {
            try
            {
                ManagementObjectCollection collection;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2",
                    string.Format("SELECT BytesReceivedPersec, BytesSentPersec, BytesTotalPersec FROM Win32_PerfFormattedData_Tcpip_NetworkInterface WHERE Name = '{0}'", wmiName)))
                    collection = searcher.Get();

                ManagementObjectCollection.ManagementObjectEnumerator enumerator = collection.GetEnumerator();
                enumerator.MoveNext();
                ManagementBaseObject mo = enumerator.Current;

                sensors[0].Value = (float)(ulong)mo["BytesReceivedPersec"];
                sensors[1].Value = (float)(ulong)mo["BytesSentPersec"];
                sensors[2].Value = (float)(ulong)mo["BytesTotalPersec"];

                if (enumerator.MoveNext())
                    throw new Exception();
            }
            catch { }
        }
    }
}
