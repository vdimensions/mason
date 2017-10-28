using System.Text;

using Mason.Versioning;


namespace Mason
{
    class Verman
    {
        public static void Main(string[] args)
        {
            var location = args[0];
            var projectName = args.Length > 1 ? args[1] : null;

            VersionManager.IncreaseVersion(location, projectName, Encoding.UTF8);
        }
    }
}

