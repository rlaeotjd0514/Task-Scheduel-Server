using System.ServiceModel;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Runtime.Serialization;

//client
namespace WCFClient
{
    public class clientControl : ClientController
    {
        public static Timer timer;        
        public clientControl()
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Tick += new EventHandler(SendSystemInformation);
                timer.Enabled = false;
            }
        }
        public void SetCycle(double priod)
        {
            var stat = timer.Enabled;
            timer.Enabled = false;
            timer.Interval = (int)priod;
            MainWindow.CurrentInstance.title = string.Format("CUID::{0} Interval::{1}", MainWindow.CUID, MainWindow.CurrentInstance.Interval);
            timer.Enabled = stat;            
        }

        public void Start()
        {
            timer.Start();            
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void SendSystemInformation(object sender, EventArgs e)
        {
            new Task((Action)(() =>
            {
                ComputeUsageInfo CUI = new ComputeResourceAnalyzer().GetComputeInformation();
                MainWindow.status.SendSystemInformation(CUI.CUID, CUI.CpuUsage, CUI.MemoryUsage, CUI.Drives, CUI);
            })).Start();
            
        }       
    }    

    [ServiceContract]
    public interface Status
    {
        [OperationContract]
        void ToggleStatus(int ClientNumber, bool Status);
        [OperationContract]
        int AddClient();
        [OperationContract]
        void DelClient(int ClientNumber);
        [OperationContract]
        void SendSystemInformation(int CUID, double ComputeUsage, double MemoryUsage, DriveInfo[] Disks, ComputeUsageInfo ci);        
    }

    [DataContract(Namespace = "ComputeResource")]
    public class ComputeUsageInfo
    {
        [DataMember]
        public int CUID { get; set; }
        [DataMember]
        public double CpuUsage { get; set; }
        [DataMember]
        public double MemoryUsage { get; set; }
        public DriveInfo[] Drives { get; set; }
    }

    [ServiceContract]
    public interface ClientController
    {
        [OperationContract]
        void SetCycle(double priod);
        [OperationContract]
        void Start();
        [OperationContract]
        void Stop();
    }    
}
