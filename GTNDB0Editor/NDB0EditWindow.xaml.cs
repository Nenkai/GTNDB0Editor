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
using System.Windows.Shapes;

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
