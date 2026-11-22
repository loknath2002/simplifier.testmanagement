using System;
using Avalonia;
using Simplifier.Serilog;
namespace Simplifier.TestManagement;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
#if DEBUG
        LoggingConfiguration? aConf = LoggingConfiguration.ProdConfigWithConsole;
#else
            var aConf = LoggingConfiguration.ProdConfig;
#endif
        aConf.FileName = Paths.LogFilePath;
        aConf.MultiProcess = true;
        LoggingSetup.Create(aConf);
        var aDomain = new LoggerDetail(typeof(Program));
        using var aLogger = new LogScope(aDomain);
        aLogger.Info($"Starting the application CommandLine : {Environment.CommandLine}");
        aLogger.Info($"AppDataRoot: {Paths.AppDataRoot}");
        aLogger.Info($"LogDirectory: {Paths.LogDirectory}");
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}