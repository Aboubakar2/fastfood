using SQLite;

namespace fastfood.Data
{
    public class ProductOrder
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [NotNull]
        public long ProviderId { get; set; } // Clé étrangère pour Provider

        [NotNull]
        public int Quantity { get; set; } // Quantité du produit

        [NotNull]
        public DateTime OrderDate { get; set; }

        [NotNull]
        public decimal Price { get; set; } // Prix du produit au moment de la commande
    }
}
