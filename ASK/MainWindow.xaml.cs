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
using ASK.ViewModels.OptionsControl;

namespace ASK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isExpanded;

        public static OptionsPanelViewModel OptionsPanelViewModel {
            get { return (((Application.Current.MainWindow as MainWindow).OptionsPanel.DataContext) as OptionsPanelViewModel); }
        }

        public MainWindow()
        {
            NetsListModel netsListModel = new NetsListModel();

            MainViewModel mainViewModel = new MainViewModel(netsListModel);

            InitializeComponent();
            isExpanded = false;
            DataContext = mainViewModel;
            NetsList.DataContext = mainViewModel.NetsListViewModel;
            OptionsPanel.DataContext = mainViewModel.OptionsPanelViewModel;

            NetsList.OptionsControl = OptionsPanel; // let netsList know about optionsControl

            //// tray TODO

            //System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            //ni.Icon = new System.Drawing.Icon("Main.ico");
            //ni.Visible = true;
            //ni.DoubleClick +=
            //    delegate(object sender, EventArgs args)
            //    {
            //        this.Show();
            //        this.WindowState = WindowState.Normal;
            //    };
        }

        private void SetPanelVisible(bool visible)
        {
            isExpanded = visible;
            updateWindowPosition();
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
            SetPanelVisible(false);
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            SetPanelVisible(true);
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

        private void hideButton_Click(object sender, RoutedEventArgs e)
        {
            SetPanelVisible(false);
        }
    }
}
