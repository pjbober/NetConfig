using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Net;

namespace NetworkManager.Profiles
{

    public delegate void ProfileStateChangedEvent(AbstractProfileModel.StateEnum newState);
    public delegate void ProfileDataChangedEvent(AbstractProfileModel profile);
    public delegate void ProfileEditEndEvent(AbstractProfileModel profile);

    [XmlInclude(typeof(SystemProfileModel))]
    [XmlInclude(typeof(WiredProfileModel))]
    [XmlInclude(typeof(WifiProfileModel))]
    public abstract class AbstractProfileModel : IEquatable<AbstractProfileModel>
    {

        public enum StateEnum
        {
            OFF, ON, ACTIVATING, DEACTIVATING
        }

        public string Name { get; set; }

        public virtual string PhysicalAddress { get; set; }

        public virtual string Description { get; set; }

        [XmlIgnoreAttribute]
        public virtual bool IsWifi { get { return NetInterface.Type == NetInterfaceType.Wireless; } }

        private StateEnum profileState;

        [XmlIgnoreAttribute]
        public virtual NetInterfaceModel NetInterface { get; set; }

        
        public event ProfileStateChangedEvent ProfileStateChangedEvent;
        public event ProfileDataChangedEvent ProfileDataChangedEvent;
        public event ProfileEditEndEvent ProfileEditEndEvent;


        public AbstractProfileModel(String name, NetInterfaceModel netInterface)
        {
            Name = name;

            if (netInterface == null)
            {
                // TODO
                throw new ArgumentNullException();
            }
            NetInterface = netInterface;

            ProfileState = StateEnum.OFF;
        }

        public AbstractProfileModel()
        {
            this.profileState = StateEnum.OFF;
        }

        public StateEnum ProfileState
        {
            get
            {
                return profileState;
            }
            set
            {
                profileState = value;
                if (ProfileStateChangedEvent != null)
                {
                    ProfileStateChangedEvent(profileState);
                }
            }
        }

        public bool IsActive()
        {
            return this.NetInterface.ActiveProfile == this;
        }

        public void ActivateAsync()
        {
            Thread t = new Thread(() => NetInterface.ActivateProfile(this));
            t.Start();
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

        public virtual bool Equals(AbstractProfileModel other)
        {
            return true;
        }
    }
}
