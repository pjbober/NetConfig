using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.Model.NetsList;
using System.ComponentModel;
using NetworkManager;
using NetworkManager.Profiles;
using System.Threading;
using System.Net;

using NetworkManager.Profiles;

namespace ASK.ViewModels.OptionsControl
{
    public class OptionsPanelViewModel : ViewModelBase
    {
        public OptionsPanelViewModel()
        {
            SetProfile(null);
        }

        private AbstractProfileModel profile;

        public AbstractProfileModel Profile
        {
            get { return profile; }
            set
            {
                profile = value;
                IsModified = false;

                if (profile != null)
                {
                    ProfileName = profile.Name;

                    EmitPropertyChanged("ProfileName");

                    if (profile is WiredProfileModel)
                    {
                        WiredProfileModel wiredProfile = profile as WiredProfileModel;

                        // wartości tymczasowe - nie są od razu zapisywane do modelu
                        IpAddress = wiredProfile.IP;
                        SubnetMask = wiredProfile.SubnetMask;
                        Gateway = wiredProfile.Gateway;
                        DNS = wiredProfile.DNS;

                        _isDHCP = wiredProfile.IsDHCP;


                        EmitPropertyChanged("Profile");
                        EmitPropertyChanged("InterfaceName");

                        EmitPropertyChanged("IpAddress");
                        EmitPropertyChanged("SubnetMask");
                        EmitPropertyChanged("Gateway");
                        EmitPropertyChanged("DNS");

                        EmitPropertyChanged("IsDHCP");
                    }
                }

                EmitPropertyChanged("IsVisible");
            }
        }

        public void SaveProfile()
        {

            profile.Name = ProfileName;

            if (profile is WiredProfileModel)
            {
                WiredProfileModel wiredProfile = profile as WiredProfileModel;

                wiredProfile.IP = IpAddress;
                wiredProfile.SubnetMask = SubnetMask;
                wiredProfile.Gateway = Gateway;
                wiredProfile.DNS = DNS;
                wiredProfile.IsDHCP = _isDHCP;
            }

            IsModified = true;

            profile.EmitProfileDataChanged();

            // jeśli zmieniliśmy profil, to należy go ponownie aktywować
            if (Profile.IsActive())
            {
                Profile.ActivateAsync();
            }

            SetProfile(null);

        }

        public void SetProfile(AbstractProfileModel newProfile)
        {
            AbstractProfileModel oldProfile = Profile;

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

        public Certificate Cert { get; set; }

        private bool _isDHCP;
        public Boolean IsDHCP
        {
            get { return _isDHCP; }
            set { _isDHCP = value; EmitPropertyChanged("IsDHCP"); }
        }

        // Wifi

        public Boolean IsWifi { get { return true; } }

        // -- enumeracje

        private static Array _allSecurityEnums = Enum.GetValues(typeof(NetworkManager.Profiles.WifiProfileModel.SecurityEnum));
        public Array AllSecurityEnums { get { return _allSecurityEnums; } }
        
        private static Array _allEncryptionEnums = Enum.GetValues(typeof(NetworkManager.Profiles.WifiProfileModel.EncryptionEnum));
        public Array AllEncryptionEnums { get { return _allEncryptionEnums; } }

        private static Array _allAuthEnums = Enum.GetValues(typeof(NetworkManager.Profiles.WifiProfileModel.AuthEnum));
        public Array AllAuthEnums { get { return _allAuthEnums; } }

        private static IList<Certificate> _allCertEnums = CertificateCollection.Certificates;
        public IList<Certificate> AllCertEnums { get { return _allCertEnums; } }

        // Wartości
        public String SSID { get; set; }

        private WifiProfileModel.SecurityEnum _securityType;
        public WifiProfileModel.SecurityEnum SecurityType
        {
            get { return _securityType; }
            set
            {
                _securityType = value;
                EmitPropertyChanged("HasEncryptionOption");
                EmitPropertyChanged("HasPasswordOption");
                EmitPropertyChanged("HasAuthenticationOption");
            }
        }
        public WifiProfileModel.EncryptionEnum EncryptionType { get; set; }
        public String WifiPassword { get; set; }
        public Boolean Use802 { get; set; }
        public WifiProfileModel.AuthEnum AuthenticationType { get; set; }
        //public String Certificate { get; set; }

        // Opcje edytora dla WiFi

        // WPA/WPA2 personal
        public bool HasEncryptionOption { 
            get {
                return SecurityType == WifiProfileModel.SecurityEnum.WPA
                    || SecurityType == WifiProfileModel.SecurityEnum.WPAPSK
                    || SecurityType == WifiProfileModel.SecurityEnum.WPA2
                    || SecurityType == WifiProfileModel.SecurityEnum.WPA2PSK;
            } 
        }

        // otwarte, współdzielone, WEP, WPA/WPA2 Personal
        public bool HasPasswordOption {
            get {
                return SecurityType == WifiProfileModel.SecurityEnum.OPEN
                    || SecurityType == WifiProfileModel.SecurityEnum.SHARED
                    || SecurityType == WifiProfileModel.SecurityEnum.WEP
                    || SecurityType == WifiProfileModel.SecurityEnum.WPAPSK
                    || SecurityType == WifiProfileModel.SecurityEnum.WPA2PSK;
            }
        }

        // także cerfyfikat: WPA/WPA2 Enterprise
        public bool HasAuthenticationOption {
            get {
                return SecurityType == WifiProfileModel.SecurityEnum.WPA2
                    || SecurityType == WifiProfileModel.SecurityEnum.WPA;
            }
        }   

    }
}
