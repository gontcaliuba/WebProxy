using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace Server.Data
{
    class EmployeeRepository : IEmployeeRepository
    {
        private static readonly EmployeeRepository _instance = new EmployeeRepository();
        private readonly ConcurrentDictionary<int, Employee> _employees = new ConcurrentDictionary<int, Employee>();

        private EmployeeRepository()
        { }

        public static EmployeeRepository Instance
        {
            get { return _instance; }
        }

        public void Add(Employee employee)
        {
            if (!_employees.TryAdd(employee.EmployeeId, employee))
            {
                throw new Exception("ERROR: Couldn't add Employee to the dictionary.");
            }
        }

        public void Update(Employee employee)
        {
            Employee employeeToUpdate = null;
            if (_employees.TryGetValue(employee.EmployeeId, out employeeToUpdate))
            {
                employeeToUpdate = employee;
            }
            else
            {
                throw new Exception("ERROR: Couldn't find the Employee to update.");
            }
        }
    }
}
