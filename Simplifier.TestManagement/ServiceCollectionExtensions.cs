// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */

using Microsoft.Extensions.DependencyInjection;
using Simplifier.TestManagement.Services;
using Simplifier.TestManagement.ViewModels;
namespace Simplifier.TestManagement
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddTransient<ProjectDetailsViewModel>();
            
            collection.AddTransient<WelcomeToSSTMViewModel>();
    
            collection.AddSingleton<MainWindowViewModel>();
    
            collection.AddSingleton<IViewNavigation>(provider => provider.GetRequiredService<MainWindowViewModel>());
            
            collection.AddSingleton<ITestCaseRendererFactory, TestCaseRendererFactory>();
            collection.AddSingleton<IStorageService, StorageServiceImpl>();
        }
    }
}