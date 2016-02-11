using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace Server.Data
{
    interface IEmployeeRepository
    {
        void Add(Employee employee);
        void Update(Employee employee);
    }
}
