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
using System.Globalization;

namespace WCFClient
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        const string ServerAddr = "net.tcp://127.0.0.1:2345";
        readonly int CUID;
        private bool currentStatus = false;
        public event PropertyChangedEventHandler PropertyChanged;
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
        Status status;


        public MainWindow()
        {
            InitializeComponent();
            ChannelFactory<Status> channelFactory = new ChannelFactory<Status>();
            channelFactory.Endpoint.Address = new EndpointAddress(ServerAddr);
            channelFactory.Endpoint.Binding = new NetTcpBinding();
            channelFactory.Endpoint.Contract.ContractType = typeof(Status);
            status = channelFactory.CreateChannel();
            CUID = status.AddClient();
            this.Title = string.Format("CUID::{0}", CUID);
            this.DataContext = this;
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

    public class ColorConverter : IValueConverter
    {
        public ColorConverter()
        {

        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(targetType == typeof(Style) && value is bool)
            {
                if((bool)value)
                {
                    return Application.Current.Resources["TrueST"] as Style;
                }
                else
                {
                    return Application.Current.Resources["FalseST"] as Style;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class MainWindowViewModel : BasePropertyChanged
    {
        private bool currentstatus = false;
        public bool CurrentStatus
        {
            get
            {
                return currentstatus;
            }
            set
            {
                currentstatus = value;
                OnPropertyChanged(nameof(CurrentStatus));
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
