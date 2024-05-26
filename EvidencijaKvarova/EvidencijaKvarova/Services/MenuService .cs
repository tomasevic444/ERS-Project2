using EvidencijaKvarova.Interfaces;
using EvidencijaKvarova.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencijaKvarova.Services
{
    public class MenuService
    {
        private readonly FaultService _faultService;
        private readonly IElementRepository _elementRepository;
        private readonly IUserInterface _userInterface;

        public MenuService(FaultService faultService, IElementRepository elementRepository, IUserInterface userInterface)
        {
            _faultService = faultService;
            _elementRepository = elementRepository;
            _userInterface = userInterface;
        }

        public void DisplayMenu()
        {
            while (true)
            {
                _userInterface.DisplayMenu();
                Console.Write("Select an option: ");
                var option = _userInterface.GetUserInput();

                switch (option)
                {
                    case "1":
                        AddNewFault();
                        break;
                    case "2":
                        AddActionToFault();
                        break;
                    case "3":
                        ListAllElements();
                        break;
                    case "4":
                        RetrieveFaults();
                        break;
                    case "5":
                        SelectAndUpdateFault();
                        break;
                    case "6":
                        CreateExcelDocument();
                        break;
                    case "7":
                        return;
                    default:
                        _userInterface.ShowMessage("Invalid option. Please try again.");
                        break;
                }
            }
        }

        public void AddNewFault()
        {
            ListAllElements();

            _userInterface.ShowMessage("Enter the ID of the electrical element where the fault occurred: ");
            string elementId = _userInterface.GetUserInput();

            var element = _elementRepository.GetElementById(elementId);
            if (element == null)
            {
                _userInterface.ShowMessage("Element not found.\n");
                return;
            }

            _userInterface.ShowMessage("Enter a short description of the fault: ");
            string shortDescription = _userInterface.GetUserInput();

            _userInterface.ShowMessage("Enter a detailed description of the fault: ");
            string detailedDescription = _userInterface.GetUserInput();

            Fault fault = new Fault
            {
                ShortDescription = shortDescription,
                DetailedDescription = detailedDescription,
                ElementId = element.Id
            };
            fault.Actions.Add(new EvidencijaKvarova.Models.Action { Time = DateTime.Now, Description = "Initial inspection" });

            _faultService.CreateFault(fault);
            _userInterface.ShowMessage($"Fault added successfully. Fault ID: {fault.Id}\n");
        }

        private void AddActionToFault()
        {
            var faults = _faultService.GetAllFaults();
            if (faults.Count == 0)
            {
                _userInterface.ShowMessage("No faults found.\n");
                return;
            }
            _userInterface.ShowMessage("List of faults");
            foreach (var f in faults)
            {
                _userInterface.ShowMessage($"Fault ID: {f.Id}, Short description: {f.ShortDescription}, Description: {f.DetailedDescription}, Status: {f.Status}, Created: {f.CreationTime}");
            }
            _userInterface.ShowMessage("Enter the fault ID: ");
            string faultId = _userInterface.GetUserInput();
            Fault fault = _faultService.GetFaultById(faultId);

            if (fault == null)
            {
                _userInterface.ShowMessage("Fault not found.\n");
                return;
            }

            _userInterface.ShowMessage("Enter a description of the action: ");
            string actionDescription = _userInterface.GetUserInput();

            fault.Actions.Add(new EvidencijaKvarova.Models.Action { Time = DateTime.Now, Description = actionDescription });
            _faultService.UpdateFault(fault);
            _userInterface.ShowMessage("Action added successfully.\n");
        }

        private void ListAllElements()
        {
            var elements = _elementRepository.GetAllElements();
            foreach (var element in elements)
            {
                _userInterface.ShowMessage($"Element ID: {element.Id}, Name: {element.Name}, Type: {element.Type}, Location: {element.Location}");
            }
            _userInterface.ShowMessage("");
        }

        private void RetrieveFaults()
        {
            try
            {
                _userInterface.ShowMessage("Enter start date (yyyy-MM-dd): ");
                DateTime fromDate;
                if (!DateTime.TryParse(_userInterface.GetUserInput(), out fromDate))
                {
                    _userInterface.ShowMessage("Invalid start date format. Please enter date in yyyy-MM-dd format.\n");
                    return; // Exit the method if parsing fails
                }

                _userInterface.ShowMessage("Enter end date (yyyy-MM-dd): ");
                DateTime toDate;
                if (!DateTime.TryParse(_userInterface.GetUserInput(), out toDate))
                {
                    _userInterface.ShowMessage("Invalid end date format. Please enter date in yyyy-MM-dd format.\n");
                    return; // Exit the method if parsing fails
                }

                if (toDate < fromDate)
                {
                    _userInterface.ShowMessage("End date cannot be before start date.\n");
                    return; // Exit the method if end date is before start date
                }

                var faults = _faultService.GetFaults(fromDate, toDate);
                if (faults == null)
                {
                    _userInterface.ShowMessage("Error occurred while retrieving faults.\n");
                    return; // Exit the method if fault retrieval fails
                }

                if (faults.Count == 0)
                {
                    _userInterface.ShowMessage("No faults found in the specified date range.\n");
                }
                else
                {
                    foreach (var fault in faults)
                    {
                        _userInterface.ShowMessage($"Fault ID: {fault.Id}, Short description: {fault.ShortDescription}, Description: {fault.DetailedDescription}, Status: {fault.Status}, Created: {fault.CreationTime}");
                    }
                    _userInterface.ShowMessage("");
                }
            }
            catch (Exception ex)
            {
                _userInterface.ShowMessage($"An error occurred: {ex.Message}\n");
            }
        }

        private void SelectAndUpdateFault()
        {
            var faults = _faultService.GetAllFaults();
            if (faults.Count == 0)
            {
                _userInterface.ShowMessage("No faults found.\n");
                return;
            }

            _userInterface.ShowMessage("List of faults:\n");
            foreach (var f in faults)
            {
                _userInterface.ShowMessage($"Fault ID: {f.Id}, Short description: {f.ShortDescription},Description: {f.DetailedDescription}, Status: {f.Status}, Created: {f.CreationTime}");
            }

            _userInterface.ShowMessage("Enter the fault ID to select: ");
            string faultId = _userInterface.GetUserInput();

            var selectedFault = _faultService.GetFaultById(faultId);
            if (selectedFault == null)
            {
                _userInterface.ShowMessage("Fault not found.\n");
                return;
            }

            _userInterface.ShowMessage("Fault details:");
            _userInterface.ShowMessage($"ID: {selectedFault.Id}");
            _userInterface.ShowMessage($"Short Description: {selectedFault.ShortDescription}");
            _userInterface.ShowMessage($"Description: {selectedFault.DetailedDescription}");
            _userInterface.ShowMessage($"Status: {selectedFault.Status}");
            _userInterface.ShowMessage($"Element ID: {selectedFault.ElementId}");
            _userInterface.ShowMessage($"Creation Time: {selectedFault.CreationTime}");
            _userInterface.ShowMessage("Actions:");
            foreach (var action in selectedFault.Actions)
            {
                _userInterface.ShowMessage($"- {action.Time}: {action.Description}");
            }

            if (selectedFault.Status == "Zatvoreno")
            {
                _userInterface.ShowMessage("Fault is closed and cannot be updated. Details are read-only.\n");
            }
            else
            {
                CalculateFaultPriority(faultId);
                _userInterface.ShowMessage("Do you want to update this fault? (yes/no): ");
                string response = _userInterface.GetUserInput().Trim().ToLower();

                if (response == "yes")
                {
                    // Proceed with update
                    _userInterface.ShowMessage("\nUpdate fault details:");
                    _userInterface.ShowMessage("Enter a new short description: ");
                    selectedFault.ShortDescription = _userInterface.GetUserInput();
                    _userInterface.ShowMessage("Enter a new  description: ");
                    selectedFault.ShortDescription = _userInterface.GetUserInput();

                    _userInterface.ShowMessage("Enter a new status (Nepotvrđen, U popravci, Testiranje, Zatvoreno): ");
                    selectedFault.Status = _userInterface.GetUserInput();

                    _faultService.UpdateFault(selectedFault);
                    _userInterface.ShowMessage("Fault updated successfully.\n");
                }
                else
                {
                    _userInterface.ShowMessage("Update cancelled.\n");
                }
            }
        }

        private void CalculateFaultPriority(string faultId)
        {
            var fault = _faultService.GetFaultById(faultId);
            if (fault == null)
            {
                _userInterface.ShowMessage("Fault not found.\n");
                return;
            }

            if (fault.Status != "U popravci" && fault.Status != "Testiranje")
            {
                _userInterface.ShowMessage("Priority calculation is only applicable for faults with status 'U popravci' or 'Testiranje'.\n");
                return;
            }

            double priority = _faultService.CalculatePriority(fault);
            _userInterface.ShowMessage($"The priority of the fault is: {priority}\n");
        }

        private void CreateExcelDocument()
        {
            string excelPath = "faultsExcel.xlsx";
            _faultService.CreateFaultsExcelDocument(excelPath);
            _userInterface.ShowMessage("Excel document created successfully.");
        }
    }
}
