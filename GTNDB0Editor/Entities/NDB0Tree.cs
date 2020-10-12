using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTNDB0Editor.Utils;
namespace GTNDB0Editor.Entities
{
    public class NDB0Tree
    {
        public List<NDB0Entry> Entries { get; set; } = new List<NDB0Entry>();

        public OptimizedStringTable GetStringTable()
        {
            var table = new OptimizedStringTable();
            table.Alignment = 0x04;

            foreach (var name in Entries)
                table.AddString(name.FullName);

            return table;
        }

        public void Add(NDB0Entry entry)
        {
            entry.FullName = entry.ToString();
            Entries.Add(entry);
            Resort();
        }

        public void Remove(NDB0Entry entry)
        {
            Entries.Remove(entry);
            Resort();
        }

        private void Resort()
        {
            var ordered = Entries.OrderBy(e => e.FullName).ToList();
            for (int i = 0; i < ordered.Count; i++)
                ordered[i].AlphabeticalID = (uint)i;
        }
    }
}
