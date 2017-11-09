using Mason.Sdk;


namespace Mason
{
    sealed class Packager : ModuleChain
    {
        public Packager() : base(
            "pack", 
            true, 
            new NuspecIncludesModule(), new NugetPackageCreateModule(), new NugetPackagePurgerModule()) { }
    }
}

