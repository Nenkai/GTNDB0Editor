using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using GTNDB0Editor.Entities;

namespace GTNDB0Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NDB0 Database { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenLib_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Gran Turismo Car Name Database (*.dat)|*.dat";
            openDialog.CheckFileExists = true;
            openDialog.CheckPathExists = true;

            if (openDialog.ShowDialog() == true)
            {
                NDB0 ndb;
                try
                {
                    ndb = NDB0.ReadFromFile(openDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occured while loading file: {ex.Message}", "A not so friendly prompt", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                Database = ndb;

                UpdateNamesList();
                UpdatePrefixesList();

                menuItem_Save.IsEnabled = true;
                menuItem_Dump.IsEnabled = true;

            }
        }

        private void SaveLib_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Gran Turismo Car Name Database (*.dat)|*.dat";

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    Database.Save(saveDialog.FileName);
                    MessageBox.Show($"Database file saved successfuly as {saveDialog.FileName} ({Database.NameTrees[0].Entries.Count} entries)", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occured while saving file: {ex.Message}", "A not so friendly prompt", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void Dump_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Log file(*.log)|*.log";

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    Database.Print(saveDialog.FileName);
                    MessageBox.Show($"Log file saved successfuly as {saveDialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occured while saving file: {ex.Message}", "A not so friendly prompt", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void lvContextTrack_AddNew(object sender, RoutedEventArgs e)
        {
            if (Database is null)
                return;

            var entryL = new NDB0Entry();
            var entryS = new NDB0Entry();
            var dialog = new NDB0EntryEditWindow(entryL, entryS);
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                Database.NameTrees[0].Add(entryL);
                Database.NameTrees[1].Add(entryS);

                UpdateNamesList();
            }
        }

        private void lvContextLongName_Edit(object sender, RoutedEventArgs e)
        {
            if (Database is null || lvCarNames.SelectedIndex == -1)
                return;

            var entryLong = Database.NameTrees[0].Entries[lvCarNames.SelectedIndex];
            var entryShort = Database.NameTrees[1].Entries[lvCarNames.SelectedIndex];
            var dialog = new NDB0EntryEditWindow(entryLong, entryShort);
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                entryLong.FullName = entryLong.Name;
                entryShort.FullName = entryShort.Name;
                UpdateNamesList();
            }
        }


        private void lvContextShortName_Edit(object sender, RoutedEventArgs e)
        {
            if (Database is null || lvShortCarNames.SelectedIndex == -1)
                return;

            var entryLong = Database.NameTrees[0].Entries[lvShortCarNames.SelectedIndex];
            var entryShort = Database.NameTrees[1].Entries[lvShortCarNames.SelectedIndex];
            var dialog = new NDB0EntryEditWindow(entryLong, entryShort);
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                entryLong.FullName = entryLong.Name;
                entryShort.FullName = entryShort.Name;
                UpdateNamesList();
            }
        }

        private void lvContextLongName_Remove(object sender, RoutedEventArgs e)
        {
            if (Database is null || lvCarNames.SelectedIndex == -1)
                return;

            var entry = (NDB0Entry)lvCarNames.SelectedItem;
            var result = MessageBox.Show($"Are you sure that you want to remove \"{entry.ToString()}\"?", "A friendly prompt", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Database.NameTrees[0].Remove(entry);
                Database.NameTrees[1].Remove(entry);
                UpdateNamesList();
            }
        }

        private void lvContextShortName_Remove(object sender, RoutedEventArgs e)
        {
            if (Database is null || lvShortCarNames.SelectedIndex == -1)
                return;

            var entry = (NDB0Entry)lvShortCarNames.SelectedItem;
            var result = MessageBox.Show($"Are you sure that you want to remove \"{entry.ToString()}\"?", "A friendly prompt", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Database.NameTrees[0].Remove(entry);
                Database.NameTrees[1].Remove(entry);
                UpdateNamesList();
            }
        }

        public void UpdateNamesList()
        {
            lvCarNames.ItemsSource = Database.NameTrees[0].Entries;
            lvCarNames.Items.Refresh();

            lvShortCarNames.ItemsSource = Database.NameTrees[1].Entries;
            lvShortCarNames.Items.Refresh();
        }

        public void UpdatePrefixesList()
        {
            lvPrefixes.ItemsSource = Database.Prefixes;
            lvPrefixes.Items.Refresh();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Credits:\n" +
                "- Nenkai#9075 - GT Car Name DB Editor - Creator & Research\n", "About", MessageBoxButton.OK);
        }
    }
}
