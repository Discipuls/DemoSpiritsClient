using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Maui;
using Microsoft.Extensions.Logging;
using SpiritsFirstTry.AutoMappers;
using SpiritsFirstTry.Services;
using SpiritsFirstTry.Services.Interfaces;
using SpiritsFirstTry.ViewModels;
using The49.Maui.BottomSheet;

namespace SpiritsFirstTry
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBottomSheet()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("A DAY WITHOUT SUN.otf", "Asterlight");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.UseArcGISRuntime(config => config.UseApiKey("AAPK5097e917f3254b0fb46110d95982e99exKDkBAgOLzsIkGV76szaup4bRsialeK74tnC5M4D-QeyQGGrcERl11Q7BZFmAQ5y"));
            builder.Services.AddAutoMapper(typeof(SpiritAutoMapper));

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<IRestService,RestService>();
            builder.Services.AddSingleton<ISpiritService, SpiritService>();
            builder.Services.AddSingleton<IHabitatService, HabitatService>();
            builder.Services.AddSingleton<AdminSpiritsViewModel>();
            builder.Services.AddSingleton<AdminSpiritPage>();
            builder.Services.AddSingleton<AdminHabitatPage>();
            builder.Services.AddSingleton<AdminHabitatViewModel>();


            builder.Services.AddTransient<BottomSheetView>();
            builder.Services.AddTransient<BottomSheetViewModel>();


            return builder.Build();
        }
    }
}
