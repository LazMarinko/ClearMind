using System.Configuration;
using System.Data;
using System.Windows;

namespace ClearMindUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void TitleBar_MinimizeClick(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject d && Window.GetWindow(d) is Window window)
                window.WindowState = WindowState.Minimized;
        }

        private void TitleBar_MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject d && Window.GetWindow(d) is Window window)
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
        }

        private void TitleBar_CloseClick(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject d && Window.GetWindow(d) is Window window)
                window.Close();
        }
    }

}
