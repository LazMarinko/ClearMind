using System.Windows;

namespace ClearMindUI
{
    public partial class CustomMessageBoxWindow : Window
    {
        private CustomMessageBoxWindow(string message, string title, string okText, string? cancelText)
        {
            InitializeComponent();
            Title = title;
            MessageText.Text = message;
            OkButton.Content = okText;

            if (cancelText is not null)
            {
                CancelButton.Content = cancelText;
                CancelButton.Visibility = Visibility.Visible;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public static void Show(Window owner, string message, string title = "Notice")
        {
            var messageBox = new CustomMessageBoxWindow(message, title, "OK", null) { Owner = owner };
            messageBox.ShowDialog();
        }

        public static bool Confirm(Window owner, string message, string title = "Confirm", string okText = "Yes", string cancelText = "Cancel")
        {
            var messageBox = new CustomMessageBoxWindow(message, title, okText, cancelText) { Owner = owner };
            return messageBox.ShowDialog() == true;
        }
    }
}
