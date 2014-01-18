using ASK.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASK.Model.NetsList
{
    public delegate void ProfileAdded(ProfileModel newProfile);

    public delegate void InterfaceUp();
    public delegate void InterfaceDown();

    public delegate void Connected();
    public delegate void  Disconnected();

    public delegate void  IPSettingsChanged();
    public delegate void  WifiSettingsChanged();

    public enum NetInterfaceType
    {
        Loopback,
        Wired,
        Wireless,
        Other
    }

    public enum NetInterfaceState
    {
        Connected,
        Disconnected
    }

    public class NetInterfaceModel
    {
        public string InterfaceName { get; set; }
        public List<ProfileModel> Profiles { get; set; }

        public NetInterfaceType Type { get; private set; }

        public NetInterfaceState State { get; private set; }

        public ProfileModel CurrentActiveProfile
        {
            get
            {
                foreach (ProfileModel p in Profiles)
                {
                    // sprawdź, który profil jest włączony - powinien być tylko jeden
                    // TODO: wykrywanie sytuacji, gdy kilka profili jest aktywnych? niezgodne stany?
                    if (p.ProfileState == ProfileModel.StateEnum.ON)
                    {
                        return p;
                    }
                }
                return null;
            }
        }

        public NetInterfaceModel(string name, NetInterfaceType type)
        {
            Profiles = new List<ProfileModel>();
            this.InterfaceName = name;
            this.Type = type;
            this.State = NetInterfaceState.Connected; // TODO
            this.IsEnabled = true;
        }

        public event ProfileAdded ProfileAddedEvent;

        public void AddProfile(ProfileModel profile)
        {
            Profiles.Add(profile);
            if (ProfileAddedEvent != null)
            {
                ProfileAddedEvent(profile);
            }
        }

        public void ProfileChange(ProfileModel profile)
        {
            Console.Out.WriteLine("Changing profile to: " + profile.ProfileName);

            switch (profile.ProfileState)
            {
                case ProfileModel.StateEnum.ON:
                    // TODO: coś poszło nie tak - profil zazwyczaj domaga się włączenia
                    throw new Exception();
                //break;
                case ProfileModel.StateEnum.OFF:
                case ProfileModel.StateEnum.ACTIVATING:
                // TODO: powdójny request - niecierpliwy użytkownik?
                case ProfileModel.StateEnum.DEACTIVATING:
                default:
                    break;
            }

            // TODO: sytuacja, kiedy jakiś profil się aktywuje bądź deaktywuje
            ProfileModel currentActiveProfile = CurrentActiveProfile;

            if (currentActiveProfile != null)
            {
                currentActiveProfile.Deactivate(); // blokująca deaktywacja profilu
                if (currentActiveProfile.ProfileState != ProfileModel.StateEnum.OFF)
                {
                    // TODO: nie udało się deaktywować, ponowić próbę?
                }
            }

            // teraz żaden inny profil nie powinien być aktywny
            profile.Activate(); // blokująca aktywacja profilu z żądania
            if (profile.ProfileState != ProfileModel.StateEnum.ON)
            {
                // TODO: coś poszło nie tak
            }

            // sukces
        }

        internal void AddNewProfile()
        {
            // TODO przemyśleć zachowanie
            AddProfile(new ProfileModel("Nowy profil", this));
        }

        // TODO mock
        public bool IsEnabled { get; set; }

        public bool Enable()
        {
            IsEnabled = true;
            if (InterfaceUp != null) InterfaceUp();
            return true;
        }

        public bool Disable()
        {
            IsEnabled = false;
            if (InterfaceDown != null) InterfaceDown();
            return true;
        }

        // zdarzenia

        public event InterfaceUp InterfaceUp;
        public event InterfaceDown InterfaceDown;

        public event Connected Connect;
        public event Disconnected Disconnect;

        public event IPSettingsChanged IPSettingsChanged;
        public event WifiSettingsChanged WifiSettingsChanged;
    }
}
