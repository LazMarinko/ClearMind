using System.Windows;

namespace ClearMindUI
{
    public partial class AddProcessWindow : Window
    {
        public LockedProcessEntry CreatedEntry { get; private set; } = new();

        public AddProcessWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProcessNameBox.Text))
                return;

            CreatedEntry = new LockedProcessEntry
            {
                Name = ProcessNameBox.Text.Trim()
            };

            ProcessStore.Save(CreatedEntry); 

            DialogResult = true; // Closes the window and returns true to tell MainWindow that a new process was added successfully
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Closes the window and returns false to tell MainWindow that the user canceled the operation
        }
    }
}
