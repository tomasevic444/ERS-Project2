using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencijaKvarova.Interfaces
{
    public interface IConfigurationService
    {
        double GetPriorityIncrementForRepair();
        double GetPriorityIncrementForTesting();
        double GetPriorityIncrementForHighVoltage();
    }
}
