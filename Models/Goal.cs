using System.Text.Json.Serialization;

namespace FinanceApi.Models
{
    public class Goal
    {
        public int Id { get; set; }
        [JsonIgnore]
        public string? UserId { get; set; }
        public string Title { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
