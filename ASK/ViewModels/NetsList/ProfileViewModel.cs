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
            BtnCliked = new CommandHandler(x => MyCommand(x), true);
        }

        public void MyCommand(object obj)
        {
            Profile profile = obj as Profile;
            System.Windows.MessageBox.Show("You've chosen '" + profile.Name + "' profile");
        }

        public Profile ProfileModel { get; set; }

        public ICommand BtnCliked
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