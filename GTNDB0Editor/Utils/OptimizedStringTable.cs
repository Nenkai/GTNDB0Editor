using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace GTNDB0Editor.Utils
{
    /// <summary>
    /// Represents a binary string table that Polyphony Digital uses to save space within files.
    /// </summary>
    public class OptimizedStringTable
    {
        /// <summary>
        /// Position within the string table
        /// </summary>
        public int _currentPos;

        private int _alignment = -1;
        /// <summary>
        /// Gets or sets the string alignment. No alignment by default.
        /// </summary>
        public int Alignment
        {
            get => _alignment;
            set
            {
                if (value % 2 != 0)
                    throw new Exception("Alignment must be a power of two");
                _alignment = value;
            }
        }

        /// <summary>
        /// Whether if the strings should be null terminated. On by default.
        /// </summary>
        public bool NullTerminated { get; set; } = true;

        public Dictionary<string, int> StringMeta = new Dictionary<string, int>();


        /// <summary>
        /// Adds a string to the string table.
        /// </summary>
        public void AddString(string str)
        {
            if (!StringMeta.ContainsKey(str))
            {
                StringMeta.Add(str, _currentPos);
                _currentPos += Encoding.UTF8.GetByteCount(str);

                if (NullTerminated)
                    _currentPos++;

                if (_alignment != -1)
                {
                    var newPos = (-_currentPos % _alignment + _alignment) % _alignment;
                    _currentPos += newPos;
                }
            }
        }

        /// <summary>
        /// Saves the string table into a main stream.
        /// This updates the underlaying table holding the offsets to match the main stream.
        /// </summary>
        public void SaveStream(BinaryStream bs)
        {
            int basePos = (int)bs.Position;
            foreach (var strEntry in StringMeta)
            {
                if (NullTerminated)
                    bs.WriteString(strEntry.Key, StringCoding.ZeroTerminated, Encoding.UTF8);
                else
                    bs.WriteString(strEntry.Key, StringCoding.Raw, Encoding.UTF8);

                bs.Align(_alignment);
            }

            // Update the offsets - kinda inefficient way to do it
            foreach (var strEntry in StringMeta.Keys.ToList())
                StringMeta[strEntry] += basePos;
        }

        /// <summary>
        /// Gets the offset of a string within the binary table.
        /// Save stream should've already been called.
        /// </summary>
        public int GetStringOffset(string str)
        {
            return StringMeta[str];
        }
    }
}
