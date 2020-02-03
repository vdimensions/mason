using Axle.Modularity;

namespace Mason.Builder
{
    [Module]
    [ModuleCommandLineTrigger(ArgumentIndex = 0, ArgumentValue = "build")]
    internal sealed class BuildModule
    {
    }
}
