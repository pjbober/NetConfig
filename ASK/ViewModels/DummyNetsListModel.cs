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
            interfaces.Add(new NetInterface("Przewodowe"));
            interfaces.Add(new NetInterface("Bezprzewodowe"));
            interfaces.Add(new NetInterface("Virtual Box"));
        }

        public List<NetInterface> NetInterfacesCollection
        {
            get { return interfaces; }
        }
    }
}
