using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
            SystemProfileModel system = new SystemProfileModel(netInterface, true);

            this.IsDHCP = system.IsDHCP;
            this.IP = system.IP;
            this.SubnetMask = system.SubnetMask;
            this.Gateway = system.Gateway;
            this.DNS = system.DNS;

            this.SSID = system.SSID;
            this.Security = system.Security;
            this.Encryption = system.Encryption;
            this.Authorization = system.Authorization;
            this.UseOneX = system.UseOneX;
            this.Key = system.Key;
            this.CAName = system.CAName;
            this.CAHash = system.CAHash;
        }

        public WifiProfileModel()
            : base()
        {

        }
    }
}
