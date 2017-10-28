using System;
using System.Collections.Generic;


namespace Mason.Config
{
    internal class BuildConfigChain : IBuildConfig
    {
        private readonly IEnumerable<IBuildConfig> configs;
        private readonly HashSet<string> keys = new HashSet<string>(StringComparer.Ordinal);

        public BuildConfigChain(IEnumerable<IBuildConfig> configs) 
        {
            this.configs = configs;
            foreach (var config in configs)
            foreach (var key in config.Keys)
            {
                if (keys.Contains(key))
                {
                    continue;
                }
                keys.Add(key);
            }
        }
        public BuildConfigChain(params IBuildConfig[] configs) : this(configs as IEnumerable<IBuildConfig>) { }

        public IEnumerable<string> Keys => this.keys;

        public string this[string key]
        {
            get
            {
                foreach (var config in configs)
                {
                    var value = config[key];
                    if (value != null)
                    {
                        return value;
                    }
                }
                return null;
            }
        }
    }    
}
