using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ServiceModel;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
//server
namespace WCF
{
    public class ClientStatus : Status
    {
        const int MaxConnectionLimit = 256;
        public static Dictionary<int, bool> ClientUsage;
        public static Dictionary<int, ChannelFactory<ClientController>> ClientPipe;
        public static Dictionary<int, ComputeUsageInfo> ClientComputeUsage;
        private static int max = -1;
        public ClientStatus()
        {
            if (ClientUsage == null)
            {
                ClientUsage = new Dictionary<int, bool>();
            }
            if(ClientPipe == null)
            {
                ClientPipe = new Dictionary<int, ChannelFactory<ClientController>>();
            }
            if(ClientComputeUsage == null)
            {
                ClientComputeUsage = new Dictionary<int, ComputeUsageInfo>();
            }
        }

        void Status.ToggleStatus(int ClientNumber, bool Status)
        {
            if (ClientNumber >= MaxConnectionLimit)
            {
                throw new Exception("Fatal::Requested Client's Index is Not Valid!!!");
            }
            ClientUsage[ClientNumber] = Status;
            MainWindow.CurrentViewModelObject.CSD[ClientNumber].ClientStatus = Status;            
        }

        int Status.AddClient()
        {
            int cur = -1;
            for (int i = 0; i <= max + 1; i++)
            {
                if (!ClientUsage.ContainsKey(i))
                {
                    cur = i;
                    break;
                }
            }
            if (cur == -1) throw new Exception("Fatal::Connection Limitation Reached. Can not Connect More Clients");
            if (cur > max) max = cur;
            MainWindow.CurrentViewModelObject.CSD.Add(new MainWindowViewModel.ClientStatusData() { ClientNumber = cur, ClientStatus = false });            
            ClientUsage.Add(cur, false);

            OperationContext context = OperationContext.Current;
            MessageProperties properties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string address = string.Format("net.tcp://{0}:{1}", endpoint.Address, (6600).ToString());
            Uri remoteUri = new Uri(address);
            ChannelFactory<ClientController> channelFactory = new ChannelFactory<ClientController>();
            channelFactory.Endpoint.Address = new EndpointAddress(remoteUri);
            channelFactory.Endpoint.Binding = new NetTcpBinding();
            channelFactory.Endpoint.Contract.ContractType = typeof(ClientController);
            ClientPipe.Add(cur, channelFactory);            
            return cur;
        }
        void Status.DelClient(int ClientNumber)
        {
            ClientUsage.Remove(ClientNumber);
            var rindex = MainWindow.CurrentViewModelObject.CSD.Where(x => x.ClientNumber == ClientNumber);
            MainWindow.CurrentViewModelObject.CSD.Remove(rindex.ToArray()[0]);            
            if (ClientNumber == max)
            {
                for (int i = max - 1; i >= 0; i--)
                {
                    if (ClientUsage.ContainsKey(i))
                    {
                        max = i;
                        break;
                    }
                }
            }

            var index = MainWindow.CurrentViewModelObject.CUI.Where(x => x.CUID == ClientNumber);
            MainWindow.CurrentViewModelObject.CUI.Remove(index.ToArray()[0]);
        }

        void Status.SendSystemInformation(int CUID, double ComputeUsage, double MemoryUsage, DriveInfo[] Disks, ComputeUsageInfo ci)
        {
            ComputeUsageInfo si = new ComputeUsageInfo();
            si.CUID = CUID;
            si.CpuUsage = ComputeUsage;
            si.MemoryUsage = MemoryUsage;
            si.Drives = Disks;                        
            if(ClientComputeUsage.ContainsKey(CUID))
            {
                ClientComputeUsage[CUID] = si;
                MainWindow.CurrentViewModelObject.CUI[CUID] = si;
            }
            else
            {
                ClientComputeUsage.Add(CUID, si);
                MainWindow.CurrentViewModelObject.CUI.Add(si);
            }
            Debug.WriteLine("We Received Data From Client!! CPU Usage::" + si.CpuUsage + "Mem Usage::" + si.MemoryUsage);
        }        
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
