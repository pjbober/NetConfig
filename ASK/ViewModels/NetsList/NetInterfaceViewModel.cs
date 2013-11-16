using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ASK.Model.NetsList;

namespace ASK.ViewModels.NetsList
{
    class NetInterfaceViewModel
    {
        public NetInterfaceViewModel(NetInterface netInterface)
        {
            NetInterfaceModel = netInterface;
            Profiles = new ObservableCollection<ProfileViewModel>();
            foreach (Profile profile in netInterface.Profiles)
                Profiles.Add(new ProfileViewModel(profile));
        }

        public ObservableCollection<ProfileViewModel> Profiles { get; set; }

        public NetInterface NetInterfaceModel { get; set; }
    }
}
