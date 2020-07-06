using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace WCF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        ServiceHost host;
        public static MainWindowViewModel CurrentViewModelObject = new MainWindowViewModel();
        
        public MainWindow()
        {
            InitializeComponent();
            string address = "net.tcp://127.0.0.1:2345";
            NetTcpBinding ServerBind = new NetTcpBinding();
            host = new ServiceHost(typeof(ClientStatus));
            host.AddServiceEndpoint(typeof(Status), ServerBind, address);
            host.Open();            
            this.DataContext = CurrentViewModelObject;
            ClientListBox.ItemsSource = CurrentViewModelObject.CSD;
        }        

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ClientListBox.Items.Refresh();
        }
    }

    public class MainWindowViewModel : BasePropertyChanged
    {        
        public MainWindowViewModel()
        {            
            
        }        

        private static ObservableCollection<ClientStatusData> csd;
        public ObservableCollection<ClientStatusData> CSD
        {
            get
            {
                if(csd == null)
                {
                    csd = new ObservableCollection<ClientStatusData>();
                }
                return csd;
            }
            set
            {
                csd = value;
                OnPropertyChanged(nameof(CSD));
            }
        }
        
        public class ClientStatusData : BasePropertyChanged
        {
            private int clientno;                     
            public int ClientNumber
            {
                get
                {
                    return clientno;
                }
                set
                {
                    clientno = value;
                    OnPropertyChanged(nameof(ClientNumber));
                }
            }
            private bool clientstat;
            public bool ClientStatus
            {
                get
                {
                    return clientstat;
                }
                set
                {
                    clientstat = value;
                    OnPropertyChanged(nameof(ClientStatus));
                }
            }            
            private static List<ClientStatusData> datalist;            
            public static List<ClientStatusData> getStatusList()
            {
                if (datalist == null)
                {
                    return datalist = new List<ClientStatusData>();
                }
                else return datalist;
            }
        }
    }

    public class ClientStatus : Status
    {        
        public static Dictionary<int, bool> ClientUsage;
        private static int max = -1;        
        public ClientStatus()
        { 
            if(ClientUsage == null)
            {
                ClientUsage = new Dictionary<int, bool>();
            }
        }

        void Status.ToggleStatus(int ClientNumber, bool Status)
        {
            if(ClientNumber >= 256)
            {
                throw new Exception("Fatal::Requested Client Index is Not Valid!!!");
            }
            ClientUsage[ClientNumber] = Status;
            MainWindow.CurrentViewModelObject.CSD[ClientNumber].ClientStatus = Status;            
        }

        int Status.AddClient()
        {
            int cur = -1;
            for(int i = 0; i <= max + 1;i++)
            {
                if(!ClientUsage.ContainsKey(i))
                {
                    cur = i;
                    break;
                }
            }
            if (cur == -1) throw new Exception("Connection Limitation Reached. Can not Connect More Clients");
            if (cur > max) max = cur;
            MainWindow.CurrentViewModelObject.CSD.Add(new MainWindowViewModel.ClientStatusData() { ClientNumber = cur, ClientStatus = false });
            //ListChanged(null, EventArgs.Empty);
            ClientUsage.Add(cur, false);
            return cur;
        }
        void Status.DelClient(int ClientNumber)
        {
            ClientUsage.Remove(ClientNumber);
            var rindex = MainWindow.CurrentViewModelObject.CSD.Where(x => x.ClientNumber == ClientNumber);
            MainWindow.CurrentViewModelObject.CSD.Remove(rindex.ToArray()[0]);
            if(ClientNumber == max)
            {
                for(int i=max - 1;i>=0;i--)
                {
                    if(ClientUsage.ContainsKey(i))
                    {
                        max = i;
                        break;
                    }
                }
            }
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
    }

    public class BasePropertyChanged : INotifyPropertyChanged
    {
        #region 속성 

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        #endregion
    }
}
