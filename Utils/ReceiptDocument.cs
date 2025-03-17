using fastfood.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;


namespace fastfood.Utils
{
    public class ReceiptDocument : IDocument
    {
        public IEnumerable<CartModel> CartItems { get; }
        public decimal SubTotal { get; }
        public decimal TaxPercentage { get; }
        public decimal TaxAmount { get; }
        public decimal Total { get; }
        public string OrderId { get; }
        public DateTime OrderDate { get; }
        public string PaymentMode { get; }
        public string RestaurantName { get; }
        public string RestaurantAddress { get; }
        public string RestaurantPhone { get; }

        public ReceiptDocument(
            IEnumerable<CartModel> cartItems,
            decimal subTotal,
            decimal taxPercentage,
            decimal taxAmount,
            decimal total,
            string orderId,
            DateTime orderDate,
            string paymentMode,
            string restaurantName,
            string restaurantAddress,
            string restaurantPhone)
        {
            CartItems = cartItems;
            SubTotal = subTotal;
            TaxPercentage = taxPercentage;
            TaxAmount = taxAmount;
            Total = total;
            OrderId = orderId;
            OrderDate = orderDate;
            PaymentMode = paymentMode;
            RestaurantName = restaurantName;
            RestaurantAddress = restaurantAddress;
            RestaurantPhone = restaurantPhone;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Format ticket de caisse standard: 80mm de large
                page.Size(226.8f, 226.8f); // 226.8 points = 80mm
                page.Margin(10);
                page.DefaultTextStyle(x => x.FontSize(8));
                page.PageColor(Colors.White);

                page.Content().Element(ComposeContent);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                // En-tête
                column.Item().AlignCenter().Text(RestaurantName).FontSize(12).Bold();
                column.Item().AlignCenter().Text(RestaurantAddress).FontSize(8);
                column.Item().AlignCenter().Text(RestaurantPhone).FontSize(8);

                column.Item().PaddingVertical(5).LineHorizontal(1);

                // Détails de la commande
                column.Item().Text($"Commande #{OrderId}").Bold();
                column.Item().Text($"Date: {OrderDate:dd/MM/yyyy HH:mm}");
                column.Item().Text($"Mode de paiement: {PaymentMode}");

                column.Item().PaddingVertical(5).LineHorizontal(1);

                // Articles
                column.Item().Element(ComposeItems);

                column.Item().PaddingVertical(5).LineHorizontal(1);

                // Totaux
                column.Item().Element(ComposeSummary);

                column.Item().PaddingVertical(5).LineHorizontal(1);

                // Pied de page
                column.Item().AlignCenter().Text("Merci de votre visite!");
                column.Item().AlignCenter().Text("À bientôt!");
            });
        }

        private void ComposeItems(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                foreach (var item in CartItems)
                {
                    table.Cell().Column(column =>
                    {
                        column.Item().Text(item.Name);
                    });
                    table.Cell().AlignRight().Text($"{item.Price:F2}");
                    table.Cell().AlignRight().Text($"x{item.Quantity}");
                    table.Cell().AlignRight().Text($"{item.Amount:F2}");
                }
            });
        }

        private void ComposeSummary(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                });

                table.Cell().AlignRight().Text("Sous-total:").Bold();
                table.Cell().AlignRight().Text($"{SubTotal:F2}");

                table.Cell().AlignRight().Text($"TVA ({TaxPercentage}%):").Bold();
                table.Cell().AlignRight().Text($"{TaxAmount:F2}");

                table.Cell().AlignRight().Text("Total:").Bold();
                table.Cell().AlignRight().Text($"{Total:F2}").Bold();
            });
        }
    }
}