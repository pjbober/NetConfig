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


        public IList<NetInterfaceModel> Interfaces { get { return networkInterfaces; } }

        private IList<NetInterfaceModel> networkInterfaces;

        private WlanClient client = new WlanClient();

        private bool disposed = false;


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

            foreach (NetInterfaceModel netIface in this.networkInterfaces)
            {
                NetworkInterface value;
                if (ifaceDict.TryGetValue(netIface.Name, out value))
                    netIface.SetNetworkInterface(value);
            }
        }

        private IList<NetInterfaceModel> GetAllNetInterfaces()
        {
            IList<NetInterfaceModel> interfaces = new List<NetInterfaceModel>();
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
                    interfaces.Add(new NetInterfaceModel(adapter, value));
                else
                    interfaces.Add(new NetInterfaceModel(adapter, null));
            }

            return interfaces;
        }


        private void RefreshWlanInterfaces()
        {
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                NetInterfaceModel niface = networkInterfaces.FirstOrDefault(it => it.Description == wlanIface.InterfaceDescription);

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (networkInterfaces != null)
                    {
                        foreach(NetInterfaceModel niface in networkInterfaces)
                            if (niface != null)
                            {
                                niface.Dispose();
                            }
                    }
                }

                networkInterfaces = null;
                disposed = true;
            }
        }

    }
}
