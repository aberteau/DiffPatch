using System.Collections.Generic;

namespace DiffPatch.Data
{
    public class Chunk
    {
        public Chunk(string content, int oldStart, int oldLines, int newStart, int newLines)
        {
            Content = content;
            OldStart = oldStart;
            OldLines = oldLines;
            NewStart = newStart;
            NewLines = newLines;
        }

        public ICollection<LineDiff> Changes { get; } = new List<LineDiff>();

        public string Content { get; }

        public int OldStart { get; }

        public int OldLines { get; }

        public int NewStart { get; }

        public int NewLines { get; }
    }
}
