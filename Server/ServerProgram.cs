using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Models;
using Server.Data;
using Server.TransportService;

namespace Server
{
    class ServerProgram
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Server application...");
            Thread.Sleep(1000);
            Task t = Task.Run(async () =>
            {
                // === Read settings ===
                // Emplyees file name.
                string employeesFileName = args[0];
                // Server port.
                string serverPortSetting = args[1];
                int serverPort = int.Parse(serverPortSetting);
                // Data format.
                string dataFormat = args[2];

                // Read Employees data from the XML file.
                string employeesDataText =
                    File.ReadAllText("D:\\DistributionCollections\\" + employeesFileName);

                Employee[] employees = 
                    await UtilityXml.DeserializeXmlAsync<Employee[]>(employeesDataText, rootElementName: "Employees");

                // Add Employees to the repository.
                employees.ToList().ForEach(e =>
                {
                    EmployeeRepository.Instance.Add(e);
                });

                // PUT all employees data to the DataWarehouse.
                foreach (Employee employee in employees)
                {
                    await SendEmployee(employee, dataFormat, serverPort);
                }

                // Run the HTTP Transport Service.
                var httpTransportService = new HttpTransportService(serverPort, dataFormat);
                httpTransportService.Start();
                Thread.Sleep(60);
                httpTransportService.Stop();
            });

            t.Wait();
            Console.ReadLine();
        }

        private static async Task SendEmployee(Employee employee, string dataFormat, int portNumber)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("DataUpdatePort", portNumber.ToString());
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", dataFormat);

                // Construct the request.
                string url = "http://localhost:8080/employee/";
                HttpResponseMessage response = null;
                if (dataFormat == "application/xml")
                {
                    response = await httpClient.PutAsync(url, employee, new XmlMediaTypeFormatter() { UseXmlSerializer = true});
                }
                else if (dataFormat == "application/json")
                {
                    response = await httpClient.PutAsync(url, employee, new JsonMediaTypeFormatter());
                }

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(@"Employee with ID = " + employee.EmployeeId + "was sent to the DataWarehouse.");
                }
                else
                {
                    Console.WriteLine(@"Warning: Unable to send employee with ID = " + employee.EmployeeId + " to the DataWarehouse.");
                }
            }
        }
    }
}
