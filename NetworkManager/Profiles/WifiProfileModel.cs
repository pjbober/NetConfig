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
            OPEN,       // Otwarte
            WEP,        // WEP
            SHARED,     // Udostepnione
            WPA,        // WPA-Enterprise
            WPAPSK,     // WPA-Personal
            WPA2,       // WPA2-Enterprise
            WPA2PSK,    // WPA2-Personal
            Other       // ?
        }

        public enum EncryptionType
        {
            WEP,
            TKIP,
            AES,
            Other,
            None
        }

        public enum AuthorizationMethod
        {
            PEAP,
            Card,
            None
        }

        public override bool IsWifi { get { return true; } }

        public virtual string SSID { get; set; }
        public virtual SecurityType Security { get; set; }
        public virtual EncryptionType Encryption { get; set; }
        public virtual AuthorizationMethod Authorization { get; set; }
        public virtual bool UseOneX { get; set; }
        public virtual string Key { get; set; }
        public virtual string CAName { get; set; }
        public virtual string CAHash { get; set; }

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
