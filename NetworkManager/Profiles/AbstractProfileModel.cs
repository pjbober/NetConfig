using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
