using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ASK.Model.NetsList
{
    //TODO: zrobic enumy do rzeczy, gdzie sie da
    public class Profile : INotifyPropertyChanged
    {
        public static Style GetStyle(String name)
        {
            Style s = Application.Current.Resources[name] as Style;
            return s;
        }

        public enum ProfileStateEnum
	    {
            OFF, ON, ACTIVATING, DEACTIVATING
	    }

        public event PropertyChangedEventHandler PropertyChanged;

#region Informacje profilu
        public string Name { get; set; }
        public string PhysicalAddress { get; set; }
        public string Description { get; set; }
        public string GUID { get; set; }
        public string State { get; set; }
        public string SSID { get; set; }
        public string BSSID { get; set; }
        public string NetworkType { get; set; }
        public string RadioType { get; set; }
        public string Authentication { get; set; }
        public string Cipher { get; set; }
        public string ConnectionMode { get; set; }
        public string Channel { get; set; }
        public string ReceiveRate { get; set; }
        public string TransmitRate { get; set; }
        public string Signal { get; set; }
        public string ProfileName { get; set; }
#endregion

        //public ProfileStateEnum ProfileState { get; private set; }

        private ProfileStateEnum _profileState = ProfileStateEnum.OFF;

        // TEST
        public ProfileStateEnum ProfileState
        {
            get { return _profileState; }
            private set
            {
                Console.Out.WriteLine("Change profile " + ProfileName + " state to " + value);
                _profileState = value;
                // TODO: tego nie powinno być - będzie zrobione inaczej
                switch (value)
                {
                    case ProfileStateEnum.OFF:
                        StyleTest = GetStyle("DefaultButton");
                        break;
                    case ProfileStateEnum.ON:
                        StyleTest = GetStyle("ActiveButton");
                        break;
                    case ProfileStateEnum.ACTIVATING:
                        StyleTest = GetStyle("ActivatingButton");
                        break;
                    case ProfileStateEnum.DEACTIVATING:
                        StyleTest = GetStyle("DeactivatingButton");
                        break;
                    default:
                        break;
                }
            }
        }

        public NetInterface MyNetInterface { get; set; } // TODO private set

        private BackgroundWorker _interfaceRequestWorker = new BackgroundWorker();

        // Konstruktor tylko z name
        public Profile(String name)
        {
            Name = name;
        }

        public Profile(String name, NetInterface netInterface)
        {
            Name = name;

            if (netInterface == null)
            {
                // TODO
                throw new ArgumentNullException();
            }
            MyNetInterface = netInterface;

            _interfaceRequestWorker.DoWork +=
                new System.ComponentModel.DoWorkEventHandler(this.RequestProfileChange);

            _interfaceRequestWorker.RunWorkerCompleted +=
                new System.ComponentModel.RunWorkerCompletedEventHandler(this.ProfileChangeSuccess);
        }

        public void ToggleState()
        {
            Console.Out.WriteLine("ToggleState start " + this.Name);
            switch (ProfileState)
            {
                case ProfileStateEnum.ON:
                case ProfileStateEnum.DEACTIVATING:
                case ProfileStateEnum.ACTIVATING:
                    // TODO: ignorować?
                    break;
                case ProfileStateEnum.OFF:
                    _interfaceRequestWorker.RunWorkerAsync();
                    break;
                default:
                    break;
            }
            Console.Out.WriteLine("ToggleState end " + this.Name);
        }

        public void Activate()
        {
            ProfileState = ProfileStateEnum.ACTIVATING;
            // TODO: włączenie... gdzieś trzeba zrobić to współbieżnie z powiadomieniami
            ProfileState = ProfileStateEnum.ON;
        }

        public void Deactivate()
        {
            ProfileState = ProfileStateEnum.DEACTIVATING;
            // TODO: włączenie... gdzieś trzeba zrobić to współbieżnie z powiadomieniami
            ProfileState = ProfileStateEnum.OFF;
        }

        internal void RequestProfileChange(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Console.Out.WriteLine("Request profile change");
            MyNetInterface.ProfileChange(this);
        }

        internal void ProfileChangeSuccess(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            switch (ProfileState)
            {
                case ProfileStateEnum.OFF:
                case ProfileStateEnum.DEACTIVATING:
                case ProfileStateEnum.ACTIVATING:
                    // TODO: jakiś błąd?
                    break;
                case ProfileStateEnum.ON:
                    // TODO: jeszcze nie wiem co
                    break;
                default:
                    break;
            }
        }

        private Style _buttonStyle = GetStyle("DefaultButton");

        public Style StyleTest
        {
            get { return _buttonStyle; }
            set
            {
                _buttonStyle = value;
                OnPropertyChanged("StyleTest");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
