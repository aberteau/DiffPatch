        private Chunk current = null;
            ChunkRangeInfo rangeInfo = new ChunkRangeInfo(
                new ChunkRange(oldStart, oldLines),
                new ChunkRange(newStart, newLines)

            current = new Chunk(line, rangeInfo);