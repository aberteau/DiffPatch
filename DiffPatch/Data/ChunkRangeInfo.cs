using System;
using System.Collections.Generic;
using System.Text;

namespace DiffPatch.Data
{
    public class ChunkRangeInfo
    {
        public ChunkRangeInfo(ChunkRange originalRange, ChunkRange newRange)
        {
            OriginalRange = originalRange;
            NewRange = newRange;
        }

        public ChunkRange OriginalRange { get; }

        public ChunkRange NewRange { get; }
    }
}
