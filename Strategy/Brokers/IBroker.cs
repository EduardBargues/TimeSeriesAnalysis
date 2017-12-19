using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.Actions;
using Strategy.Actors;
using Strategy.Portfolios;

namespace Strategy.Brokers
{
    public interface IBroker
    {
        double GetFee(ITransaction action);
        IPortfolio GetClientPortfolio(string clientId);
        IActor GetClient(string clientId);
    }
}
