﻿using System;
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

namespace ASK.GUI
{
    /// <summary>
    /// Interaction logic for NetsList.xaml
    /// </summary>
    public partial class NetsList : UserControl
    {
        public NetsList()
        {
            InitializeComponent();
            DataContext = new DummyNetsListModel();
        }
    }
}