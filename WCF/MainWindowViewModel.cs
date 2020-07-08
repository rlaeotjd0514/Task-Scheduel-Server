using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ServiceModel;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace WCF
{
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
                if (csd == null)
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
        }

        private static ObservableCollection<ComputeUsageInfo> cui;
        public ObservableCollection<ComputeUsageInfo> CUI
        {
            get
            {
                if(cui == null)
                {
                    cui = new ObservableCollection<ComputeUsageInfo>();
                }
                return cui;
            }
            set
            {
                cui = value;
                OnPropertyChanged(nameof(CUI));
            }
        }        
    }
}
