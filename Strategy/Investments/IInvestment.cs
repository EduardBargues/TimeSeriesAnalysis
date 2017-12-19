using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.Investments
{
    public interface IInvestment
    {
        string Id { get; }
        string Name { get; set; }
        double Quantity { get; set; }
        Stock Stock { get; set; }
    }
}
