using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkManager;


namespace NetworkManagerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var netman = new NetInterfaceManager();
            var ifaces = netman.Interfaces;

            foreach(var i in ifaces) {
                if (i.Type != NetInterfaceType.Wireless)
                    continue;

                var x = i.ListWifiNetworks();
                string ssid = i.GetSSID();
                string bssid = i.GetBSSID();
                var sec = i.GetSecurityType();
                var enc = i.GetEncryptionType();
                var auth = i.GetAuthMethod();
                var useonex = i.IsUsingOneX();
                var key = i.GetKey();
                var cert = i.GetCert();
            }

            var certs = CertificateCollection.Certificates;

            while (Console.Read() != 'q')
            {
                Console.WriteLine(ifaces[0].Name + "\t" + ifaces[0].IsEnabled);
                //System.Threading.Thread.Sleep(1000);
            }

            foreach (NetInterfaceModel ni in ifaces)
                ni.Dispose();
        }
    }
}
