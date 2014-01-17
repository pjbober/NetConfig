using ASK.ViewModels.NetsList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ASK.GUI
{
    /// <summary>
    /// Interaction logic for ProfileButton.xaml
    /// </summary>
    public partial class ProfileButton : Button
    {
        public ProfileButton()
        {
            InitializeComponent();
        }

        private void ProfileNameClick(object sender, RoutedEventArgs e)
        {
            (DataContext as ProfileButtonViewModel).ToggleState();
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            (DataContext as ProfileButtonViewModel).EditProfile();
        }



    }
}
