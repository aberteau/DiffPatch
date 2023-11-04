using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DiffPatch.Data;
using FluentAssertions;

namespace DiffPatch.Tests
{
    public class DiffParserTests
    {
        [Fact]
        public void ShouldParseNull()
        {
            DiffParserHelper.Parse(null).Should().HaveCount(0);
        }

        [Fact]
        public void ShouldParseEmptyString()
        {
            DiffParserHelper.Parse(string.Empty).Should().HaveCount(0);
        }

        [Fact]
        public void ShouldParseWhitespace()
        {
            DiffParserHelper.Parse(" ").Should().HaveCount(0);
        }

        [Fact]
        public void ShouldParseDataSet1709251127Diff()
        {
            string diff = DataSetHelper.ReadFileContent("D1709251127", "Diff-b3a6303-781096c.diff");

            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(1);
            var file = files[0];

            string expectedFileName = "DiffPatch.DiffParser/Diff.cs";
            file.From.Should().Be(expectedFileName);
            file.To.Should().Be(expectedFileName);
            file.Chunks.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldParseDataSet108Diff()
        {
            string diff = DataSetHelper.ReadFileContent("D1709251127", "BinaryDiff.diff");

            var files = DiffParserHelper.Parse(diff).ToArray();
            files.Length.Should().Be(1);
            var file = files[0];

            file.From.Should().Be("/dev/null");
            file.To.Should().Be("Blog.db");
            file.Type.Should().Be(FileChangeType.Modified);
        }

        [Fact]
        public void ShouldParseSimpleGitLikeDiff()
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
            files.Length.Should().Be(1);
            var file = files[0];
            file.From.Should().Be("file");
            file.To.Should().Be("file");
            file.Chunks.Should().HaveCount(1);
            var chunk = file.Chunks.First();
            chunk.Content.Should().Be("@@ -1,2 +1,2 @@");
            var changes = chunk.Changes.ToArray();
            changes.Should().HaveCount(2);
            changes[0].Content.Should().Be(" line1");
            changes[1].Content.Should().Be(" line2");
        }

        [Fact]
        public void ShouldParseDiffWIthDeletedFileModeLine()
        {
            var diff = @"
diff --git a/test b/test
deleted file mode 100644
index db81be4..0000000
--- b/test
+++ /dev/null
@@ -1,2 +0,0 @@
-line1
-line2
";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(1);
            var file = files[0];
            file.Type.Should().Be(FileChangeType.Delete);
            file.From.Should().Be("test");
            file.To.Should().Be("/dev/null");
            var chunk = file.Chunks.First();
            chunk.Content.Should().Be("@@ -1,2 +0,0 @@");
            chunk.Changes.Should().HaveCount(2);
            var changes = chunk.Changes.ToArray();
            changes[0].Content.Should().Be("line1");
            changes[1].Content.Should().Be("line2");
        }

        [Fact]
        public void ShouldParseDiffWithNewFileModeLine()
        {
            var diff = @"
diff --git a/test b/test
new file mode 100644
index 0000000..db81be4
--- /dev/null
+++ b/test
@@ -0,0 +1,2 @@
+line1
+line2
";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(1);
            var file = files[0];
            file.Type.Should().Be(FileChangeType.Add);
            file.From.Should().Be("/dev/null");
            file.To.Should().Be("test");
            file.Chunks.ElementAt(0).Content.Should().Be("@@ -0,0 +1,2 @@");
            file.Chunks.ElementAt(0).Changes.Should().HaveCount(2);
            file.Chunks.ElementAt(0).Changes.ElementAt(0).Content.Should().Be("line1");
            file.Chunks.ElementAt(0).Changes.ElementAt(1).Content.Should().Be("line2");
        }

        [Fact]
        public void ShouldParseDiffWithDeletedFileModeLine()
        {
            var diff = @"
diff --git a/test b/test
deleted file mode 100644
index db81be4..0000000
--- b/test
+++ /dev/null
@@ -1,2 +0,0 @@
-line1
-line2
";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(1);
            var file = files[0];
            file.Type.Should().Be(FileChangeType.Delete);
            file.From.Should().Be("test");
            file.To.Should().Be("/dev/null");
            file.Chunks.ElementAt(0).Content.Should().Be("@@ -1,2 +0,0 @@");
            file.Chunks.ElementAt(0).Changes.Should().HaveCount(2);
            file.Chunks.ElementAt(0).Changes.ElementAt(0).Content.Should().Be("line1");
            file.Chunks.ElementAt(0).Changes.ElementAt(1).Content.Should().Be("line2");
        }

        [Fact]
        public void ShouldParseDiffWithSingleLineFiles()
        {
            var diff = @"
diff --git a/file1 b/file1
deleted file mode 100644
index db81be4..0000000
--- b/file1
+++ /dev/null
@@ -1 +0,0 @@
-line1
diff --git a/file2 b/file2
new file mode 100644
index 0000000..db81be4
--- /dev/null
+++ b/file2
@@ -0,0 +1 @@
+line1
";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(2);

            var file = files[0];
            file.Deleted.Should().BeTrue();
            file.From.Should().Be("file1");
            file.To.Should().Be("/dev/null");
            file.Chunks.ElementAt(0).Content.Should().Be("@@ -1 +0,0 @@");
            file.Chunks.ElementAt(0).Changes.Should().HaveCount(1);
            file.Chunks.ElementAt(0).Changes.ElementAt(0).Content.Should().Be("line1");
            file.Chunks.ElementAt(0).Changes.ElementAt(0).Type.Should().Be(LineChangeType.Delete);

            file = files[1];
            file.Add.Should().BeTrue();
            file.From.Should().Be("/dev/null");
            file.To.Should().Be("file2");
            file.Chunks.ElementAt(0).Content.Should().Be("@@ -0,0 +1 @@");
            file.Chunks.ElementAt(0).RangeInfo.NewRange.LineCount.Should().Be(0);
            file.Chunks.ElementAt(0).Changes.Should().HaveCount(1);
            file.Chunks.ElementAt(0).Changes.ElementAt(0).Content.Should().Be("line1");
            file.Chunks.ElementAt(0).Changes.ElementAt(0).Type.Should().Be(LineChangeType.Add);
        }

        [Fact]
        public void ShouldParseMultipleFilesInDiff()
        {
            var diff = @"
diff --git a/file1 b/file1
index 123..456 789
--- a/file1
+++ b/file1
@@ -1,2 +1,2 @@
- line1
+ line2
diff --git a/file2 b/file2
index 123..456 789
--- a/file2
+++ b/file2
@@ -1,3 +1,3 @@
- line1
+ line2
";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(2);
            var file = files[0];
            file.From.Should().Be("file1");
            file.To.Should().Be("file1");
            file.Chunks.ElementAt(0).Content.Should().Be("@@ -1,2 +1,2 @@");
            file.Chunks.ElementAt(0).Changes.Should().HaveCount(2);
            file.Chunks.ElementAt(0).Changes.ElementAt(0).Content.Should().Be(" line1");
            file.Chunks.ElementAt(0).Changes.ElementAt(1).Content.Should().Be(" line2");

            file = files[1];
            file.From.Should().Be("file2");
            file.To.Should().Be("file2");
            file.Chunks.ElementAt(0).Content.Should().Be("@@ -1,3 +1,3 @@");
            file.Chunks.ElementAt(0).Changes.Should().HaveCount(2);
            file.Chunks.ElementAt(0).Changes.ElementAt(0).Content.Should().Be(" line1");
            file.Chunks.ElementAt(0).Changes.ElementAt(1).Content.Should().Be(" line2");
        }

        [Fact]
        public void ShouldParseGnuSampleDiff()
        {
            var diff = @"
--- lao	2002-02-21 23:30:39.942229878 -0800
+++ tzu	2002-02-21 23:30:50.442260588 -0800
@@ -1,7 +1,6 @@
-The Way that can be told of is not the eternal Way;
-The name that can be named is not the eternal name.
The Nameless is the origin of Heaven and Earth;
-The Named is the mother of all things.
+The named is the mother of all things.
+
Therefore let there always be non-being,
	so we may see their subtlety,
And let there always be being,
@@ -9,3 +8,6 @@
The two are the same,
But after they are produced,
	they have different names.
+They both may be called deep and profound.
+Deeper and more profound,
+The door of all subtleties!
";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(1);
            var file = files[0];
            file.From.Should().Be("lao");
            file.To.Should().Be("tzu");
            file.Chunks.Should().HaveCount(2);

            var chunk0 = file.Chunks.ElementAt(0);
            chunk0.RangeInfo.OriginalRange.StartLine.Should().Be(1);
            chunk0.RangeInfo.OriginalRange.LineCount.Should().Be(7);
            chunk0.RangeInfo.NewRange.StartLine.Should().Be(1);
            chunk0.RangeInfo.NewRange.LineCount.Should().Be(6);

            var chunk1 = file.Chunks.ElementAt(1);
            chunk1.RangeInfo.OriginalRange.StartLine.Should().Be(9);
            chunk1.RangeInfo.OriginalRange.LineCount.Should().Be(3);
            chunk1.RangeInfo.NewRange.StartLine.Should().Be(8);
            chunk1.RangeInfo.NewRange.LineCount.Should().Be(6);
        }

        [Fact]
        public void ShouldParseHgDiffOutput()
        {
            var diff = @"
diff -r 514fc757521e lib/parsers.coffee
--- a/lib/parsers.coffee	Thu Jul 09 00:56:36 2015 +0200
+++ b/lib/parsers.coffee	Fri Jul 10 16:23:43 2015 +0200
@@ -43,6 +43,9 @@
             files[file] = { added: added, deleted: deleted }
         files
+    diff: (out) ->
+        files = {}
+
 module.exports = Parsers
 module.exports.version = (out) ->
";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(1);
            var file = files[0];
            file.Chunks.ElementAt(0).Content.Should().Be("@@ -43,6 +43,9 @@");
            file.From.Should().Be("lib/parsers.coffee");
            file.To.Should().Be("lib/parsers.coffee");
        }

        [Fact]
        public void ShouldParseFileNamesForNNewEmptyFile()
        {
            var diff = @"
diff --git a/newFile.txt b/newFile.txt
new file mode 100644
index 0000000..e6a2e28
";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(1);
            var file = files[0];
            file.From.Should().Be("/dev/null");
            file.To.Should().Be("newFile.txt");
        }

        [Fact]
        public void ShouldParseFileNamesForADeletedFile()
        {
            var diff = @"
diff --git a/deletedFile.txt b/deletedFile.txt
deleted file mode 100644
index e6a2e28..0000000
";
            var files = DiffParserHelper.Parse(diff, Environment.NewLine).ToArray();
            files.Length.Should().Be(1);
            var file = files[0];
            file.From.Should().Be("deletedFile.txt");
            file.To.Should().Be("/dev/null");
        }
    }
}