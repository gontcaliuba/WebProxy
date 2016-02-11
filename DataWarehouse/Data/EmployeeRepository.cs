using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using Common.Models;

namespace DataWarehouse.Data
{
    class EmployeeRepository : IEmployeeRepository
    {
        private readonly ISession _session;

        public EmployeeRepository()
        {
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            _session = cluster.Connect("proxyapp");
        }

        public void Add(Employee employee)
        {
            _session.Execute("INSERT INTO employees (EmployeeId, Department, FirstName, LastName, salary) " +
                             "VALUES ({employee.EmployeeId}, '{employee.Department}', " +
                             "'{employee.FirstName}', '{employee.LastName}', {employee.Salary});");
        }

        public List<Employee> Get()
        {
            List<Employee> employees = new List<Employee>();
            RowSet rows = _session.Execute("SELECT * FROM employees;");
            foreach (Row row in rows)
            {
                employees.Add(new Employee()
                {
                    EmployeeId = (int)row["employeeid"],
                    Department = (string)row["department"],
                    FirstName = (string)row["firstname"],
                    LastName = (string)row["lastname"],
                    Salary = (decimal)row["salary"]
                });
            }

            return employees;
        }

        public Employee Get(int id)
        {
            Row row = _session.Execute("SELECT * FROM employees WHERE employeeid = " + id +";").First();
            return new Employee()
            {
                EmployeeId = (int)row["employeeid"],
                Department = (string)row["department"],
                FirstName = (string)row["firstname"],
                LastName = (string)row["lastname"],
                Salary = (decimal)row["salary"]
            };
        }

        public void Update(Employee employee, int id)
        {
            string str = "UPDATE employees SET department = '" + employee.Department + "', " +
                             "firstname = '" + employee.FirstName + "', lastname = '" + employee.LastName + "', " +
                             "salary = " + (int)employee.Salary + " WHERE employeeid = " + id + ";";
            _session.Execute(str);
        }
    }
}
