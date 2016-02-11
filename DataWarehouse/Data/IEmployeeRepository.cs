using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace DataWarehouse.Data
{
    interface IEmployeeRepository
    {
        void Add(Employee employee);
        List<Employee> Get();
        Employee Get(int id);
        void Update(Employee employee, int id);
    }
}
