using System.Windows;
using System.ServiceModel;
using System.ComponentModel;

namespace WCFClient
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static MainWindow CurrentInstance;
        private string _title;
        public string title
        {
            get
            {
                return string.Format("CUID::{0} Interval::{1}", CUID, Interval);
            }
            set
            {
                _title = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Interval)));
            }
        }        
        public static int CUID;
        private static double interval = 1.0f;
        public double Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Interval)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private bool currentStatus = false;
        public bool CurrentStatus
        {
            get
            {
                return currentStatus;
            }
            set
            {
                currentStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentStatus)));
            }
        }

        const string ServerAddr = "net.tcp://127.0.0.1:2345";
        public static Status status;
        ServiceHost host;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            CurrentInstance = this;
            
            string address = "net.tcp://127.0.0.1:" + (6600).ToString();
            NetTcpBinding ServerBind = new NetTcpBinding();
            host = new ServiceHost(typeof(clientControl));
            host.AddServiceEndpoint(typeof(ClientController), ServerBind, address);
            host.Open();        

            ChannelFactory<Status> channelFactory = new ChannelFactory<Status>();
            channelFactory.Endpoint.Address = new EndpointAddress(ServerAddr);
            channelFactory.Endpoint.Binding = new NetTcpBinding();
            channelFactory.Endpoint.Contract.ContractType = typeof(Status);
            status = channelFactory.CreateChannel();
            CUID = status.AddClient();

        }
        
        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            status.ToggleStatus(CUID, StatusButton.IsChecked.Value);
            CurrentStatus = StatusButton.IsChecked.Value;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            status.DelClient(CUID);
        }
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
