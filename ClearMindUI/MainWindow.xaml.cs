using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClearMindUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Regex DigitsOnly = new("^[0-9]+$");
        private bool _isNormalizing;
        private int startHourValue;
        private int startMinuteValue;
        private int endHourValue = 23;
        private int endMinuteValue = 59;
        private static int timeTextBoxIndex; // Used to know which is the current text box being edited
        private readonly ObservableCollection<LockedProcessEntry> _lockedProcesses = new(ProcessStore.Load()); // List that contains the locked processes and their schedules
        private ScheduleEntry? _selectedSchedule; // The currently selected schedule entry

        public MainWindow()
        {
            InitializeComponent();
            ProcessListBox.ItemsSource = _lockedProcesses;
        }

        // Handles the click of the add process button. Opens the AddProcessWindow and adds the new process to the list if confirmed.
        private void AddProcessButton_Click(object sender, RoutedEventArgs e)
        {
            var addProcessWindow = new AddProcessWindow { Owner = this };

            if (addProcessWindow.ShowDialog() != true) // Checks if the user pressed confirm if not exits the method   
                return;

            var newEntry = addProcessWindow.CreatedEntry; // Stores the newly created process entry from the AddProcessWindow

            _lockedProcesses.Add(newEntry); // Adds it to the observable collection which will automatically update the UI
            ProcessListBox.SelectedItem = newEntry; // Selects is the current selected item
            ProcessListBox.ScrollIntoView(newEntry); // Scrolls the list box to make sure the new entry is visible
            ProcessListBox.Focus(); // Sets the focus to the list box so that the user can see the new entry is selected
        }

        private void TimeSegment_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !DigitsOnly.IsMatch(e.Text);
        }


        // Handles the text changed event for the time segment text boxes. Moves focus to the next box when the max length is reached.
        private void TimeSegment_TextChanged(object sender, TextChangedEventArgs e)
        {

            // Checks if the method is normalizing and returns if it is.
            if (_isNormalizing)
                return;
            if (sender is TextBox box)
            {   // Checks which text box was changed and sets the timeTextBoxIndex accordingly. 
                switch (box.Name)
                {
                    case nameof(StartHourBox):
                        timeTextBoxIndex = 0;
                        break;
                    case nameof(StartMinuteBox):
                        timeTextBoxIndex = 1;
                        break;
                    case nameof(EndHourBox):
                        timeTextBoxIndex = 2;
                        break;
                    case nameof(EndMinuteBox):
                        timeTextBoxIndex = 3;
                        break;
                    default:
                        timeTextBoxIndex = -1;
                        break;
                } // Checks to see if we reached max length and if we are not in the last text box
                if (box.Text.Length == box.MaxLength && timeTextBoxIndex < 3)
                {
                    box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); // Snaps focus to the next text box
                }
                else if (timeTextBoxIndex == 3 && box.Text.Length == box.MaxLength) // Checks if we are in the last text box and reached max length
                {
                    Keyboard.ClearFocus(); // Clears the focus from the text box to prevent jumping to buttons
                }
                
            }
        }

        // Handles the lost focus event for the time segment text boxes
        private void TimeSegment_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox box)
                NormalizeTimeSegment(box); 
        }
        // Handles the key down event for the time segment text boxes and also handles the text box losing focus
        private void NormalizeTimeSegment(TextBox box)
        {
            _isNormalizing = true; // Sets the normalizing flag to true to prevent further changes while normalizing
            try
            {   // Checks if the user clicked on the text box and left it empty, if so fills it with the previous stored value
                if (string.IsNullOrEmpty(box.Text))
                {
                    switch (box.Name)
                    {
                        case nameof(EndHourBox):
                            box.Text = endHourValue.ToString("D2");
                            break;
                        case nameof(EndMinuteBox):
                            box.Text = endMinuteValue.ToString("D2");
                            break;
                        case nameof(StartHourBox):
                            box.Text = startHourValue.ToString("D2");
                            break;
                        case nameof(StartMinuteBox):
                            box.Text = startMinuteValue.ToString("D2");
                            break;
                        default:
                            box.Text = "00";
                            break;
                    }
                    return;
                }

                int max = (string)box.Tag == "Hour" ? 23 : 59; // Sets the maximum value for the box depending on its tag

                if (!int.TryParse(box.Text, out int value)) // Tries to parse the text from the box to intiger
                    value = 0;

                value = Math.Clamp(value, 0, max); // Ensures the value is within the valid range
                switch (box.Name) // Chekcs which box is being normalized and updates the corresponding stored value
                {
                    case nameof(EndHourBox):
                        endHourValue = value;
                        break;
                    case nameof(EndMinuteBox):
                        endMinuteValue = value;
                        break;
                    case nameof(StartHourBox):
                        startHourValue = value;
                        break;
                    case nameof(StartMinuteBox):
                        startMinuteValue = value;
                        break;
                }
                box.Text = value.ToString("D2");
            }
            finally
            {
                _isNormalizing = false; // Sets the normalizing flag to false to allow further changes
            }
        }

        // Handles the click event for the active days button.
        private void ActiveDaysButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSchedule is null) // Sanity check to ensure a schedule is selected before opening the active days window
                return;

            var activeDaysWindow = new ActiveDaysWindow(_selectedSchedule) // Creates a new instance of the ActiveDaysWindow and passes the selected schedule to it
            {
                Owner = this
            };

            if (activeDaysWindow.ShowDialog() == true) // Checks if the user pressed confirm in the ActiveDaysWindow
                ProcessListBox.Items.Refresh(); // Recfreshes the list box to show the updated data
        }

        // Handles the got focus event for the time segment text boxes
        private void TimeSegment_GotFocus(object sender, RoutedEventArgs e)
        {
            if(sender is TextBox box)
            {
                box.Text = ""; // Sets the text in the boxes to "" to allow the user to type new text
            }
        }


        // Handles the user clicking away from the time segment text boxes, but not on any element in the ui
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not DependencyObject source || FindAncestor<TextBox>(source) is not null) // Checks if the click was inside a text box or the clicked item cant be inspected
                return;

            if (Keyboard.FocusedElement is TextBox focusedBox) // Checks if the currently focused element is a text box
                NormalizeTimeSegment(focusedBox); // Normalizes the text box

            Keyboard.ClearFocus(); // Clears focus from the text box
        }

        // Generic helper that walks up the WPF visual tree and finds an ansestor of the specified type. Returns null if no ancestor is found.
        private static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current is not null)
            {
                if (current is T match)
                    return match;

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        private bool _isRevertingSelection; // Flag to check if we are switching back focus to the previous element that has unsaved changes


        // Handles the change of selected processes in the ProcessListBox
        private void ProcessListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRevertingSelection) // Sanity check to ensure when the user presses the Keep Editing button we don't go through the entire method just break it here
            {
                _isRevertingSelection = false;
                return;
            }

            if (e.AddedItems.Count > 0 // Checks if something new was selected
                && e.RemovedItems.Count > 0 // Checks if there was a previous item that is now deselected, not just a first selection
                && e.RemovedItems[0] is LockedProcessEntry previousProcess // Checks if the item removed from selection is a LockedProcessEntry
                && _selectedSchedule is not null // Checks if theres a schedule actually loaded to check for edits
                && HasUnsavedTimeChanges(_selectedSchedule)) // Checks if there are unsaved edits
            {   
                // Creates a CustomMessageBoxWindow to ask the user do they want to Keep Editing or discard their changes and stores the answer as a bool
                bool discard = CustomMessageBoxWindow.Confirm( 
                    this,
                    $"You have unsaved time changes for \"{previousProcess.Name}\". Switch anyway and discard them?",
                    "Unsaved Changes",
                    "Discard",
                    "Keep Editing");

                if (!discard) // Checks if the user clicked Keep Editing, and reverts the selection if they did
                {
                    _isRevertingSelection = true;
                    ProcessListBox.SelectedItem = previousProcess;
                    return;
                }
            }

            RemoveButton.IsEnabled = true;

            if (ProcessListBox.SelectedItem is not LockedProcessEntry selectedProcess) // Sanity check to ensure the selected proces is a LockedProcesEntry
            {
                ClearSelectedSchedule();
                return;
            }

            if (selectedProcess.IsComplex) // Check if the process is complex, if it is we don't select any schedule and just clear the selected schedule
                ClearSelectedSchedule();
            else // If the process is not complex we select the first schedule of the process
                SelectSchedule(selectedProcess.Schedules[0]);
        }

        // Used to the enable the buttons when a schedule is selected and to set the time segment text boxes to the selected schedule values
        private void SelectSchedule(ScheduleEntry schedule)
        {
            if (_selectedSchedule is not null) // Checks if there is a previously selected schedule and deselects it
                _selectedSchedule.IsSelected = false;

            schedule.IsSelected = true; // Sets the bool in the newly selected schedule to true
            _selectedSchedule = schedule; // Updates the reference to the newly selected schedule

            // Updates the stored values to the newly selected schedule values
            startHourValue = schedule.StartTimeHour;
            startMinuteValue = schedule.StartTimeMin;
            endHourValue = schedule.EndTimeHour;
            endMinuteValue = schedule.EndTimeMin;

            _isNormalizing = true; // Sets the normalizing flag to true to prevent further changes while updating the text boxes
            try
            {   // Updates the text boxes to the newly selected schedule values
                StartHourBox.Text = startHourValue.ToString("D2");
                StartMinuteBox.Text = startMinuteValue.ToString("D2");
                EndHourBox.Text = endHourValue.ToString("D2");
                EndMinuteBox.Text = endMinuteValue.ToString("D2");
            }
            finally
            {
                _isNormalizing = false; // Sets the normalizing flag to false to allow further changes
            }

            // Enables the buttons and time range panel now that a schedule is selected
            DaysButton.IsEnabled = true;
            TimeRangePanel.IsEnabled = true;
            SaveButton.IsEnabled = true;

            ProcessListBox.Items.Refresh(); // Refreshes the list box to show the updated data
        }

        // Used to deselect the currently selected schedule and disable the buttons and time range panel
        private void ClearSelectedSchedule()
        {
            if (_selectedSchedule is not null)
                _selectedSchedule.IsSelected = false;

            _selectedSchedule = null;
            DaysButton.IsEnabled = false;
            TimeRangePanel.IsEnabled = false;
            SaveButton.IsEnabled = false;

            ProcessListBox.Items.Refresh();
        }

        // Checks for unsaved changes in the time segment text boxes compared to the currently selected schedule values
        private bool HasUnsavedTimeChanges(ScheduleEntry schedule)
        {
            return startHourValue != schedule.StartTimeHour
                || startMinuteValue != schedule.StartTimeMin
                || endHourValue != schedule.EndTimeHour
                || endMinuteValue != schedule.EndTimeMin;
        }

        // Checks whether the given candidate time range would overlap another schedule on the same process on a shared active day
        private static bool HasConflictingSchedule(LockedProcessEntry process, ScheduleEntry editedSchedule, int startHour, int startMin, int endHour, int endMin)
        {
            foreach (var other in process.Schedules)
            {
                if (ReferenceEquals(other, editedSchedule)) // Skips comparing the schedule against itself
                    continue;

                if (!editedSchedule.ActiveDays.Any(day => other.ActiveDays.Contains(day))) // No shared active day means no possible conflict
                    continue;

                if (TimeRangesOverlap(startHour, startMin, endHour, endMin, other.StartTimeHour, other.StartTimeMin, other.EndTimeHour, other.EndTimeMin))
                    return true;
            }

            return false;
        }

        // Checks whether two [start, end] time ranges (expressed as hour/minute pairs) overlap
        private static bool TimeRangesOverlap(int startHour1, int startMin1, int endHour1, int endMin1, int startHour2, int startMin2, int endHour2, int endMin2)
        {
            int start1 = startHour1 * 60 + startMin1;
            int end1 = endHour1 * 60 + endMin1;
            int start2 = startHour2 * 60 + startMin2;
            int end2 = endHour2 * 60 + endMin2;

            return start1 <= end2 && start2 <= end1;
        }


        // Handles the click event for the complex toggle button. Adds a new schedule to the process if it is not complex, otherwise shows a message box.
        private void ComplexToggle_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not LockedProcessEntry process) // Sanity check to ensure the sender is a LockedProcessEntry
                return;

            if (process.IsComplex) // Checks if the process is already complex, if it is we show a message box and return to stop the user from making invalid data in the json
            {
                CustomMessageBoxWindow.Show(
                    this,
                    $"\"{process.Name}\" has multiple schedules. Remove schedules down to one before turning off complex mode.",
                    "Cannot Disable Complex Mode");
                return;
            }

            process.Schedules.Add(new ScheduleEntry());
            process.IsExpanded = true;
            ProcessStore.Save(process);
            ProcessListBox.Items.Refresh();
        }

        // Used to expand or collapse the schedules of a complex process when the user clicks on the expand/collapse button.
        private void ExpandToggle_Click(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not LockedProcessEntry process || !process.IsComplex) // Sanity check to ensure the sender is a LockedProcessEntry and that it is complex
                return;

            process.IsExpanded = !process.IsExpanded;
            ProcessListBox.Items.Refresh();
        }

        // Handles the Selection of schedules inside of a complex process entry
        private void ScheduleRow_Click(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not ScheduleEntry schedule) // Sanity check to ensure the sender is a ScheduleEntry
                return;

            var process = _lockedProcesses.FirstOrDefault(p => p.Schedules.Contains(schedule));  // Fetches the schedule's parent process from the list of locked processes
            if (process is null || !ReferenceEquals(ProcessListBox.SelectedItem, process)) // Checks if the process exists and if the selected item differs from the process who is the parent
                return;

            SelectSchedule(schedule);
        }


        // Handles the click of the X in the schedule sof a complex process
        private void RemoveSchedule_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Handles the click of the X so it doesnt call ScheduleRow_Click when clicked

            if ((sender as FrameworkElement)?.DataContext is not ScheduleEntry schedule) // Sanity check to ensure the element is a schedule
                return;

            var process = _lockedProcesses.FirstOrDefault(p => p.Schedules.Contains(schedule)); // Fetches the parent process of the schedule to be deleted
            if (process is null || process.Schedules.Count <= 1) // Sanity check to ensure the process exists and has more than 1 schedule
                return;

            bool wasSelected = ReferenceEquals(_selectedSchedule, schedule); // Flag to check if the schedule thats being deleted is also the currently selected schedule
            process.Schedules.Remove(schedule);

            if (wasSelected)
            {
                if (process.Schedules.Count == 1) // Checks if the schedule that was deleted was the only other schedule for the process if so, the process stops being complex
                    SelectSchedule(process.Schedules[0]);
                else
                    ClearSelectedSchedule(); // Clears the selection if not
            }

            ProcessStore.Save(process); // Updates the current process
            ProcessListBox.Items.Refresh();
        }


        // Handles the click of the AddSubEntry button
        private void AddSubEntry_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not LockedProcessEntry process) // Sanity check to ensure the sender is a process 
                return;

            var addSubEntryWindow = new AddSubEntryWindow(process) { Owner = this }; // Creates a SubEntryWindow and stores the result once its done

            if (addSubEntryWindow.ShowDialog() == true) // Checks if the user actually created a new schedule
                ProcessListBox.Items.Refresh();
        }

        // Handles the click of the SaveButton
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Sanity check to ensure that the selected process is a LockedProcessEntry and that there is a selected schedule
            if (ProcessListBox.SelectedItem is not LockedProcessEntry selectedProcess || _selectedSchedule is null) 
                return;

            // Sanity check to ensure that the user didn't specify an impossible time period
            if(startHourValue > endHourValue || (startHourValue == endHourValue && startMinuteValue > endMinuteValue))
            {
                CustomMessageBoxWindow.Show(this, "Starting time is greater than end time. Please enter a valid time period.");
                return;
            }

            // Sanity check to ensure the new time range doesn't conflict with another schedule on the same process, so the python script never reads overlapping data
            if (HasConflictingSchedule(selectedProcess, _selectedSchedule, startHourValue, startMinuteValue, endHourValue, endMinuteValue))
            {
                CustomMessageBoxWindow.Show(this, "This time period overlaps with another schedule on a shared day. Please choose a different time period or days.");
                return;
            }

            // Save the current time value to the selected process
            _selectedSchedule.StartTimeHour = startHourValue;
            _selectedSchedule.StartTimeMin = startMinuteValue;
            _selectedSchedule.EndTimeHour = endHourValue;
            _selectedSchedule.EndTimeMin = endMinuteValue;

            ProcessStore.Save(selectedProcess);

            ProcessListBox.Items.Refresh();
        }

        // Handles the click of the RemoveButton
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessListBox.SelectedItem is not LockedProcessEntry selectedProcess) // Sanity check to ensure the SelectedItem is a LockedProcessEntry
                return;

            // Deletes the process from memory and from the screen  
            ProcessStore.Remove(selectedProcess);
            _lockedProcesses.Remove(selectedProcess);

            // Clears the selection and resets the buttons.
            ClearSelectedSchedule();
            RemoveButton.IsEnabled = false;
        }
    }
}