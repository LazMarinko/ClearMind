using ClearMindUI.Core;
using System.Windows;

namespace ClearMindUI
{
    public partial class SettingsWindow : Window
    {

        private Config config = ConfigStore.Load();
        public SettingsWindow()
        {
            InitializeComponent();
            EditsToggle_Switch.IsChecked = config.allowEdits;

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EditsToggle_Checked(object sender, RoutedEventArgs e)
        {
            config.allowEdits = true;
            ConfigStore.Save(config);
        }

        private void EditsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            config.allowEdits = false;
            ConfigStore.Save(config);
        }
    }
}
