
namespace DigiBot
{
    using System;
    using Avalonia;
    using Avalonia.Logging.Serilog;
    using DigiBotExtension;
    using DigiBot.ViewModels;
    using DigiBot.Views;
    using Ninject;
    using Serilog;
    class Digibot
    {
        static void Main(string[] args)
        {
            LoggerConfiguration config = new LoggerConfiguration();
#if DEBUG
            config = config.WriteTo.ColoredConsole();
#endif
            config = config.WriteTo.RollingFile("log-{Date}.log", Serilog.Events.LogEventLevel.Warning);
            Log.Logger = config.CreateLogger();
            Log.Information("Starting Ninject.");
            Kernel = new StandardKernel();
            try
            {
                Log.Information("Starting app in windowed mode");

                BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
            }
            catch (Exception e)
            {
                Log.Fatal(e, "This should never be seen");
            }
        }
        public static IKernel Kernel { get; private set; }
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI();
    }
}
