using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Mason.Config
{
    internal class WriteableBuildConfig : JavaProperties
    {
        private readonly FileInfo file;
        private readonly Encoding encoding;
        private readonly HashSet<string> updatedKeys = new HashSet<string>(StringComparer.Ordinal);

        public WriteableBuildConfig(string location, Encoding encoding) : base(location, encoding) 
        { 
            this.file = new FileInfo(location);
            this.encoding = encoding;
        }

        public WriteableBuildConfig UpdateValue(string key, string value)
        {
            this[key] = value;
            updatedKeys.Add(key);
            return this;
        }

        public void UpdateConfig()
        {
            string[] lines = File.ReadAllLines(file.FullName, encoding);
            int updates = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                foreach (string updatedKey in updatedKeys)
                {
                    if (line.TrimStart( ).StartsWith(updatedKey))
                    {
                        lines[i] = string.Format("{0} = {1}", updatedKey, this[updatedKey]);
                        updates++;
                    }
                }
            }
            if (updates > 0)
            {
                File.WriteAllLines(file.FullName, lines, encoding);
            }
        }
    }
}

