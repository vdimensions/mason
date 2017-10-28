using System.Collections.Generic;
using System.IO;


namespace Mason
{
    public static class FileSystem
    {
        public delegate bool LocationPredicate(string location);

        public static IEnumerable<string> EnumerateDirectories(string location, LocationPredicate filter, LocationPredicate termninateCondition)
        {
            var root = Directory.GetDirectoryRoot(location);
            var current = location;
            do
            {
                var dir = current;
                current = Directory.GetParent(current).FullName;
                if (filter == null || filter(dir))
                {
                    yield return dir;
                }
                if (termninateCondition != null && termninateCondition(dir))
                {
                    break;
                }
            }
            while (current != root);
        }
        public static IEnumerable<string> EnumerateDirectories(string location, LocationPredicate filter)
        {
            return EnumerateDirectories(location, filter, null);
        }

        public static IEnumerable<string> SearchDirectories(string location, string pattern, SearchOption searchOption)
        {
            return Directory.GetFiles(location, pattern, searchOption);
        }
        public static IEnumerable<string> SearchDirectories(string location, string pattern) 
        { 
            return SearchDirectories(location, pattern, SearchOption.TopDirectoryOnly); 
        }
    }
}

