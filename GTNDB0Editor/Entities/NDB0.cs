using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;
using GTNDB0Editor.Utils;

namespace GTNDB0Editor.Entities
{
    public class NDB0
    {
        public const string MAGIC = "NDB0";

        public List<NDB0Prefix> Prefixes { get; private set; }
        public List<NDB0Tree> NameTrees { get; private set; } = new List<NDB0Tree>();

        public static NDB0 ReadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (var fs = new FileStream(path, FileMode.Open))
            using (var bs = new BinaryStream(fs, ByteConverter.Big))
            {
                if (bs.ReadString(4) != "NDB0")
                {
                    return null;
                }

                var ndb = new NDB0();

                bs.Position += 4;
                uint prefixTreeOffset = bs.ReadUInt32();
                bs.Position = (int)prefixTreeOffset;

                int prefCount = bs.ReadInt32();
                ndb.Prefixes = new List<NDB0Prefix>(prefCount);
                for (int i = 0; i < prefCount; i++)
                {
                    int prefixNameOffset = (int)((prefixTreeOffset + 4) + (i * sizeof(uint)));
                    bs.Position = prefixNameOffset;
                    bs.Position = (int)bs.ReadUInt32();
                    ndb.Prefixes.Add(new NDB0Prefix(i, bs.ReadString(StringCoding.ZeroTerminated)));
                }

                bs.Position = 0x0C;
                int treeCount = bs.ReadInt32();
                for (int i = 0; i < treeCount; i++)
                {
                    bs.Position = 0x10 + (sizeof(int) * i);
                    uint mapOffset = bs.ReadUInt32();
                    bs.Position = (int)mapOffset;

                    ndb.NameTrees.Add(ndb.DeserializeMapNames(bs));
                }
                return ndb;
            }
        }
        
        private NDB0Tree DeserializeMapNames(BinaryStream bs)
        {
            uint unk = bs.ReadUInt32(); // 100
            uint nameCount = bs.ReadUInt32();
            bs.Position += 8; // Skip 5E 5E 5E 5E 5E 5E 5E 5E

            int basePos = (int)bs.Position;

            NDB0Tree entries = new NDB0Tree();
            for (int i = 0; i < nameCount; i++)
            {
                bs.Position = basePos + (i * 0x10);

                var entry = new NDB0Entry();
                entry.SpecDBID = bs.ReadUInt32();
                entry.AlphabeticalID = bs.ReadUInt32();
                uint nameOffset = bs.ReadUInt32();

                int temp = (int)bs.Position;
                bs.Position = (int)nameOffset;
                entry.Name = bs.ReadString(StringCoding.ZeroTerminated);
                bs.Position = temp;

                for (int j = 0; j < 4; j++)
                {
                    int prefixIndex = bs.ReadByte();
                    if (prefixIndex != byte.MaxValue)
                        entry.Prefixes.Add(Prefixes[prefixIndex].Name);
                }

                entry.FullName = entry.ToString();
                entries.Entries.Add(entry);
            }

            return entries;
        }

        public void Save(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            using (var bs = new BinaryStream(fs, ByteConverter.Big))
            {
                bs.WriteString(MAGIC, StringCoding.Raw);
                bs.Position += 4;
                bs.Position += 4; // Skip prefixes offset for now
                bs.WriteInt32(NameTrees.Count);

                bs.Position += NameTrees.Count * sizeof(uint);
                bs.AlignWithValue(0x10, 0x5E);

                long lastPos = bs.Position;
                for (int i = 0; i < NameTrees.Count; i++)
                {
                    var currentTree = NameTrees[i];
                    bs.Position = lastPos;
                    int baseTreePos = (int)bs.Position;
                    // For now skip everything and go to the string tables
                    bs.Position += 0x10;
                    bs.Position += 0x10 * currentTree.Entries.Count;

                    OptimizedStringTable opt = NameTrees[i].GetStringTable();
                    opt.SaveStream(bs);
                    bs.AlignWithValue(0x10, 0x5E);

                    lastPos = bs.Position;

                    // Write where the tree is located
                    bs.Position = 0x10 + (i * sizeof(uint));
                    bs.WriteInt32(baseTreePos);

                    // Write its data
                    bs.Position = baseTreePos;
                    bs.WriteInt32(100);
                    bs.WriteInt32(currentTree.Entries.Count);
                    bs.AlignWithValue(0x10, 0x5E);
                    for (int j = 0; j < currentTree.Entries.Count; j++)
                    {
                        var entry = currentTree.Entries[j];
                        bs.WriteUInt32(entry.SpecDBID);
                        bs.WriteUInt32(entry.AlphabeticalID);
                        bs.WriteInt32(opt.GetStringOffset(entry.FullName));
                        bs.WriteBytes(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                    }
                }

                // Write the prefix tree
                bs.Position = lastPos;
                bs.WriteInt32(Prefixes.Count); // 4 byte header

                // Skip entries
                bs.Position += Prefixes.Count * sizeof(uint);
                OptimizedStringTable prefixST = GetPrefixStringTable();
                prefixST.SaveStream(bs);
                bs.Align(0x10, true);

                // String table
                bs.Position = lastPos + 4;
                foreach (var prefix in Prefixes)
                    bs.WriteInt32(prefixST.GetStringOffset(prefix.Name));

                // Write prefix tree offset
                bs.Position = 0x08;
                bs.WriteInt32((int)lastPos);
            }
        }

        public OptimizedStringTable GetPrefixStringTable()
        {
            var opt = new OptimizedStringTable();
            opt.Alignment = 0x04;
            foreach (var prefix in Prefixes)
                opt.AddString(prefix.Name);
            return opt;
        }

        public void Print(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine($"Prefix Count: {Prefixes.Count}");
                for (int i = 0; i < Prefixes.Count; i++)
                    sw.WriteLine($"[{i}] {Prefixes[i].Name}");
                sw.WriteLine();

                sw.WriteLine($"Names Map 1 Count: {NameTrees[0].Entries.Count}");
                for (int i = 0; i < NameTrees[0].Entries.Count; i++)
                {
                    NDB0Entry entry = NameTrees[0].Entries[i];
                    sw.WriteLine($"[{i}] {entry.FullName,-75} > ID: {entry.SpecDBID}, ID2: {entry.AlphabeticalID}, Name: {entry.Name}, {entry.Prefixes.Count} Prefixes {(entry.Prefixes.Count > 0 ? $"({string.Join(", ", entry.Prefixes)})" : "")}");
                }

                sw.WriteLine();

                sw.WriteLine($"Names Map 2 Count: {NameTrees[1].Entries.Count}");
                for (int i = 0; i < NameTrees[1].Entries.Count; i++)
                {
                    NDB0Entry entry = NameTrees[1].Entries[i];
                    sw.WriteLine($"[{i}] {entry.FullName,-75} > SpecDBID: {entry.SpecDBID}, AlphabeticalID: {entry.AlphabeticalID}, Name: {entry.Name}, {entry.Prefixes.Count} Prefixes {(entry.Prefixes.Count > 0 ? $"({string.Join(", ", entry.Prefixes)})" : "")}");
                }

                sw.Flush();
            }
        }

    }

    public class NDB0Entry
    {
        public uint SpecDBID { get; set; }
        public uint AlphabeticalID { get; set; }
        public string Name { get; set; }

        public string FullName { get; set; }

        public List<string> Prefixes = new List<string>();

        public override string ToString()
        {
            if (Prefixes.Count != 0)
                return $"{string.Join(" ", Prefixes)} {Name}";
            else
                return Name;
        }
    }

}
