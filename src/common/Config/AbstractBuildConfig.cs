using System;
using System.Collections.Generic;


namespace Mason.Config
{
    public abstract class AbstractBuildConfig : IBuildConfig
    {
        private readonly IDictionary<string, string> data = new Dictionary<string, string>(StringComparer.Ordinal);

        protected abstract void Read();

        public IEnumerable<string> Keys { get { return data.Keys; } }

        public string this[string key]
        {
            get 
            { 
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                string result = null;
                return data.TryGetValue(key, out result) ? result : null;
            }
            protected set
            {
                data[key] = value;
            }
        }
    }

}
