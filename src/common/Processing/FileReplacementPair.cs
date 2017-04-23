using System.IO;


namespace Mason.Processing
{
    public class FileReplacementPair
    {
        public FileReplacementPair(string rawFile, string targetFile) : this(new FileInfo(rawFile), new FileInfo(targetFile)) { }
        public FileReplacementPair(FileInfo rawFile, FileInfo targetFile)
        {
            RawFile = rawFile;
            TargetFile = targetFile;
        }

        public FileInfo RawFile { get; private set; }
        public FileInfo TargetFile { get; private set; }
    }

}

