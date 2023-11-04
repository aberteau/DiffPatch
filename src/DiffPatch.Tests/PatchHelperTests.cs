using System;
using System.Linq;
using DiffPatch.Data;
using FluentAssertions;

namespace DiffPatch.Tests
{
    public class PatchHelperTests
    {
        [Fact]
        public void ShouldPatchSingleLine()
        {
            var diff = @"
diff --git a/file b/file
index 123..456 789
--- a/file
+++ b/file
@@ -1,2 +1,2 @@
- line1
+ line2";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            var file = files[0];
            file.Chunks.Should().HaveCount(1);

            var chunk = file.Chunks.First();

            var srcString = " line1\n line1a";
            var expectedString = " line2\n line1a";

            string patchedString = PatchHelper.Patch(srcString, new [] {chunk}, "\n");
            patchedString.Should().Be(expectedString);
        }

        [Fact]
        public void ShouldPatchDataSet1709251127Diff()
        {
            string dataSetId = "D1709251127";

            var diff = DataSetHelper.ReadFileContent(dataSetId, "Diff-b3a6303-781096c.diff");

            FileDiff[] files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            FileDiff file = files[0];

            string srcString = DataSetHelper.ReadFileContent(dataSetId, "Diff-b3a6303.txt");
            string expectedString = DataSetHelper.ReadFileContent(dataSetId, "Diff-781096c.txt").Trim();

            string patchedString = PatchHelper.Patch(srcString, file.Chunks, Environment.NewLine).Trim();
            patchedString.Should().Be(expectedString);
        }
    }
}