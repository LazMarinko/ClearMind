using System.Collections.Generic;
using System.Windows;

namespace ClearMindUI
{
    public partial class ActiveDaysWindow : Window
    {
        private readonly ScheduleEntry _process;

        public ActiveDaysWindow(ScheduleEntry process)
        {
            InitializeComponent();
            _process = process;

            // Sets the toggle buttons to reflect the current active days of the process.
            MondayToggle.IsChecked = _process.ActiveDays.Contains("Mon");
            TuesdayToggle.IsChecked = _process.ActiveDays.Contains("Tue");
            WednesdayToggle.IsChecked = _process.ActiveDays.Contains("Wed");
            ThursdayToggle.IsChecked = _process.ActiveDays.Contains("Thu");
            FridayToggle.IsChecked = _process.ActiveDays.Contains("Fri");
            SaturdayToggle.IsChecked = _process.ActiveDays.Contains("Sat");
            SundayToggle.IsChecked = _process.ActiveDays.Contains("Sun");
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var days = new List<string>();

            // Checks each toggle button and adds the corresponding day to the list if it is checked.
            if (MondayToggle.IsChecked == true) days.Add("Mon");
            if (TuesdayToggle.IsChecked == true) days.Add("Tue");
            if (WednesdayToggle.IsChecked == true) days.Add("Wed");
            if (ThursdayToggle.IsChecked == true) days.Add("Thu");
            if (FridayToggle.IsChecked == true) days.Add("Fri");
            if (SaturdayToggle.IsChecked == true) days.Add("Sat");
            if (SundayToggle.IsChecked == true) days.Add("Sun");

            _process.ActiveDays = days;
            DialogResult = true; // Returns true to indicate to MainWindow that the user confirmed the changes.
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Returns false to indicate to MainWindow that the user canceled the changes.
        }
    }
}
