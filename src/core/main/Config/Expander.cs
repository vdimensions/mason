using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Mason.Config
{
    public class Expander
    {
        private const string MatchGroupName = "exp";
        private const string RegExPattern = @"(?<"+MatchGroupName+@">(?:\${)(?:<"+MatchGroupName+@">|(?:[^}])+)+(?:}))";
        private const RegexOptions Options = RegexOptions.Compiled|RegexOptions.CultureInvariant|RegexOptions.IgnoreCase|RegexOptions.Multiline|RegexOptions.ExplicitCapture;
        private static readonly Regex _expression = new Regex(RegExPattern, Options);

        public Expander() { }

        private IList<string> GetSubstitutableKeys(string text)
        {
            var matches = _expression.Matches(text);
            var result = new List<string>( );
            foreach (Match match in matches)
            foreach (var groupName in _expression.GetGroupNames())
            {
                result.Add(match.Groups[groupName].Value);
            }
            return result;
        }

        public string Expand(IBuildConfig config, string raw)
        {
            return TryExpand(config, raw, out string expanded) ? expanded : raw;
        }

        public bool TryExpand(IBuildConfig config, string raw, out string result)
        {
            return TryExpand(config, 0, raw, out result);
        }
        private bool TryExpand(IBuildConfig config, int previousCount, string raw, out string result)
        {
            result = raw;
            var keys = GetSubstitutableKeys(raw);
            if (keys.Count == 0)
            {
                return true;
            }
            if (keys.Count != previousCount)
            {
                foreach (var key in keys)
                {
                    var nakedKey = key.Substring(2, key.Length - 3);
                    var replacement = config[nakedKey];
                    if (replacement == null)
                    {
                        return false;
                    }
                    result = result.Replace(key, config[nakedKey]);
                }
                return TryExpand(config, keys.Count, result, out result);
            }
            return false;
        }
    }
}