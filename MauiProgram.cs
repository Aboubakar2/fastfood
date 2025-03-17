 using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using fastfood.Data;
using fastfood.Pages;
using fastfood.ViewModels;
using fastfood.Utils;
using QuestPDF.Infrastructure;

namespace fastfood;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()

    {
        var builder = MauiApp.CreateBuilder();
        // QuestPDF Licence 
        QuestPDF.Settings.License = LicenseType.Community;

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
            .AddSingleton<ProviderPage>()
            .AddSingleton<ProvidersViewModel>()
            .AddSingleton<ProductsPage>()
            .AddSingleton<ProductOrderViewModel>()
            .AddTransient<ManageMenuViewModel>()
            .AddTransient<ManageMenuItemsPage>()
            .AddSingleton<ReceiptService>(); 

        return builder.Build();
    }
}