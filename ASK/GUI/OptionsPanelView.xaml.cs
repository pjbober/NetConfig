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
using ASK.ViewModels.OptionsControl;

namespace ASK.GUI
{
    /// <summary>
    /// Interaction logic for OptionsControl.xaml
    /// </summary>
    public partial class OptionsPanel : UserControl
    {
        public OptionsPanel()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as OptionsPanelViewModel).SaveProfile();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as OptionsPanelViewModel).SetProfile(null);
        }
    }
}
