using System;
using System.IO;
using System.Text;


namespace Mason.Config
{
    internal class BuildConfig : AbstractBuildConfig
    {
        private readonly FileInfo file;
        private readonly Encoding encoding;

        public BuildConfig(Uri location, Encoding encoding) : this(location.ToString(), encoding) { }
        public BuildConfig(string location, Encoding encoding) 
        {
            file = new FileInfo(location);
            this.encoding = encoding;
            Read();
        }

        protected sealed override void Read()
        {
            var lines = File.ReadAllLines(file.FullName, encoding);
            foreach (var line in lines)
            {
                var l = line.TrimStart( );

                if (l.StartsWith("#") || l.StartsWith("//"))
                {   //
                    // skip comments
                    //
                    continue;
                }

                var splitIndex = l.IndexOf('=');
                if (splitIndex < 0)
                {   //
                    // invalid row
                    //
                    continue;
                }

                var key = l.Substring(0, splitIndex).TrimEnd( );
                var value = l.Substring(splitIndex + 1).Trim( );
                var quote = "\"";
                if (value.StartsWith(quote) && value.EndsWith(quote))
                {
                    value = value.Substring(1, value.Length - 2);
                }
                this[key] = value;
            }
        }

        public bool Exists => file.Exists;
    }
}

