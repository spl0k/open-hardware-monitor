using System;
using System.Collections.Generic;
using System.Management;

namespace OpenHardwareMonitor.Hardware.NIC
{
    internal class NICGroup : IGroup
    {
        private readonly List<GenericNIC> hardware = new List<GenericNIC>();

        public NICGroup(ISettings settings)
        {
            int p = (int)Environment.OSVersion.Platform;
            if (p == 4 || p == 128) return;

            try
            {
                ManagementObjectCollection collection;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT Name, Index FROM Win32_NetworkAdapter WHERE PhysicalAdapter = 1"))
                    collection = searcher.Get();

                foreach (ManagementObject mo in collection)
                    hardware.Add(new GenericNIC((string)mo["Name"], (int)(uint)mo["Index"], settings));
            }
            catch { }
        }

        public IHardware[] Hardware
        {
            get { return hardware.ToArray(); }
        }

        public string GetReport()
        {
            return "NICGroup: report not implemented";
        }

        public void Close()
        {
            foreach (GenericNIC nic in hardware)
                nic.Close();
        }
    }
}
