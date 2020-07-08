using System.Windows;
using System.ServiceModel;
using System.ComponentModel;
using System.Windows.Controls;

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
            //ComputeList.ItemsSource = CurrentViewModelObject.CUI;
        }        

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ClientListBox.Items.Refresh();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            //test
            var tester = ClientStatus.ClientPipe[0].CreateChannel();
            tester.SetCycle(1000.0);
            tester.Start();
            //test
        }
        private void IntervalBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var parent = (sender as Button).Parent;            
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

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
