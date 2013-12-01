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
            MainModel model = new MainModel();
            InitializeComponent();
            isExpanded = false;
            DataContext = model;
            netsList.DataContext = model.NetsList;
            optionsControl.DataContext = model.OptionsControl;

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
                Left = SystemParameters.PrimaryScreenWidth - Width;
            else
                Left = SystemParameters.PrimaryScreenWidth - 10;
        }

        private void newConfigButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Not yet implemented");
        }
    }
}
