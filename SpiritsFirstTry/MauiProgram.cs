﻿using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Maui;
using Microsoft.Extensions.Logging;
using SpiritsFirstTry.AutoMappers;
using SpiritsFirstTry.Services;
using SpiritsFirstTry.Services.Interfaces;
using SpiritsFirstTry.ViewModels;
using The49.Maui.BottomSheet;
using System.Reflection;
using Microsoft.Extensions.Configuration;
#if ANDROID
using SpiritsFirstTry.Platforms.Android;
#endif
namespace SpiritsFirstTry
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("SpiritsFirstTry.appsettings.Development.json");

            var config = new ConfigurationBuilder()
                        .AddJsonStream(stream)
                        .Build();

            builder
                .UseMauiApp<App>()
                .UseBottomSheet()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("A DAY WITHOUT SUN.otf", "Asterlight");
                });






            builder.Configuration.AddConfiguration(config);
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.UseArcGISRuntime(c => c.UseApiKey(config["ArcGis:ApiKey"]));
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
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginViewModel>();
#if ANDROID
            builder.Services.AddSingleton<IGoogleAuthenticationService, GoogleAuthenticationService>();
#endif


            builder.Services.AddTransient<BottomSheetView>();
            builder.Services.AddTransient<BottomSheetViewModel>();


            return builder.Build();
        }
    }
}
