using System;

namespace DiffPatch
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
