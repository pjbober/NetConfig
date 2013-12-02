using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ASK.Model.NetsList;
using System.Windows.Input;
using ASK.Logic;

namespace ASK.ViewModels.NetsList
{
    public delegate void ChangedProfileHandlerEvent(Profile newProfile);

    public class DummyNetsListViewModel
    {
        public ObservableCollection<NetInterfaceViewModel> interfaces { get; set; }

        public DummyNetsListViewModel()
        {
            WifiInterfacesParser parser = new WifiInterfacesParser();

            interfaces = new ObservableCollection<NetInterfaceViewModel>();
            NetInterface int1 = new NetInterface("Przewodowe");
            int1.AddProfile(new Profile("Dom", int1));
            NetInterface int2 = new NetInterface("Bezprzewodowe");
            int2.AddProfile(new Profile("AGH-WPA", int2));
            int2.AddProfile(new Profile("AGH-Guest", int2));
            foreach (Profile profile in parser.parse()) {
                profile.MyNetInterface = int2;
                int2.AddProfile(profile);
            }
            NetInterface int3 = new NetInterface("Virtual Box");
            AddInterface(int1);
            AddInterface(int2);
            AddInterface(int3);
        }

        public ObservableCollection<NetInterfaceViewModel> NetInterfacesCollection
        {
            get { return interfaces; }
        }

        public void AddInterface(NetInterface netInterface)
        {
            NetInterfaceViewModel model = new NetInterfaceViewModel(netInterface);
            model.ChangedProfile += model_ChangedProfile;
            interfaces.Add(model);
        }

        void model_ChangedProfile(Profile newProfile)
        {
            ChangedProfile(newProfile);
        }

        public event ChangedProfileHandlerEvent ChangedProfile;
    }
}
