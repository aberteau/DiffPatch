using System;
using System.Linq;
using DiffPatch.DiffParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiffPatch.DiffPatcher;

namespace DiffPatch.Tests
{
    [TestClass]
    public class PatcherTests
    {
        [TestMethod]
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
            var files = Diff.Parse(diff, Environment.NewLine).ToArray();
            var file = files[0];
            Assert.AreEqual(1, file.Chunks.Count());
            var chunk = file.Chunks.First();


            var srcString = "line1\nline1a";
            var expectedString = "line2\nline1a";

            string patchedString = Patcher.Patch(srcString, new [] {chunk});
            Assert.AreEqual(expectedString, patchedString);
        }
    }
}