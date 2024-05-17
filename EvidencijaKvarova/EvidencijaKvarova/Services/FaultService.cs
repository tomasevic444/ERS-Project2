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
            fault.Id = GenerateFaultId(fault);
            fault.CreationTime = DateTime.Now;
            fault.Status = "Nepotvrđen";
            _faultRepository.AddFault(fault);
        }

        public List<Fault> GetFaults(DateTime fromDate, DateTime toDate)
        {
            return _faultRepository.GetFaults(fromDate, toDate);
        }

        public Fault GetFaultById(string faultId)
        {
            return _faultRepository.GetFaultById(faultId);
        }

        public void UpdateFault(Fault fault)
        {
            _faultRepository.UpdateFault(fault);
        }

        public double CalculatePriority(Fault fault)
        {
            double priority = 0;

            if (fault.Status == "U popravci")
            {
                foreach (var action in fault.Actions)
                {
                    priority += _configurationService.GetPriorityIncrementForRepair();
                }
            }
            else if (fault.Status == "Testiranje")
            {
                foreach (var action in fault.Actions)
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

        private string GenerateFaultId(Fault fault)
        {
            DateTime now = DateTime.Now;
            int dailyCount = GetDailyFaultCount(now) + 1;
            return $"{now:yyyyMMddHHmmss}_{dailyCount}";
        }

        private int GetDailyFaultCount(DateTime date)
        {
            DateTime startOfDay = date.Date;
            DateTime endOfDay = date.Date.AddDays(1).AddTicks(-1);
            var faultsToday = _faultRepository.GetFaults(startOfDay, endOfDay);
            return faultsToday.Count;
        }

        public void AddActionToFault(string faultId, EvidencijaKvarova.Models.Action action)
        {
            var fault = _faultRepository.GetFaultById(faultId);
            if (fault == null)
            {
                throw new Exception("Fault not found.");
            }

            fault.Actions.Add(action);
            fault.Status = "U popravci"; // Update status to "U popravci"
            _faultRepository.UpdateFault(fault);
        }
        public List<Fault> GetAllFaults()
        {
            return _faultRepository.GetAllFaults();
        }

    }
}

