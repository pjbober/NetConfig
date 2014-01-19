using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.Model.NetsList;
using System.ComponentModel;
using NetworkManager;
using System.Threading;

namespace ASK.ViewModels.OptionsControl
{
    public enum SecurityEnum
    {
        OPEN,
        WEP,
        SHARED,
        WPA,
        WPAPSK,
        WPA2,
        WPA2PSK
    }

    public enum EncryptionEnum
    {
        AES,
        TKIP
    }

    public enum AuthEnum
    {
        CARD,
        PEAP
    }

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
                    _isDHCP = profile.IsDHCP;


                    EmitPropertyChanged("Profile");
                    EmitPropertyChanged("ProfileName");
                    EmitPropertyChanged("InterfaceName");

                    EmitPropertyChanged("IpAddress");
                    EmitPropertyChanged("SubnetMask");
                    EmitPropertyChanged("Gateway");
                    EmitPropertyChanged("DNS");

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
            profile.IsDHCP = _isDHCP;

            IsModified = true;

            profile.EmitProfileDataChanged();

            // jeśli zmieniliśmy profil, to należy go ponownie aktywować
            if (Profile.IsActive())
            {
                Profile.ActivateAsync();
            }

            SetProfile(null);

        }

        public void SetProfile(ProfileModel newProfile)
        {
            ProfileModel oldProfile = Profile;

            // TODO: ostrzeżenie, jeśli są niezapisane zmiany
            Profile = newProfile;

            if (oldProfile != null) oldProfile.EmitProfileEditEnd();
        }

        // TODO będzie nieużywane w wersji 1.0
        public Boolean IsModified { get; set; }

        public Boolean IsVisible { get { return Profile != null; } }
        
        public String ProfileName { get; set; }
        public String InterfaceName { get { return profile != null ? profile.NetInterface.Name : ""; } }

        public String IpAddress { get; set; }
        public String SubnetMask { get; set; }
        public String Gateway { get; set; }
        public String DNS { get; set; }

        private bool _isDHCP;
        public Boolean IsDHCP
        {
            get { return _isDHCP; }
            set { _isDHCP = value; EmitPropertyChanged("IsDHCP"); }
        }

        // Wifi

        public Boolean IsWifi { get { return true; } }

        // -- enumeracje

        private static Array _allSecurityEnums = Enum.GetValues(typeof(SecurityEnum));
        public Array AllSecurityEnums { get { return _allSecurityEnums; } }
        
        private static Array _allEncryptionEnums = Enum.GetValues(typeof(EncryptionEnum));
        public Array AllEncryptionEnums { get { return _allEncryptionEnums; } }

        private static Array _allAuthEnums = Enum.GetValues(typeof(AuthEnum));
        public Array AllAuthEnums { get { return _allAuthEnums; } }

        // Wartości
        public String SSID { get; set; }

        private SecurityEnum _securityType;
        public SecurityEnum SecurityType {
            get { return _securityType; }
            set
            {
                _securityType = value;
                EmitPropertyChanged("HasEncryptionOption");
                EmitPropertyChanged("HasPasswordOption");
                EmitPropertyChanged("HasAuthenticationOption");
            }
        }
        public EncryptionEnum EncryptionType { get; set; }
        public String WifiPassword { get; set; }
        public Boolean Use802 { get; set; }
        public AuthEnum AuthenticationType { get; set; }
        public String Certificate { get; set; }

        // Opcje edytora dla WiFi

        // WPA/WPA2 personal
        public bool HasEncryptionOption { 
            get {
                return SecurityType == SecurityEnum.WPA
                    || SecurityType == SecurityEnum.WPAPSK
                    || SecurityType == SecurityEnum.WPA2
                    || SecurityType == SecurityEnum.WPA2PSK;
            } 
        }

        // otwarte, współdzielone, WEP, WPA/WPA2 Personal
        public bool HasPasswordOption {
            get {
                return SecurityType == SecurityEnum.OPEN
                    || SecurityType == SecurityEnum.SHARED
                    || SecurityType == SecurityEnum.WEP
                    || SecurityType == SecurityEnum.WPAPSK
                    || SecurityType == SecurityEnum.WPA2PSK;
            }
        }

        // także cerfyfikat: WPA/WPA2 Enterprise
        public bool HasAuthenticationOption {
            get {
                return SecurityType == SecurityEnum.WPA2
                    || SecurityType == SecurityEnum.WPA;
            }
        }   

    }
}
