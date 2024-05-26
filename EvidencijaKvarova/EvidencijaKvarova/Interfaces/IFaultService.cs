using EvidencijaKvarova.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = System.Action;

namespace EvidencijaKvarova
{
    public interface IFaultService
    {
        void CreateFault(Fault fault);
        List<Fault> GetFaults(DateTime fromDate, DateTime toDate);
        Fault GetFaultById(string faultId);
        void UpdateFault(Fault fault);
        double CalculatePriority(Fault fault);
        List<Fault> GetAllFaults();
        void CreateFaultsExcelDocument(string outputPath);
    }
}
