using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace Client.TransportService
{
    interface ITransportService
    {
        Task<List<Employee>> Get(int offset, int limit);
        Task<Employee> Get(int id);
        Task<bool> IncreaseSalary(int id);
    }
}
