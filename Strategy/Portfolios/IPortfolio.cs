using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.Investments;

namespace Strategy.Portfolios
{
    public interface IPortfolio
    {
        IEnumerable<IInvestment> Investments { get; }
        double GetLiquidity();
        void Buy(string investmentId, double volume, double pricePerUnit);
        void Sell(string investmentId, double volume, double pricePerUnit);
    }
}
