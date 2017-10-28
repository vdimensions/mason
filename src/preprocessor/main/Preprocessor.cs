using System.Text;

using Mason.Processing;


namespace Mason
{
    class Preprocessor
    {
        public static void Main(string[] args)
        {
            var location = args[0];
            var projectName = args.Length > 1 ? args[1] : null;
            var encoding = Encoding.UTF8;

            FileReplacementScanner.ProcessFiles(location, projectName, encoding);
        }
    }
}
