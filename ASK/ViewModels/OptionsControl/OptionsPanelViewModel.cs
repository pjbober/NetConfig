using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.Model.NetsList;
using System.ComponentModel;

namespace ASK.ViewModels.OptionsControl
{
    public class OptionsPanelViewModel : ViewModelBase
    {
        public OptionsPanelViewModel()
        {
            // pusty profil na początek
            Profile = new ProfileModel("<nie wybrano>", new NetInterfaceModel("<nie wybrano>", NetInterfaceType.Other));
        }

        private ProfileModel profile;

        public ProfileModel Profile
        {
            get { return profile; }
            set
            {
                profile = value;
                EmitPropertyChanged("Profile");
                EmitPropertyChanged("ProfileName");

                EmitPropertyChanged("IpAddress");
                EmitPropertyChanged("SubnetMask");
                EmitPropertyChanged("Gateway");
                EmitPropertyChanged("DNS");
                EmitPropertyChanged("MAC");

                EmitPropertyChanged("IsDHCP");
            }
        }

        public void OnProfileChanged(ProfileModel newProfile)
        {
            // TODO: ostrzeżenie, jeśli są niezapisane zmiany
            Profile = newProfile;

            // TODO: wypełnienie pól
        }

        public String ProfileName { get { return Profile.Name; } }

        public String IpAddress { get { return Profile.IpAddress; } }
        public String SubnetMask { get { return Profile.SubnetMask; } }
        public String Gateway { get { return Profile.Gateway; } }
        public String DNS { get { return Profile.DNS; } }
        public String MAC { get { return Profile.PhysicalAddress; } }

        public Boolean IsDHCP { get; set; }

        public String InterfaceName { get { return Profile.NetInterface.InterfaceName; } }
    }
}
