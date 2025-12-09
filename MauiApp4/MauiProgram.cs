using MauiApp4.Services;
using MauiApp4.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MauiApp4
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Vokiar.otf", "Vokiar");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            RegisterServices(builder.Services);

            var app = builder.Build();
            Services = app.Services;

            return app;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IApiService, ApiService>();
            services.AddTransient<ContactsViewModel>();
            services.AddTransient<ContactsPage>();
        }
    }
}