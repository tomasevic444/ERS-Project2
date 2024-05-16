using EvidencijaKvarova.Interfaces;
using EvidencijaKvarova.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencijaKvarova.Services
{
    public class FaultService
    {
        private readonly IFaultRepository _faultRepository;
        private readonly IElementRepository _elementRepository;
        private readonly IConfigurationService _configurationService;

        public FaultService(IFaultRepository faultRepository, IElementRepository elementRepository, IConfigurationService configurationService)
        {
            _faultRepository = faultRepository;
            _elementRepository = elementRepository;
            _configurationService = configurationService;
        }

        public void CreateFault(Fault fault)
        {
            fault.Id = GenerateFaultId();
            fault.CreationTime = DateTime.Now;
            fault.Status = "Nepotvrdjen";  // Default status
            _faultRepository.AddFault(fault);
        }

        public Fault GetFaultById(string id)
        {
            return _faultRepository.GetFaultById(id);
        }

        public List<Fault> GetFaults(DateTime from, DateTime to)
        {
            return _faultRepository.GetFaults(from, to);
        }

        public void UpdateFault(Fault fault)
        {
            _faultRepository.UpdateFault(fault);
        }

        public double CalculatePriority(Fault fault)
        {
            double priority = 0;
            foreach (var action in fault.Actions)
            {
                if (fault.Status == "U popravci")
                {
                    priority += _configurationService.GetPriorityIncrementForRepair();
                }
                else if (fault.Status == "Testiranje")
                {
                    priority += _configurationService.GetPriorityIncrementForTesting();
                }
            }

            var element = _elementRepository.GetElementById(fault.ElementId);
            if (element != null && element.VoltageLevel == "visoki napon")
            {
                priority += _configurationService.GetPriorityIncrementForHighVoltage();
            }

            return priority;
        }

        private string GenerateFaultId()
        {
            DateTime now = DateTime.Now;
            string datePart = now.ToString("yyyyMMddHHmmss");
            int countForToday = GetFaultCountForToday(now);
            return $"{datePart}_{countForToday + 1}";
        }

        private int GetFaultCountForToday(DateTime today)
        {
            var startOfDay = today.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            var faultsToday = _faultRepository.GetFaults(startOfDay, endOfDay);
            return faultsToday.Count;
        }
    }
}

