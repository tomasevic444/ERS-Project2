using EvidencijaKvarova.Config;
using EvidencijaKvarova.Interfaces;
using EvidencijaKvarova.Klase;
using EvidencijaKvarova.Repositories;
using EvidencijaKvarova.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvidencijaKvarova.Models;
using EvidencijaKvarova.Input;

namespace EvidencijaKvarova
{
    class Program
    {
        static void Main(string[] args)
        {
            string faultsFilePath = "faults.xml";
            string elementsFilePath = "elements.xml";
            string configFilePath = "config.xml";

            // Repositories
            IFaultRepository faultRepository = new XmlFaultRepository(faultsFilePath);
            IElementRepository elementRepository = new XmlElementRepository(elementsFilePath);
            IConfigurationService configurationService = new ConfigurationService(configFilePath);

            // Services
            FaultService faultService = new FaultService(faultRepository, elementRepository, configurationService);

            // User Interface
            IUserInterface userInterface = new ConsoleUserInterface();

            // Menu Service
            MenuService menuService = new MenuService(faultService, elementRepository, userInterface);

            // Start menu
            menuService.DisplayMenu();
        }
    }
}
    