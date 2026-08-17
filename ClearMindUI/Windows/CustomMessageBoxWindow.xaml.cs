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
            DialogResult = true; // Returns true to indicate to the calling code that the user clicked "OK".
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Returns false to indicate to the calling code that the user clicked "Cancel".
        }

        public static void Show(Window owner, string message, string title = "Notice")
        {
            var messageBox = new CustomMessageBoxWindow(message, title, "OK", null) { Owner = owner };
            messageBox.ShowDialog();
        }

        // Used only in the case of a user making changes and attempting to select a new process without saving the changes.
        public static bool Confirm(Window owner, string message, string title = "Confirm", string okText = "Yes", string cancelText = "Cancel")
        {
            var messageBox = new CustomMessageBoxWindow(message, title, okText, cancelText) { Owner = owner };
            return messageBox.ShowDialog() == true;
        }
    }
}
