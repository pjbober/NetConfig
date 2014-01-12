using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace NetworkManager
{
    public static class Helpers
    {

        public static IPAddress GetIP(NetworkInterface iface)
        {
            if (iface == null)
                return null;

            IPInterfaceProperties ipProperties = iface.GetIPProperties();

            foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    return ip.Address;

            return null;
        }

        public static IPAddress GetSubnetMask(NetworkInterface iface)
        {
            IPInterfaceProperties ipProperties = iface.GetIPProperties();

            foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    return ip.IPv4Mask;

            return null;
        }

        public static IPAddress GetGatewayAddress(NetworkInterface iface)
        {
            IPInterfaceProperties ipProperties = iface.GetIPProperties();

            foreach (GatewayIPAddressInformation ip in ipProperties.GatewayAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    return ip.Address;

            return null;
        }


        public static IPAddress GetDhcpAddress(NetworkInterface iface)
        {
            return null;
        }
    }
}
