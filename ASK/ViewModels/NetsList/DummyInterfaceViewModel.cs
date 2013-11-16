using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ASK.Model.NetsList;

namespace ASK.ViewModels.NetsList
{
    class DummyInterfaceViewModel
    {
        public ObservableCollection<NetInterface> interfaces { get; set; }
        public int SelectedProfile { get; set; }

        public void Add(NetInterface interf)
        {
            interfaces.Add(interf);
        }

    }
}
