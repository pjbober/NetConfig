using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.Model.NetsList;

namespace ASK.ViewModels.OptionsControl
{
    class DummyOptionsControlViewModel
    {
        public String ProfileName { get; set; }

        public void OnProfileChanged(Profile newProfile)
        {
            ProfileName = newProfile.Name;
        }
    }
}
