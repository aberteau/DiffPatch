using System.IO;
using System.Reflection;

namespace DiffPatch.Tests
{
    class DataSetHelper
    {
        public static string ReadFileContent(string dataSetId, string filename)
        {
            Assembly assembly = typeof(DiffParserTests).GetTypeInfo().Assembly;
            string assemblyName = assembly.GetName().Name;
            string resourceName = $"{assemblyName}.DataSets.{dataSetId}.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string fileContent = reader.ReadToEnd();
                return fileContent;
            }
        }
    }
}
