using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace ASK.GUI
{
    public class ThemeProperties : StaticExtension
    {
        public static Brush GetBackgroundColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(BackgroundColorProperty);
        }

        public static void SetBackgroundColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(BackgroundColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.RegisterAttached(
                "BackgroundColor",
                typeof(Brush),
                typeof(ThemeProperties),
                new FrameworkPropertyMetadata(Brushes.Black));

        public static String Hello
        {
            get { return "Hello world"; }
            set { }
        }
    }
}
