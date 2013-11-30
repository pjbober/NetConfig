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
using ASK.Model.NetsList;

namespace ASK.GUI
{
    /// <summary>
    /// Interaction logic for NetsList.xaml
    /// </summary>
    public partial class NetsList : UserControl
    {
        //public static readonly DependencyProperty OptionsControlProperty = DependencyProperty.Register
        //    (
        //         "OptionsControl", 
        //         typeof(OptionsControl), 
        //         typeof(NetsList), 
        //         new PropertyMetadata(string.Empty)
        //    );

        private OptionsControl _optionsControl;

        //public OptionsControl OptionsControl
        //{
        //    get { return (OptionsControl)GetValue(OptionsControlProperty); }
        //    set { SetValue(OptionsControlProperty, value); }
        //}

        public OptionsControl OptionsControl
        {
            get { return _optionsControl; }
            set { _optionsControl = value; }
        }

        public NetsList()
        {
            InitializeComponent();
        }

        private void ClickProfileChoose(object sender, RoutedEventArgs e)
        {
            //Profile profile = ((Button)sender);
            //Console.Out.WriteLine(profile.ProfileName);
            //this.OptionsControl.DataContext = profile;
        }
    }
}
