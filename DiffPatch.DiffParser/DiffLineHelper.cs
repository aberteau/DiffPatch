using System;
using System.Collections.Generic;
using System.Text;

namespace DiffPatch.DiffParser
{
    class DiffLineHelper
    {
        public static String GetContent(String line)
        {
            string content = line.Substring(1);
            return content;
        }
    }
}
