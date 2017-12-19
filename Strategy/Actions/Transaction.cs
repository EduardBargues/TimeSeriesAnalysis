namespace Strategy.Actions
{
    public class Transaction : ITransaction
    {
        public string ActorId { get; set; }
        public string BrokerId { get; set; }
        public string InvestmentId { get; set; }
        public double Quantity { get; set; }
        public double PricePerUnit { get; set; }
        public TransactionType Type { get; set; }
    }
}