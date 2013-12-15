using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ASK.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void EmitPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public static Style GetStyle(String name)
        {
            Style s = Application.Current.Resources[name] as Style;
            return s;
        }

        public static SolidColorBrush GetBrush(String name)
        {
            var res = Application.Current.Resources[name];
            SolidColorBrush s = res as SolidColorBrush;
            return s;
        }


    }

}
