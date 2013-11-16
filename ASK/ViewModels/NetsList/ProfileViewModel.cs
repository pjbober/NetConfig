using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.Model.NetsList;
using System.Windows.Input;

namespace ASK.ViewModels.NetsList
{
    class ProfileViewModel
    {
        public ProfileViewModel(Profile profile)
        {
            ProfileModel = profile;
            BtnCliked = new MyCommand();
        }

        public Profile ProfileModel { get; set; }

        public ICommand BtnCliked
        {
            get;
            set;
        }
    }
}

class MyCommand : ICommand
{
    public bool CanExecute(object param)
    {
        return true;
    }

    public void Execute(object param)
    {
        System.Windows.MessageBox.Show("Not yet implemented");
    }

    public event EventHandler CanExecuteChanged;
}