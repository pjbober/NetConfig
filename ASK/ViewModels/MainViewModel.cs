using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.ViewModels.NetsList;
using ASK.ViewModels.OptionsControl;
using ASK.Model.NetsList;

namespace ASK.ViewModels
{
    public class MainViewModel
    {
        public NetsListViewModel NetsListViewModel { get; set; }
        public OptionsPanelViewModel OptionsPanelViewModel { get; set; }

        public MainViewModel(NetsListModel netsListModel)
        {
            NetsListViewModel = new NetsListViewModel(netsListModel);
            OptionsPanelViewModel = new OptionsPanelViewModel();
            NetsListViewModel.ProfileChangedEvent += OptionsPanelViewModel.OnProfileChanged;
        }
    }
}
