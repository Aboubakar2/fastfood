using fastfood.Models;
using Windows.Storage;
using Windows.System;
using QuestPDF.Fluent;

using Launcher = Windows.System.Launcher;
using QuestPDF.Infrastructure;


namespace fastfood.Utils
{
    public class ReceiptService
    {
        // Informations du restaurant (à stocker dans vos paramètres d'application)
        private readonly string _restaurantName = "FastFood Deluxe";
        private readonly string _restaurantAddress = "Rue de la Gastronomie, Niamey";
        private readonly string _restaurantPhone = "20 23 45 67";


        public async Task<string> GenerateReceiptAsync(
            IEnumerable<CartModel> cartItems,
            decimal subTotal,
            decimal taxPercentage,
            decimal taxAmount,
            decimal total,
            string orderId,
            string paymentMode)
        {
            var document = new ReceiptDocument(
                cartItems,
                subTotal,
                taxPercentage,
                taxAmount,
                total,
                orderId,
                DateTime.Now,
                paymentMode,
                _restaurantName,
                _restaurantAddress,
                _restaurantPhone
            );

            // Génération du nom de fichier
            string fileName = $"Ticket-{orderId}-{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            // Génération du PDF
            document.GeneratePdf(filePath);

            return filePath;
        }

        public async Task PrintReceiptAsync(string filePath)
        {
            try
            {
                // Option 1: Impression directe via le dialogue d'impression de Windows
                var file = await StorageFile.GetFileFromPathAsync(filePath);
                await Launcher.LaunchFileAsync(file, new LauncherOptions { DisplayApplicationPicker = false });

                // Option 2: Impression silencieuse (si vous avez une imprimante thermique configurée comme imprimante par défaut)
                // Process.Start(new ProcessStartInfo
                // {
                //     FileName = filePath,
                //     Verb = "print",
                //     CreateNoWindow = true,
                //     WindowStyle = ProcessWindowStyle.Hidden
                // });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'impression: {ex.Message}", ex);
            }
        }
    }
}