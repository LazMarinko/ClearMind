using System.Windows;

namespace ClearMindUI
{
    public partial class AddSubEntryWindow : Window
    {
        private readonly LockedProcessEntry _process;

        public ScheduleEntry CreatedSchedule { get; private set; } = new();

        public AddSubEntryWindow(LockedProcessEntry process)
        {
            InitializeComponent();
            _process = process;
            MessageText.Text = $"Add a new schedule to \"{process.Name}\"?";
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            CreatedSchedule = new ScheduleEntry();
            _process.Schedules.Add(CreatedSchedule);
            ProcessStore.Save(_process);
            DialogResult = true; // Returns true to indicate to MainWindows that a new schedule has been created and added to the process.
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Returns false to indicate to MainWindows that the user canceled the operation.
        }
    }
}
