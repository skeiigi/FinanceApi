namespace FinanceApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Связь с пользователем
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Merchant { get; set; }
        public DateTime Date { get; set; }
    }
}
