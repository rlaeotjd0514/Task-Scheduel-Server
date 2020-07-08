using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Windows;
using System.IO;

namespace WCFClient
{
    class ComputeResourceAnalyzer
    {        
        private ManagementObjectSearcher MOS;

        public ComputeUsageInfo GetComputeInformation()
        {
            ComputeUsageInfo result = new ComputeUsageInfo();            
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
            var cpuUsage = searcher.Get()
                .Cast<ManagementObject>()
                .Where(x => x["Name"] as string == "_Total").Select(x => x["PercentProcessorTime"]).ToArray();
            MOS = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            var memUsage = MOS.Get()
                .Cast<ManagementObject>()
                .Select(mo => new MemoryPercentage()
                {
                    TotalSize = ulong.Parse(mo["TotalVisibleMemorySize"].ToString()),
                    FreeSize = ulong.Parse(mo["FreePhysicalMemory"].ToString()),
                }).FirstOrDefault().UsedPercent;            
            result.CpuUsage = double.Parse(cpuUsage[0].ToString());
            result.MemoryUsage = memUsage;
            result.Drives = DriveInfo.GetDrives();
            return result;
        }

        class MemoryPercentage
        {
            public ulong TotalSize { get; set; }
            public ulong FreeSize { get; set; }
            public ulong UsedSize => TotalSize - FreeSize;
            public double UsedPercent => ((double)UsedSize / (double)TotalSize) * 100.0f;
        }        
    }
}
