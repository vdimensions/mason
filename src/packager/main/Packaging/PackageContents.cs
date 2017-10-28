using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Mason.Packaging
{
    public sealed class PackageContents : IEnumerable<PackageFileInclude>
    {
        private const string Separator = "=>";
        private readonly IEnumerable<PackageFileInclude> includes;

        public PackageContents(string descriptor)
        {
            includes = File.ReadAllLines(descriptor)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0 && !(x.StartsWith("#") || x.StartsWith("//")))
                .Select(
                    x =>
                    {
                        var ix = x.IndexOf(Separator, StringComparison.Ordinal);
                        if (ix < 0)
                        {
                            return (PackageFileInclude?) null;
                        }
                        return new PackageFileInclude
                        {
                            Source = x.Substring(0, ix).Trim(),
                            Destination = x.Substring(ix + Separator.Length).Trim()
                        };
                    })
                .Where(x => x.HasValue)
                .Select(x => x.Value);
        }

        public IEnumerator<PackageFileInclude> GetEnumerator() { return includes.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
