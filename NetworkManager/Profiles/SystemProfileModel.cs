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

        public override bool IsWifi { get { return NetInterface.Type == NetInterfaceType.Wireless; } }

        #region WiredProfileModel overwritten settings

        public override bool IsDHCP { get { return NetInterface.IsDhcpEnabled(); } }
        public override IPAddress IP { get { return NetInterface.GetIP(); } }
        public override IPAddress SubnetMask { get { return NetInterface.GetNetmask(); } }
        public override IPAddress Gateway { get { return NetInterface.GetGateway(); } }
        public override IPAddress DNS { get { return NetInterface.GetDNS(); } }

        #endregion



        #region WifiProfileModel overwritten settings

        public override string SSID { get { return NetInterface.GetSSID(); } }
        public override SecurityType Security { get { return NetInterface.GetSecurityType(); } }
        public override EncryptionType Encryption { get { return NetInterface.GetEncryptionType(); } }
        public override AuthorizationMethod Authorization { get { return NetInterface.GetAuthMethod(); } }
        public override bool UseOneX { get { return NetInterface.IsUsingOneX(); } }
        public override string Key { get { return NetInterface.GetKey(); } }
        public override string CAName { get { return NetInterface.GetCert().Name; } }
        public override string CAHash { get { return NetInterface.GetCertHash(); } }

        #endregion



        public SystemProfileModel(NetInterfaceModel netInterface)
            : base("Konfiguracja systemowa", netInterface)
        {
            
        }

        public SystemProfileModel()
            : base()
        {

        }
    }
}
