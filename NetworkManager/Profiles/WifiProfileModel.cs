using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NetworkManager.Profiles
{
    public class WifiProfileModel : WiredProfileModel, IEquatable<WifiProfileModel>
    {
        public enum SecurityEnum
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

        public enum EncryptionEnum
        {
            WEP,
            TKIP,
            AES,
            Other,
            None
        }

        public enum AuthEnum
        {
            PEAP,
            Card,
            None
        }

        public virtual string SSID { get; set; }
        public virtual SecurityEnum Security { get; set; }
        public virtual EncryptionEnum Encryption { get; set; }
        public virtual AuthEnum Authorization { get; set; }
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

        public override bool Equals(AbstractProfileModel other)
        {
            if (other is WifiProfileModel)
                return Equals(other as WifiProfileModel);

            return false;
        }


        public bool Equals(WifiProfileModel other)
        {
            return (other as WiredProfileModel).Equals(this as WiredProfileModel); // &&
                
                    //other.SSID == this.SSID &&
                    //other.Security == this.Security &&
                    //other.Encryption == this.Encryption &&
                    //other.Authorization == this.Authorization &&
                    //other.UseOneX == this.UseOneX &&
                    //other.Key == this.Key &&
                    //other.CAName == this.CAName &&
                    //other.CAHash == this.CAHash;
        }
    }
}
