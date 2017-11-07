using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WinFolderList.Controls
{
    public class ScrollViewEx
    {

        public static readonly DependencyProperty AutoScrollProperty = DependencyProperty.RegisterAttached(
            "AutoScroll", typeof(bool), typeof(ScrollViewEx), new PropertyMetadata(default(bool), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var scrollViewer = dependencyObject as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += (sender, args) =>
                {
                    var sv = args.Source as ScrollViewer;
                    if (sv != null)
                    {
                        var manualScroll = args.ExtentHeightChange == 0;
                        // var scrolledToBottom = sv.VerticalOffset == sv.ScrollableHeight);
                        var wasScrolledToBottom = sv.ScrollableHeight - sv.VerticalOffset <= args.ExtentHeightChange + 1;  // added 1 just as a bit of a buffer (incase user was almost scrolled to bottom)

                        if (!manualScroll && wasScrolledToBottom)
                        {
                            sv.ScrollToVerticalOffset(sv.ExtentHeight);
                        }
                    }

                };
            }
        }

        public static void SetAutoScroll(DependencyObject element, bool value)
        {
            element.SetValue(AutoScrollProperty, value);
        }

        public static bool GetAutoScroll(DependencyObject element)
        {
            return (bool)element.GetValue(AutoScrollProperty);
        }
    }
}
