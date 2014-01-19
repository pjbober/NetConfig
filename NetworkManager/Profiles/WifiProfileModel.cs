using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkManager.Profiles
{
    public class WifiProfileModel : WiredProfileModel
    {
        public enum SecurityType
        {
            OPEN,
            WEP,
            SHARED,
            WPA,
            WPAPSK,
            WPA2,
            WPA2PSK
        }

        public enum EncryptionType
        {
            WEP,
            TKIP,
            AES,
            None
        }

        public enum AuthorizationMethod
        {
            PEAP,
            Card,
            None
        }

        public string SSID { get; set; }
        public SecurityType Security { get; set; }
        public EncryptionType Encryption { get; set; }
        public AuthorizationMethod Authorization { get; set; }
        public bool UseOneX { get; set; }
        public string Key { get; set; }
        public string CAName { get; set; }
        public string CAHash { get; set; }

        public WifiProfileModel(String name, NetInterfaceModel netInterface)
            : base(name, netInterface)
        {
            
        }

        public WifiProfileModel()
            : base()
        {

        }
    }
}
