//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/GTNDB0Editor
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using Syroot.BinaryData;

namespace GTNDB0Editor.Utils
{
    public static class MiscExtensions
    {
        public static void AlignWithValue(this BinaryStream bs, int alignment, byte val)
        {
            long pos = bs.Position;
            bs.Align(alignment, true);
            long endPos = bs.Position;
            bs.Position = pos;
            for (int i = 0; i < endPos - pos; i++)
                bs.WriteByte(val);
        }
    }
}
