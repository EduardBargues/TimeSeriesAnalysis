using Strategy.Investments;

namespace Strategy.Actions
{
    public interface ITransaction
    {
        string ActorId { get; }
        string BrokerId { get; }
        string InvestmentId { get; }
        double Quantity { get; }
        double PricePerUnit { get; }
        TransactionType Type { get; }
    }
}
