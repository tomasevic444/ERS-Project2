using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvidencijaKvarova.Models;

namespace EvidencijaKvarova.Interfaces
{
    public interface IFaultRepository
    {
        void AddFault(Fault fault);
        Fault GetFaultById(string id);
        List<Fault> GetFaults(DateTime from, DateTime to);
        void UpdateFault(Fault fault);
    }
}
