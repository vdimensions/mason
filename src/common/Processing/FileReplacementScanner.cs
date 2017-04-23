using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Mason.Config;


namespace Mason.Processing
{
    public static class FileReplacementScanner
    {
        private const string DefaultRawFileMatchPattern = "*.template";

        public static IEnumerable<FileReplacementPair> GetReplacementPairs(string location, string rawFilesPattern)
        {
            var files = FileSystem.SearchDirectories(
                location,
                rawFilesPattern,
                SearchOption.AllDirectories);

            var dotIndex = rawFilesPattern.LastIndexOf('.');
            var extension = rawFilesPattern.Substring(dotIndex);

            IList<FileReplacementPair> result = new List<FileReplacementPair>( );
            foreach (var rawFile in files)
            {
                var extIndex = rawFile.LastIndexOf(extension, StringComparison.Ordinal);
                var targetFile = rawFile.Substring(0, extIndex);
                result.Add(new FileReplacementPair(rawFile, targetFile));
            }

            return result;
        }
        public static IEnumerable<FileReplacementPair> GetReplacementPairs(string location)
        {
            return GetReplacementPairs(location, DefaultRawFileMatchPattern);
        }

        public static void ProcessFiles(string location, string projectName, Encoding fileEncoding)
        {
            Directory.SetCurrentDirectory(location);

            var config = BuildConfiguration.Get(location, projectName, fileEncoding);
            var x = new Expander();

            foreach (var pair in GetReplacementPairs(location))
            {
                var replacedContents = x.Expand(config, File.ReadAllText(pair.RawFile.FullName, fileEncoding));
                using (var fileStream = new FileStream(pair.TargetFile.FullName, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(fileStream, fileEncoding))
                {
                    writer.Write(replacedContents);
                    writer.Flush();
                    fileStream.Flush();
                }
            }
        }
    }
}
