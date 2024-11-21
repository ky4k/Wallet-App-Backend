namespace DAL.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CardBalance { get; set; }
        public decimal CardLimit { get; set; } = 1500m;
        public int Points { get; set; } = 0;
        public DateTime LastPointsDate { get; set; } = DateTime.MinValue;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
