﻿using System;
using System.Collections.Generic;
using System.Linq;

using Mason.Sdk;


namespace Mason
{
    sealed class Packager : CompositeModule
    {
        public Packager() : base(
            "pack", 
            new NuspecIncludesModule(), new NugetPackageCreateModule(), new NugetPackagePurgerModule()) { }

        public override IEnumerable<IMasonModule> ResolveModules(Options.IOptionMap options, IEnumerable<IMasonModule> submodules)
        {
            return submodules.Where(
                x =>
                {
                    var t = options.TryResolve<bool>(x.Name);
                    return t.Item1 && t.Item2;
                });
        }
    }
}

