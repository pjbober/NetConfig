using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.Model.NetsList;
using System.ComponentModel;
using NetworkManager;

namespace ASK.ViewModels.OptionsControl
{
    public class OptionsPanelViewModel : ViewModelBase
    {
        public OptionsPanelViewModel()
        {
            SetProfile(null);
        }

        private ProfileModel profile;

        public ProfileModel Profile
        {
            get { return profile; }
            set
            {
                profile = value;
                IsModified = false;

                if (profile != null)
                {

                    // wartości tymczasowe - nie są od razu zapisywane do modelu
                    ProfileName = profile.Name;
                    IpAddress = profile.IpAddress;
                    SubnetMask = profile.SubnetMask;
                    Gateway = profile.Gateway;
                    DNS = profile.DNS;
                    MAC = profile.PhysicalAddress;
                    _isDHCP = profile.IsDHCP;


                    EmitPropertyChanged("Profile");
                    EmitPropertyChanged("ProfileName");
                    EmitPropertyChanged("InterfaceName");

                    EmitPropertyChanged("IpAddress");
                    EmitPropertyChanged("SubnetMask");
                    EmitPropertyChanged("Gateway");
                    EmitPropertyChanged("DNS");
                    EmitPropertyChanged("MAC");

                    EmitPropertyChanged("IsDHCP");
                }

                EmitPropertyChanged("IsVisible");
            }
        }

        public void SaveProfile()
        {
            profile.Name = ProfileName;
            profile.IpAddress = IpAddress;
            profile.SubnetMask = SubnetMask;
            profile.Gateway = Gateway;
            profile.DNS = DNS;
            profile.PhysicalAddress = MAC;
            profile.IsDHCP = _isDHCP;

            IsModified = true;

            profile.EmitProfileDataChanged();
            SetProfile(null);
        }

        public void SetProfile(ProfileModel newProfile)
        {
            ProfileModel oldProfile = Profile;

            // TODO: ostrzeżenie, jeśli są niezapisane zmiany
            Profile = newProfile;

            if (oldProfile != null) oldProfile.EmitProfileEditEnd();
        }

        // TODO przy modyfikacji któregokolowiek pola ustawiwać na true
        public Boolean IsModified { get; set; }

        public Boolean IsVisible { get { return Profile != null; } }

        public String ProfileName { get; set; }
        public String InterfaceName { get { return profile != null ? profile.NetInterface.Name : ""; } }

        public String IpAddress { get; set; }
        public String SubnetMask { get; set; }
        public String Gateway { get; set; }
        public String DNS { get; set; }
        public String MAC { get; set; }

        private bool _isDHCP;
        public Boolean IsDHCP
        {
            get { return _isDHCP; }
            set { _isDHCP = value; EmitPropertyChanged("IsDHCP"); }
        }

    }
}
