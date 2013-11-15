using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.Model;

namespace ASK.ViewModels
{
    class DummyNetsListModel
    {
        private List<NetInterface> interfaces;

        public DummyNetsListModel()
        {
            interfaces = new List<NetInterface>();
            NetInterface int1 = new NetInterface("Przewodowe");
            int1.AddProfile(new Profile("Dom"));
            NetInterface int2 = new NetInterface("Bezprzewodowe");
            int2.AddProfile(new Profile("AGH-WPA"));
            int2.AddProfile(new Profile("AGH-Guest"));
            NetInterface int3 = new NetInterface("Virtual Box");
            interfaces.Add(int1);
            interfaces.Add(int2);
            interfaces.Add(int3);
        }

        public List<NetInterface> NetInterfacesCollection
        {
            get { return interfaces; }
        }
    }
}
