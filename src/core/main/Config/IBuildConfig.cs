using System.Collections.Generic;


namespace Mason.Config
{
    public interface IBuildConfig
    {
        IEnumerable<string> Keys { get; }

        string this[string key] { get; }
    }
}

