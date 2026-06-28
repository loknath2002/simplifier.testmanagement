using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Simplifier.TestManagement.Services;
using Simplifier.TestManagement.ViewModels;
using Simplifier.TestManagement.Views;

namespace Simplifier.TestManagement;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        var services = collection.BuildServiceProvider();
        TestModelDuplicateHandler.SharedContext = services.GetRequiredService<ISharedContext>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>()
            };
            
            services.GetRequiredService<IViewNavigation>().Navigate(null, services.GetRequiredService<WelcomeToSSTMViewModel>());
        }

        base.OnFrameworkInitializationCompleted();
    }
}