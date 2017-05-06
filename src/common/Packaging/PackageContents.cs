using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mason.Config;

namespace Mason.Packaging
{
    public sealed class PackageContents : IEnumerable<PackageInclude>
    {
        private const string Separator = "=>";
        private readonly IEnumerable<PackageInclude> includes;

        public PackageContents(string descriptor, IBuildConfig config)
        {
            var expander = new Expander();
            includes = File.ReadAllLines(descriptor)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0 && !(x.StartsWith("#") || x.StartsWith("//")))
                .Select(
                    x =>
                    {
                        var ix = x.IndexOf(Separator, StringComparison.Ordinal);
                        if (ix < 0)
                        {
                            return (PackageInclude?) null;
                        }
                        return new PackageInclude
                        {
                            Source = expander.Expand(config, x.Substring(0, ix).Trim()),
                            Destination = expander.Expand(config, x.Substring(ix + Separator.Length).Trim())
                        };
                    })
                .Where(x => x.HasValue)
                .Select(x => x.Value);
        }

        public IEnumerator<PackageInclude> GetEnumerator() { return includes.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
