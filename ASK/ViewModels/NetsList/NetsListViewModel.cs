using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ASK.Model.NetsList;
using System.Windows.Input;
using ASK.Logic;
using System.ComponentModel;

namespace ASK.ViewModels.NetsList
{
    public class NetsListViewModel : ViewModelBase
    {
        public ObservableCollection<NetInterfaceViewModel> NetInterfaceViewModels { get; set; }

        public NetsListModel NetsListModel { get; private set; }

        public NetsListViewModel(NetsListModel netsListModel)
        {
            NetInterfacesCollection = new ObservableCollection<NetInterfaceViewModel>();

            NetsListModel = netsListModel;

            // TODO: może generować dynamicznie?
            foreach (var ni in NetsListModel.NetInterfacesCollection)
            {
                NetInterfacesCollection.Add(new NetInterfaceViewModel(ni));
            }

        }

        public ObservableCollection<NetInterfaceViewModel> NetInterfacesCollection
        {
            get;
            private set;
        }

        public void AddInterface(NetInterfaceModel netInterface)
        {
            var netInterfaceViewModel = new NetInterfaceViewModel(netInterface);
            netInterfaceViewModel.ProfileChangedEvent += EmitChangedProfile;
            NetInterfacesCollection.Add(netInterfaceViewModel);
        }

        public event ProfileChangedHandlerEvent ProfileChangedEvent;

        public void EmitChangedProfile(ProfileModel newProfile)
        {
            ProfileChangedEvent(newProfile); // TODO: to powinno iść do jakiegoś select profile
            newProfile.ToggleState(); // TODO: to powinno iść do podwójnego kliku
        }

    }
}
