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
            string excelPath = "faultsExcel.xlsx";

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
                Console.WriteLine("5. Select and update a fault");
                Console.WriteLine("6. Create Excel document for faults");
                Console.WriteLine("7. Exit");
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
                    case "5":
                        SelectAndUpdateFault(faultService);
                        break;
                    case "6":
                        faultService.CreateFaultsExcelDocument(excelPath);
                        Console.WriteLine("Excel document created successfully.");
                        break;
                    case "7":
                        return;
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
            var faults = faultService.GetAllFaults();
            if (faults.Count == 0)
            {
                Console.WriteLine("No faults found.\n");
                return;
            }
            Console.Write("List of faults\n");
            foreach (var f in faults)
            {
                Console.WriteLine($"Fault ID: {f.Id}, Description: {f.ShortDescription}, Status: {f.Status}, Created: {f.CreationTime}");
            }
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
        static void SelectAndUpdateFault(FaultService faultService)
        {
            var faults = faultService.GetAllFaults();
            if (faults.Count == 0)
            {
                Console.WriteLine("No faults found.\n");
                return;
            }

            // Display all faults
            Console.WriteLine("List of faults:\n");
            foreach (var f in faults)
            {
                Console.WriteLine($"Fault ID: {f.Id}, Description: {f.ShortDescription}, Status: {f.Status}, Created: {f.CreationTime}");
            }

            // Select a fault
            Console.Write("Enter the fault ID to select: ");
            string faultId = Console.ReadLine();

            var selectedFault = faultService.GetFaultById(faultId);
            if (selectedFault == null)
            {
                Console.WriteLine("Fault not found.\n");
                return;
            }

            // Display fault details
            Console.WriteLine("Fault details:");
            Console.WriteLine($"ID: {selectedFault.Id}");
            Console.WriteLine($"Short Description: {selectedFault.ShortDescription}");
            Console.WriteLine($"Status: {selectedFault.Status}");
            Console.WriteLine($"Element ID: {selectedFault.ElementId}");
            Console.WriteLine($"Creation Time: {selectedFault.CreationTime}");
            Console.WriteLine("Actions:");
            foreach (var action in selectedFault.Actions)
            {
                Console.WriteLine($"- {action.Time}: {action.Description}");
            }

            if (selectedFault.Status == "Zatvoreno")
            {
                Console.WriteLine("Fault is closed and cannot be updated. Details are read-only.\n");
            }
            else
            {
                CalculateFaultPriority(faultService, faultId);
                Console.Write("Do you want to update this fault? (yes/no): ");
                string response = Console.ReadLine().Trim().ToLower();

                if (response == "yes")
                {
                    // Proceed with update
                    Console.WriteLine("\nUpdate fault details:");
                    Console.Write("Enter a new short description: ");
                    selectedFault.ShortDescription = Console.ReadLine();

                    Console.Write("Enter a new status (Nepotvrđen, U popravci, Testiranje, Zatvoreno): ");
                    selectedFault.Status = Console.ReadLine();

                    faultService.UpdateFault(selectedFault);
                    Console.WriteLine("Fault updated successfully.\n");
                }
                else
                {
                    Console.WriteLine("Update cancelled.\n");
                }
            }
        }
        static void CalculateFaultPriority(FaultService faultService, string faultId)
        {
            var fault = faultService.GetFaultById(faultId);
            if (fault == null)
            {
                Console.WriteLine("Fault not found.\n");
                return;
            }

            if (fault.Status != "U popravci" && fault.Status != "Testiranje")
            {
                Console.WriteLine("Priority calculation is only applicable for faults with status 'U popravci' or 'Testiranje'.\n");
                return;
            }

            double priority = faultService.CalculatePriority(fault);
            Console.WriteLine($"The priority of the fault is: {priority}\n");
        }

    }
}
    