using ASK.ViewModels.OptionsControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ASK.Model.NetsList
{
    public delegate void ProfileStateChangedEvent(ProfileModel.StateEnum newState);
    public delegate void ProfileDataChangedEvent(ProfileModel profile);

    public class ProfileModel : ModelBase
    {
        public enum StateEnum
	    {
            OFF, ON, ACTIVATING, DEACTIVATING
	    }

#region Profile information
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


        // IPv4
        public bool IsDHCP { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public string Gateway { get; set; }
        public string DNS { get; set; }

#endregion

        public event ProfileStateChangedEvent ProfileStateChangedEvent;
        public event ProfileDataChangedEvent ProfileDataChangedEvent;

        private StateEnum _profileState;

        public StateEnum ProfileState
        {
            get
            {
                return _profileState;
            }
            private set
            {
                _profileState = value;
                if (ProfileStateChangedEvent != null)
                {
                    ProfileStateChangedEvent(_profileState);
                }
            }
        }

        public NetInterfaceModel NetInterface { get; set; } // TODO private set

        private BackgroundWorker _interfaceRequestWorker = new BackgroundWorker();

        public ProfileModel(String name, NetInterfaceModel netInterface)
        {
            Name = name;

            if (netInterface == null)
            {
                // TODO
                throw new ArgumentNullException();
            }
            NetInterface = netInterface;

            ProfileState = StateEnum.OFF;

            _interfaceRequestWorker.DoWork +=
                new System.ComponentModel.DoWorkEventHandler(this.RequestProfileChange);

            _interfaceRequestWorker.RunWorkerCompleted +=
                new System.ComponentModel.RunWorkerCompletedEventHandler(this.ProfileChangeSuccess);


            // IPv4 mock
            IpAddress = "192.168.0.1";
            SubnetMask = "255.255.255.0";
            Gateway = "192.168.1.50";
            DNS = "192.168.1.51";

        }

        public void ToggleState()
        {
            //ProfileChangedEvent(this); // TODO

            Console.Out.WriteLine("ToggleState start " + this.Name);
            switch (ProfileState)
            {
                case StateEnum.ON:
                case StateEnum.DEACTIVATING:
                case StateEnum.ACTIVATING:
                    // TODO: ignorować?
                    break;
                case StateEnum.OFF:
                    _interfaceRequestWorker.RunWorkerAsync();
                    break;
                default:
                    break;
            }
            Console.Out.WriteLine("ToggleState end " + this.Name);
        }

        public void Activate()
        {
            ProfileState = StateEnum.ACTIVATING;
            // TODO: włączenie... gdzieś trzeba zrobić to współbieżnie z powiadomieniami
            ProfileState = StateEnum.ON;
        }

        public void Deactivate()
        {
            ProfileState = StateEnum.DEACTIVATING;
            // TODO: włączenie... gdzieś trzeba zrobić to współbieżnie z powiadomieniami
            ProfileState = StateEnum.OFF;
        }

        internal void RequestProfileChange(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            NetInterface.ProfileChange(this);
        }

        internal void ProfileChangeSuccess(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            switch (ProfileState)
            {
                case StateEnum.OFF:
                case StateEnum.DEACTIVATING:
                case StateEnum.ACTIVATING:
                    // TODO: jakiś błąd?
                    break;
                case StateEnum.ON:
                    // TODO: jeszcze nie wiem co
                    break;
                default:
                    break;
            }
        }

        public void EmitProfileDataChanged()
        {
            if (ProfileDataChangedEvent != null)
            {
                ProfileDataChangedEvent(this);
            }
        }

    }
}
