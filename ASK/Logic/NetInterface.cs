using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace ASK.Logic
{
    class NetInterface
    {
        public int NetshId { get; private set; }

        public string Name { get; private set; }

        public NetInterfaceType Type { get; private set; }

        private NetworkInterface networkInterface;

        public NetInterface(int id, string name, NetInterfaceType type, NetworkInterface netIface)
        {
            NetshId = id;
            Name = name;
            Type = type;
            networkInterface = netIface;
        }


    }
}
