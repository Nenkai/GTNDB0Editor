//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/GTNDB0Editor
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System.Windows;

using GTNDB0Editor.Entities;

namespace GTNDB0Editor
{
    /// <summary>
    /// Interaction logic for BGMLTrackEditWindow.xaml
    /// </summary>
    public partial class NDB0EntryEditWindow : Window
    {
        private NDB0Entry _entryLong;
        private NDB0Entry _entryShort;
        public bool Saved { get; set; }
        public NDB0EntryEditWindow(NDB0Entry entryLong, NDB0Entry entryShort)
        {
            _entryLong = entryLong;
            _entryShort = entryShort;
            InitializeComponent();

            tb_LongName.Text = entryLong.FullName;
            tb_ShortName.Text = entryShort.FullName;

            tb_SpecID.Text = entryLong.SpecDBID.ToString();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!uint.TryParse(tb_SpecID.Text, out uint specID))
            {
                MessageBox.Show("SpecDB needs to be a number.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _entryLong.SpecDBID = specID;
            _entryShort.SpecDBID = specID;

            _entryLong.Name = tb_LongName.Text;
            _entryShort.Name = tb_ShortName.Text;
            
            Saved = true;
            Close();
        }
    }
}
