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
            // pusty profil na początek TODO
            Profile = new ProfileModel("<nie wybrano>", new NetInterfaceModel("<nie wybrano>", NetInterfaceType.Other));
        }

        private ProfileModel profile;

        public ProfileModel Profile
        {
            get { return profile; }
            set
            {
                profile = value;

                IsModified = false;

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
        }

        public void SetProfile(ProfileModel newProfile)
        {
            // TODO: ostrzeżenie, jeśli są niezapisane zmiany
            Profile = newProfile;
        }

        // TODO przy modyfikacji któregokolowiek pola ustawiwać na true
        public Boolean IsModified { get; set; }

        public String ProfileName { get; set; }
        public String InterfaceName { get { return profile.NetInterface.InterfaceName; } }

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
