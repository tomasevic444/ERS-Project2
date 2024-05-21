using EvidencijaKvarova.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencijaKvarova.Input
{
    public class ConsoleUserInterface : IUserInterface
    {
        public void DisplayMenu()
        {
            Console.WriteLine("Outage Management System");
            Console.WriteLine("1. Add a new fault");
            Console.WriteLine("2. Add an action to an existing fault");
            Console.WriteLine("3. List all electrical elements");
            Console.WriteLine("4. Retrieve faults within a date range");
            Console.WriteLine("5. Select and update a fault");
            Console.WriteLine("6. Create Excel document for faults");
            Console.WriteLine("7. Exit");
        }

        public string GetUserInput()
        {

            return Console.ReadLine();
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
