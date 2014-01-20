using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Serialization;


namespace NetworkManager.Profiles
{
    public class SystemProfileModel : WifiProfileModel
    {

        #region WiredProfileModel overwritten settings

        public override bool IsDHCP { get { return NetInterface.IsDhcpEnabled(); } set { } }

        public override string IP { get { return NetInterface.GetIP().ToString(); } set { } }
        public override string SubnetMask { get { return NetInterface.GetNetmask().ToString(); } set { } }
        public override string Gateway { get { return NetInterface.GetGateway().ToString(); } set { } }
        public override string DNS { get { return NetInterface.GetDNS().ToString(); } set { } }

        #endregion



        #region WifiProfileModel overwritten settings
        public override string SSID { get { return NetInterface.GetSSID(); } set { } }
        public override SecurityEnum Security { get { return NetInterface.GetSecurityType(); } set { } }
        public override EncryptionEnum Encryption { get { return NetInterface.GetEncryptionType(); } set { } }
        public override AuthEnum Authorization { get { return NetInterface.GetAuthMethod(); } set { } }
        public override bool UseOneX { get { return NetInterface.IsUsingOneX(); } set { } }
        public override string Key { get { return NetInterface.GetKey(); } set { } }
        public override string CAName
        {
            get
            {
                Certificate cert = NetInterface.GetCert();
                if (cert != null)
                    return NetInterface.GetCert().Name;
                else
                    return "";
            }
            set { } 
        }
        public override string CAHash { get { return NetInterface.GetCertHash(); } set { } }

        #endregion



        public SystemProfileModel(NetInterfaceModel netInterface)
            : base("Konfiguracja systemowa", netInterface)
        {
            
        }

        public SystemProfileModel(NetInterfaceModel netInterface, bool dummy)
            : base()
        {
            this.NetInterface = netInterface;
        }

        public SystemProfileModel()
            : base()
        {

        }

        public override bool Equals(AbstractProfileModel other)
        {
            if (other is SystemProfileModel)
                return Equals(other as SystemProfileModel);

            return false;
        }

        public bool Equals(SystemProfileModel other)
        {
            return other.NetInterface.MACAddress == this.NetInterface.MACAddress;
        }
    }
}
