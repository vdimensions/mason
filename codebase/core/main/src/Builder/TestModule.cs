using Axle.Modularity;

namespace Mason.Builder
{
    [Module]
    [ModuleCommandLineTrigger(ArgumentIndex = 0, ArgumentValue = "test")]
    internal sealed class TestModule
    {
    }
}
