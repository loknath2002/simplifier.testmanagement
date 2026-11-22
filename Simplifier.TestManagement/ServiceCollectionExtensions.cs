// /*
// * Copyright (c) 2026 Loknath
// */

using Microsoft.Extensions.DependencyInjection;
using Simplifier.TestManagement.Services;
using Simplifier.TestManagement.ViewModels;
using SukiUI.Dialogs;
using SukiUI.Toasts;
namespace Simplifier.TestManagement
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddSingleton<ISukiDialogManager, SukiDialogManager>();
            collection.AddSingleton<ISukiToastManager, SukiToastManager>();
            collection.AddSingleton<IToastMessenger, SukiToastMessenger>();
            collection.AddTransient<ProjectDetailsViewModel>();
            collection.AddTransient<WelcomeToSSTMViewModel>();
            collection.AddTransient<ProjectCreationWizardViewModel>();
            collection.AddSingleton<MainWindowViewModel>();
            collection.AddSingleton<IViewNavigation>(provider => provider.GetRequiredService<MainWindowViewModel>());
            collection.AddSingleton<ITestCaseRendererFactory, TestCaseRendererFactory>();
            collection.AddSingleton<IStorageService, StorageServiceImpl>();
            collection.AddSingleton<IPlatformServices, PlatformServices>();
            collection.AddSingleton<ISharedContext, SharedContext>();
        }
    }
}