using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiffPatch.Core;
using DiffPatch.Data;

namespace DiffPatch.DiffPatcher
{
    public class Patcher
    {
        public static String Patch(String src, IEnumerable<ChunkDiff> chunkDiffs)
        {
            IEnumerable<String> srcLines = StringHelper.SplitLines(src);
            int lineIndex = 1;


            IEnumerable<Tuple<Int32, Int32, ChunkDiff>> chunkDiffByOldStart = chunkDiffs.Select(d => new Tuple<int, int, ChunkDiff>(d.OldStart, d.OldStart + d.OldLines, d));

            StringBuilder stringBuilder = new StringBuilder();


            foreach (string srcLine in srcLines)
            {
                Tuple<int, int, ChunkDiff> tuple = chunkDiffByOldStart.FirstOrDefault(kvp => lineIndex >= kvp.Item1 && lineIndex < kvp.Item2);
                if (tuple == null)
                {
                    stringBuilder.AppendLine(srcLine);
                }
                else
                {
                    LineDiff lineDiff = tuple.Item3.Changes.FirstOrDefault(l => tuple.Item1 + l.OldIndex == lineIndex);
                    if (lineDiff != null)
                    {
                        if (lineDiff.Normal || lineDiff.Add)
                        {
                            stringBuilder.AppendLine(lineDiff.Content);
                        }
                    }

                }
                lineIndex++;
            }

            return stringBuilder.ToString();
        }
    }
}
