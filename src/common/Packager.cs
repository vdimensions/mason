using System.Text;

using Mason.Config;
using Mason.Distribution;


namespace Mason
{
    class Packager
    {
        public static void Main(string[] args)
        {
            var location = args[0];
            var projectName = args.Length > 1 ? args[1] : null;

            var config = BuildConfiguration.Get(location, projectName, Encoding.UTF8);

            PackagingManager.Distribute(config, location);
        }
    }
}

