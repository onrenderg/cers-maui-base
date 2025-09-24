using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

namespace CERS
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register platform-specific SQLite implementation for both MAUI DI and DependencyService
#if ANDROID
            builder.Services.AddSingleton<ISQLite, Platforms.Android.MauiSQLite>();
            DependencyService.Register<ISQLite, Platforms.Android.MauiSQLite>();
#elif IOS
            builder.Services.AddSingleton<ISQLite, Platforms.iOS.MauiSQLite>();
            DependencyService.Register<ISQLite, Platforms.iOS.MauiSQLite>();
#elif WINDOWS
            builder.Services.AddSingleton<ISQLite, Platforms.Windows.MauiSQLite>();
            DependencyService.Register<ISQLite, Platforms.Windows.MauiSQLite>();
#endif

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
