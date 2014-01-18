using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using NativeWifi;

namespace NetworkManager
{
    public class NetInterfaceManager
    {


        public IList<NetInterface> Interfaces { get { return networkInterfaces; } }

        private IList<NetInterface> networkInterfaces;

        private WlanClient client = new WlanClient();

        public NetInterfaceManager()
        {
            networkInterfaces = GetAllNetInterfaces();
            RefreshWlanInterfaces();

            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);

        }


        private IList<NetworkAdapter> GetNetworkAdapters()
        {
            IList<NetworkAdapter> adapters = new List<NetworkAdapter>();

            foreach (NetworkAdapter adapter in NetworkAdapterEnumerator.GetAllNetworkAdapters())
            {
                if (!String.IsNullOrWhiteSpace(adapter.NetConnectionID))
                    adapters.Add(adapter);
            }

            return adapters;
        }


        private void RefreshNetworkInterfaces() 
        {
            IDictionary<string, NetworkInterface> ifaceDict = NetworkInterface.GetAllNetworkInterfaces().ToDictionary(i => i.Name);

            foreach (NetInterface netIface in this.networkInterfaces)
            {
                NetworkInterface value;
                if (ifaceDict.TryGetValue(netIface.Name, out value))
                    netIface.SetNetworkInterface(value);
            }
        }

        private IList<NetInterface> GetAllNetInterfaces()
        {
            IList<NetInterface> interfaces = new List<NetInterface>();
            IList<NetworkAdapter> adapters = new List<NetworkAdapter>();

            foreach(NetworkAdapter adapter in NetworkAdapterEnumerator.GetAllNetworkAdapters())
            {
                if (!String.IsNullOrWhiteSpace(adapter.NetConnectionID))
                    adapters.Add(adapter);
            }

            IDictionary<string, NetworkInterface> ifaceDict = NetworkInterface.GetAllNetworkInterfaces().ToDictionary(i => i.Name);

            foreach (NetworkAdapter adapter in adapters)
            {
                NetworkInterface value;
                if (ifaceDict.TryGetValue(adapter.NetConnectionID, out value))
                    interfaces.Add(new NetInterface(adapter, value));
                else
                    interfaces.Add(new NetInterface(adapter, null));
            }

            return interfaces;
        }


        private void RefreshWlanInterfaces()
        {
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                NetInterface niface = networkInterfaces.FirstOrDefault(it => it.Description == wlanIface.InterfaceDescription);

                if (niface != null && niface.Type == NetInterfaceType.Wireless)
                    niface.SetWlanInterface(wlanIface);
            }
        }


        static void AddressChangedCallback(object sender, EventArgs e)
        {

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface n in adapters)
            {
                Console.WriteLine("   {0} is {1}", n.Name, n.OperationalStatus);
            }
        }

    }
}
