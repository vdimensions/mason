using System;
using System.Collections.Generic;


namespace Mason.Config
{
    public abstract class AbstractBuildConfig : IBuildConfig
    {
        private readonly IDictionary<string, string> data = new Dictionary<string, string>(StringComparer.Ordinal);

        protected abstract void Read();

        public IEnumerable<string> Keys => data.Keys;

        public string this[string key]
        {
            get 
            { 
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return data.TryGetValue(key, out string result) ? result : null;
            }
            protected set
            {
                data[key] = value;
            }
        }
    }

}
