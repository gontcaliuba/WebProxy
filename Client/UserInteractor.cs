using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.TransportService;
using Common.Models;

namespace Client
{
    class UserInteractor
    {
        private ITransportService _transportService;

        public UserInteractor(ITransportService transportService)
        {
            _transportService = transportService;
        }

        public async Task UpdateEmployee()
        {
            Console.Write("Enter the ID of employee: ");
            int id = int.Parse(Console.ReadLine());
            bool success = await _transportService.IncreaseSalary(id);
            if (success)
            {
                Console.WriteLine("The Employee was updated successfully.");
            }
            else
            {
                Console.WriteLine("An error has occured while updating the Employee.");
            }
        }

        public async Task RetreiveEmployee()
        {
            Console.Write("Enter the ID of employee: ");
            int id = int.Parse(Console.ReadLine());

            // Retreive Employee.
            Employee employee = await _transportService.Get(id);
            PrintEmployee(employee);
        }

        public async Task RetreiveEmployees()
        {
            Console.Write("How many employees to skip? ");
            int offset = int.Parse(Console.ReadLine());
            Console.Write("How many employees to show? ");
            int limit = int.Parse(Console.ReadLine());
            List<Employee> employees = await _transportService.Get(offset, limit);

            int i = 1;
            employees.ForEach(employee =>
            {
                Console.WriteLine("{0}.", i++);
                PrintEmployee(employee);
            });
        }

        public void PrintEmployee(Employee employee)
        {
            Console.WriteLine("ID: " + employee.EmployeeId);
            Console.WriteLine("First Name: " + employee.FirstName);
            Console.WriteLine("Last Name: " + employee.LastName);
            Console.WriteLine("Department: " + employee.Department);
            Console.WriteLine("Salary: " + employee.Salary);
            Console.WriteLine("------------");
        }
    }
}
