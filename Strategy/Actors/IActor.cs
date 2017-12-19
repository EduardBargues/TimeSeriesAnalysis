using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.Brokers;
using Strategy.Portfolios;

namespace Strategy.Actors
{
    public interface IActor
    {
        string Name { get; }
        IEnumerable<IBroker> Brokers { get; }
    }
}
