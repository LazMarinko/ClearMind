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
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
