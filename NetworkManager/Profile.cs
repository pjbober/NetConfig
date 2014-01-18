using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;


namespace NetworkManager
{
    class Profile
    {
        //public delegate void ProfileStateChangedEvent(ProfileModel.StateEnum newState);
        //public delegate void ProfileDataChangedEvent(ProfileModel profile);

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

        //public event ProfileStateChangedEvent ProfileStateChangedEvent;
        //public event ProfileDataChangedEvent ProfileDataChangedEvent;

        private StateEnum _profileState;

        public StateEnum ProfileState
        {
            get
            {
                return _profileState;
            }
        }

        public void Activate()
        {
            ///ProfileState = StateEnum.ACTIVATING;
            // TODO: włączenie... gdzieś trzeba zrobić to współbieżnie z powiadomieniami
            ///ProfileState = StateEnum.ON;
        }

        public void Deactivate()
        {
            ///ProfileState = StateEnum.DEACTIVATING;
            // TODO: włączenie... gdzieś trzeba zrobić to współbieżnie z powiadomieniami
            ///ProfileState = StateEnum.OFF;
        }

        internal void RequestProfileChange(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //NetInterface.ProfileChange(this);
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
    }
}
