using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace NetworkManager.Profiles
{

    public delegate void ProfileStateChangedEvent(AbstractProfileModel.StateEnum newState);
    public delegate void ProfileDataChangedEvent(AbstractProfileModel profile);
    public delegate void ProfileEditEndEvent(AbstractProfileModel profile);


    public abstract class AbstractProfileModel
    {

        public enum StateEnum
        {
            OFF, ON, ACTIVATING, DEACTIVATING
        }

        public string Name { get; set; }

        public string PhysicalAddress { get; set; }
        public string Description { get; set; }


        private StateEnum profileState;

        [XmlIgnoreAttribute]
        public NetInterfaceModel NetInterface { get; private set; }

        
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
            // zmienic typ w NetInterface
            //return this.NetInterface.ActiveProfile == this;
            return true;
        }

        public void ActivateAsync()
        {
            // zmienic typ w NetInterface
            //Thread t = new Thread(() => NetInterface.ActivateProfile(this));
            //t.Start();
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
        
    }
}
