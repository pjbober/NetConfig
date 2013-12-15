using ASK.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ASK.Model.NetsList
{
    public class NetsListModel
    {
        public ObservableCollection<NetInterfaceModel> NetInterfacesCollection { get; private set; }

        public NetsListModel()
        {
            NetInterfacesCollection = new ObservableCollection<NetInterfaceModel>();

            // dummy

            NetInterfaceModel ethernetInt = new NetInterfaceModel("Przewodowe", NetInterfaceType.Wired);
            ethernetInt.AddProfile(new ProfileModel("Dom", ethernetInt));

            NetInterfaceModel wirelessInt = new NetInterfaceModel("Bezprzewodowe", NetInterfaceType.Wireless);
            wirelessInt.AddProfile(new ProfileModel("WiFi 1", wirelessInt));
            wirelessInt.AddProfile(new ProfileModel("WiFi 2", wirelessInt));

            WifiInterfacesParser parser = new WifiInterfacesParser();

            foreach (ProfileModel profile in parser.parse(wirelessInt))
            {
                profile.NetInterface = wirelessInt;
                wirelessInt.AddProfile(profile);
            }

            NetInterfaceModel loopbackInt = new NetInterfaceModel("Loopback", NetInterfaceType.Loopback);
            loopbackInt.AddProfile(new ProfileModel("Domyślne", loopbackInt));

            NetInterfaceModel otherInt = new NetInterfaceModel("Hamachi", NetInterfaceType.Other);
            otherInt.AddProfile(new ProfileModel("Domyślne", otherInt));

            NetInterfacesCollection.Add(ethernetInt);
            NetInterfacesCollection.Add(wirelessInt);
            NetInterfacesCollection.Add(loopbackInt);
            NetInterfacesCollection.Add(otherInt);
        }

    }
}
