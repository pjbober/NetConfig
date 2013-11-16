using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ASK.Model.NetsList;
using System.Windows.Input;

namespace ASK.ViewModels.NetsList
{
    class NetInterfaceViewModel
    {
        public NetInterfaceViewModel(NetInterface netInterface)
        {
            NetInterfaceModel = netInterface;
            BtnClicked = new CommandHandler(x => Clicked(x), true);
            Profiles = new ObservableCollection<Profile>();
            foreach (Profile profile in netInterface.Profiles)
                Profiles.Add(profile);
        }

        public ObservableCollection<Profile> Profiles { get; set; }

        public NetInterface NetInterfaceModel { get; set; }
        public void Clicked(object obj)
        {
            Profile profile = obj as Profile;
            System.Windows.MessageBox.Show("You've chosen profile " + profile.Name);
        }

        public ICommand BtnClicked
        {
            get;
            set;
        }
    }
}

public class CommandHandler : ICommand
{
    private Action<object> _action;
    private bool _canExecute;
    public CommandHandler(Action<object> action, bool canExecute)
    {
        _action = action;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
        _action(parameter);
    }
}

