//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/GTNDB0Editor
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNDB0Editor.Entities
{
    public class NDB0Prefix
    {
        public int Index { get; set; }
        public string Name { get; set; }

        public NDB0Prefix(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
}
