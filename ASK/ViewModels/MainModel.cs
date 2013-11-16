using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.ViewModels.NetsList;
using ASK.ViewModels.OptionsControl;

namespace ASK.ViewModels
{
    class MainModel
    {
        public DummyNetsListViewModel NetsList { get; set; }
        public DummyOptionsControlViewModel OptionsControl { get; set; }

        public MainModel()
        {
            NetsList = new DummyNetsListViewModel();
            OptionsControl = new DummyOptionsControlViewModel();
        }
    }
}
