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
        public static int SelectedCUID;
        public static double SelectedInterval;

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
            
            //test
        }
        private void IntervalBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var tester = ClientStatus.ClientPipe[SelectedCUID];
            tester.SetCycle(SelectedInterval);
            tester.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            var tester = ClientStatus.ClientPipe[SelectedCUID];
            tester.Stop();
        }

        private void Border_GotFocus(object sender, RoutedEventArgs e)
        {
            var Grid = sender as Grid;
            TextBox interval = Grid.FindName("IntervalBox") as TextBox;            
            TextBlock CUID = Grid.FindName("UID") as TextBlock;
            SelectedInterval = double.Parse(interval.Text) * 1000;
            SelectedCUID = int.Parse(CUID.Text);
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
