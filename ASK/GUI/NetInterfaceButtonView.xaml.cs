using ASK.ViewModels.NetsList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for NetInterfaceButton.xaml
    /// </summary>
    public partial class NetInterfaceButton : Button
    {
        // DataContext powinien być typu NetInterfaceViewModel

        public NetInterfaceButton()
        {
            InitializeComponent();
        }

        private void netInterfaceButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as NetInterfaceViewModel).IsExpanded =
                !(DataContext as NetInterfaceViewModel).IsExpanded;
        }

        private void TurnOnInterface_Click(object sender, RoutedEventArgs e)
        {
            //(DataContext as NetInterfaceViewModel).
        }

        private void AddProfile_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as NetInterfaceViewModel).AddNewProfile();
        }
    }
}
