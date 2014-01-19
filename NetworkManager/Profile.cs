using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Xml.Serialization;

namespace NetworkManager
{
    public delegate void ProfileStateChangedEvent(ProfileModel.StateEnum newState);
    public delegate void ProfileDataChangedEvent(ProfileModel profile);
    public delegate void ProfileEditEndEvent(ProfileModel profile);

    public class ProfileModel
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
        public event ProfileEditEndEvent ProfileEditEndEvent;
        
        private StateEnum _profileState;

        public StateEnum ProfileState
        {
            get
            {
                return _profileState;
            }
            set
            {
                _profileState = value;
                if (ProfileStateChangedEvent != null)
                {
                    ProfileStateChangedEvent(_profileState);
                }
            }
        }

        [XmlIgnoreAttribute]
        public NetInterfaceModel NetInterface { get; set; } // TODO private set

        private BackgroundWorker _interfaceRequestWorker = new BackgroundWorker();

        public ProfileModel()
        {

        }

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

            // IPv4 mock
            IpAddress = "192.168.0.1";
            SubnetMask = "255.255.255.0";
            Gateway = "192.168.1.50";
            DNS = "192.168.1.51";

        }

        public bool IsActive()
        {
            return this.NetInterface.ActiveProfile == this;
        }

        public void ToggleState()
        {
            Console.Out.WriteLine("ToggleState start " + this.Name);
            switch (ProfileState)
            {
                case StateEnum.DEACTIVATING:
                case StateEnum.ACTIVATING:
                    // ignorowanie
                    break;
                case StateEnum.ON:
                case StateEnum.OFF:
                    ActivateAsync();
                    break;
                default:
                    break;
            }
            Console.Out.WriteLine("ToggleState end " + this.Name);
        }

        public void ActivateAsync()
        {
            Thread t = new Thread(() => NetInterface.ActivateProfile(this));
            t.Start();
        }

        internal void RequestProfileChange(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            NetInterface.ActivateProfile(this);
        }


        public void EmitProfileDataChanged()
        {
            if (ProfileDataChangedEvent != null)
            {
                ProfileDataChangedEvent(this);
            }
        }

        public void EmitProfileEditEnd()
        {
            if (ProfileEditEndEvent != null)
            {
                ProfileEditEndEvent(this);
            }
        }

    }
}
