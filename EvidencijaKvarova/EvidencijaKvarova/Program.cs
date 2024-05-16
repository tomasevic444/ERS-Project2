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

namespace EvidencijaKvarova
{
    class Program
    {
        static void Main(string[] args)
        {
            // File paths
            string faultsFilePath = "faults.xml";
            string elementsFilePath = "elements.xml";
            string configFilePath = "config.xml";

            // Repositories
            IFaultRepository faultRepository = new XmlFaultRepository(faultsFilePath);
            IElementRepository elementRepository = new XmlElementRepository(elementsFilePath);
            IConfigurationService configurationService = new ConfigurationService(configFilePath);

            // Services
            FaultService faultService = new FaultService(faultRepository, elementRepository, configurationService);

            // Sample electrical element
            //ElectricalElement sampleElement1 = new ElectricalElement { Id = "1", Name = "Transistor", Type = "Component", Location = "Novi Sad, Latitude: 45.267136, Longitude: 19.833549" };
            //ElectricalElement sampleElement2 = new ElectricalElement { Id = "2", Name = "Batteries", Type = "Component", Location = "Novi Sad, Latitude: 45.267136, Longitude: 19.833549" };
            //ElectricalElement sampleElement3 = new ElectricalElement { Id = "3", Name = "Resistor", Type = "Component", Location = "Novi Sad, Latitude: 45.267136, Longitude: 19.833549" };
            //ElectricalElement sampleElement4 = new ElectricalElement { Id = "4", Name = "Inductor", Type = "Component", Location = "Novi Sad, Latitude: 45.267136, Longitude: 19.833549" };
            //ElectricalElement sampleElement5 = new ElectricalElement { Id = "5", Name = "Capacitor", Type = "Component", Location = "Novi Sad, Latitude: 45.267136, Longitude: 19.833549" };
            //ElectricalElement sampleElement6 = new ElectricalElement { Id = "6", Name = "Switch", Type = "Component", Location = "Novi Sad, Latitude: 45.267136, Longitude: 19.833549" };

            //elementRepository.AddElement(sampleElement1);
            //elementRepository.AddElement(sampleElement2);
            //elementRepository.AddElement(sampleElement3);
            //elementRepository.AddElement(sampleElement4);
            //elementRepository.AddElement(sampleElement5);
            //elementRepository.AddElement(sampleElement6);

            while (true)
            {
                Console.WriteLine("Outage Management System");
                Console.WriteLine("1. Add a new fault");
                Console.WriteLine("2. Add an action to an existing fault");
                Console.WriteLine("3. List all electrical elements");
                Console.WriteLine("4. Retrieve faults within a date range");
                Console.WriteLine("6. Exit");
                Console.Write("Select an option: ");
                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        AddNewFault(faultService, elementRepository);
                        break;
                    case "2":
                        AddActionToFault(faultService);
                        break;
                    case "3":
                        ListAllElements(elementRepository);
                        break;
                    case "4":
                        RetrieveFaults(faultService);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static void AddNewFault(FaultService faultService, IElementRepository elementRepository)
        {
            ListAllElements(elementRepository);

            Console.Write("Enter the ID of the electrical element where the fault occurred: ");
            string elementId = Console.ReadLine();

            var element = elementRepository.GetElementById(elementId);
            if (element == null)
            {
                Console.WriteLine("Element not found.\n");
                return;
            }

            Console.Write("Enter a short description of the fault: ");
            string shortDescription = Console.ReadLine();

            Fault fault = new Fault
            {
                ShortDescription = shortDescription,
                ElementId = element.Id
            };
            fault.Actions.Add(new EvidencijaKvarova.Models.Action { Time = DateTime.Now, Description = "Initial inspection" });

            faultService.CreateFault(fault);
            Console.WriteLine($"Fault added successfully. Fault ID: {fault.Id}\n");
        }
        static void AddActionToFault(FaultService faultService)
        {
            Console.Write("Enter the fault ID: ");
            string faultId = Console.ReadLine();
            Fault fault = faultService.GetFaultById(faultId);

            if (fault == null)
            {
                Console.WriteLine("Fault not found.\n");
                return;
            }

            Console.Write("Enter a description of the action: ");
            string actionDescription = Console.ReadLine();

            fault.Actions.Add(new EvidencijaKvarova.Models.Action { Time = DateTime.Now, Description = actionDescription });
            faultService.UpdateFault(fault);
            Console.WriteLine("Action added successfully.\n");
        }
        static void ListAllElements(IElementRepository elementRepository)
        {
            var elements = elementRepository.GetAllElements();
            foreach (var element in elements)
            {
                Console.WriteLine($"Element ID: {element.Id}, Name: {element.Name}, Type: {element.Type}, Location: {element.Location}");
            }
            Console.WriteLine();
        }
        static void RetrieveFaults(FaultService faultService)
        {
            Console.Write("Enter start date (yyyy-MM-dd): ");
            DateTime fromDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter end date (yyyy-MM-dd): ");
            DateTime toDate = DateTime.Parse(Console.ReadLine());

            var faults = faultService.GetFaults(fromDate, toDate);
            if (faults.Count == 0)
            {
                Console.WriteLine("No faults found in the specified date range.\n");
            }
            else
            {
                foreach (var fault in faults)
                {
                    Console.WriteLine($"Fault ID: {fault.Id}, Description: {fault.ShortDescription}, Status: {fault.Status}, Created: {fault.CreationTime}");
                }
                Console.WriteLine();
            }
        }

    }
}
    