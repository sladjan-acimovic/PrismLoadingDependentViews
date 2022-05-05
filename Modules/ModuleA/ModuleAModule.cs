﻿using Microsoft.Practices.Unity;
using ModuleA.Views;
using Prism.Modularity;
using Prism.Unity;

namespace ModuleA
{
    public class ModuleAModule : IModule
    {
        IUnityContainer _container;

        public ModuleAModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            //register for nav
            _container.RegisterTypeForNavigation<ViewA>();
            _container.RegisterTypeForNavigation<ViewB>();
        }
    }
}
