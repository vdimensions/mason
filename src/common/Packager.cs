using System.Text;

using Mason.Packaging;


namespace Mason
{
    class Packager
    {
        public static void Main(string[] args)
        {
            var location = args[0];
            var projectName = args.Length > 1 ? args[1] : null;

            PackagingManager.Distribute(location, projectName, Encoding.UTF8);
        }
    }
}

