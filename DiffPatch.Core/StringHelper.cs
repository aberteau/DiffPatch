using System;
using System.Collections.Generic;
using System.Linq;

namespace DiffPatch.Core
{
    public static class StringHelper
    {
        public static IEnumerable<String> SplitLines(string input, string lineEnding)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Enumerable.Empty<String>();

            string[] lines = input.Split(new[] { lineEnding }, StringSplitOptions.None);
            return lines.Length == 0 ? Enumerable.Empty<String>() : lines;
        }
    }
}
