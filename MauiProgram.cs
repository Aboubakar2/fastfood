 using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using fastfood.Data;
using fastfood.Pages;
using fastfood.ViewModels;

namespace fastfood;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
            fonts.AddFont("Poppins-SeminBold.ttf", "PoppinsSemiBold");
        })
        .UseMauiCommunityToolkit(options =>
        {
            options.SetShouldEnableSnackbarOnWindows(true);
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<DatabaseService>()
            .AddSingleton<HomeViewModel>()
            .AddSingleton<MainPage>()
            .AddSingleton<OrdersViewModel>()
            .AddSingleton<OrdersPage>()
            .AddTransient<ManageMenuViewModel>()
            .AddTransient<ManageMenuItemsPage>();

        return builder.Build();
    }
}