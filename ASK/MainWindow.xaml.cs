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
using ASK.ViewModels;
using ASK.ViewModels.NetsList;
using ASK.Model.NetsList;

namespace ASK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isExpanded;

        public MainWindow()
        {
            NetsListModel netsListModel = new NetsListModel();

            MainViewModel mainViewModel = new MainViewModel(netsListModel);
            InitializeComponent();
            isExpanded = false;
            DataContext = mainViewModel;
            netsList.DataContext = mainViewModel.NetsListViewModel;
            optionsControl.DataContext = mainViewModel.OptionsPanelViewModel;
            newProfileChoose.DataContext = mainViewModel.NetsListViewModel;

            netsList.OptionsControl = optionsControl; // let netsList know about optionsControl
        }

        private void closingButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateWindowPosition();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            isExpanded = false;
            updateWindowPosition();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            isExpanded = true;
            updateWindowPosition();
        }


        private void updateWindowPosition()
        {
            if (isExpanded)
            {
                Left = SystemParameters.PrimaryScreenWidth - Width;
            }
            else
            {
                Left = SystemParameters.PrimaryScreenWidth - 10;
            }
        }
        
        // dla tabów dolnej częsci
        const int PROFILE_OPTIONS_INDEX = 0;
        const int ADD_PROFILE_INDEX = 1;

        private void NewProfileClick(object sender, RoutedEventArgs e)
        {
            lowerPanel.SelectedIndex = ADD_PROFILE_INDEX;
        }

        private void InterfaceChooseClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var viewModel = button.DataContext as NetInterfaceViewModel;
            var netInterface = viewModel.NetInterfaceModel;
            netInterface.AddProfile(new ProfileModel("Nowy profil", netInterface));

            lowerPanel.SelectedIndex = PROFILE_OPTIONS_INDEX;
        }
    }
}
