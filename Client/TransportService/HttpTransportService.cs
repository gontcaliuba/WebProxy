using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Common;
using Common.Models;

namespace Client.TransportService
{
    class HttpTransportService : ITransportService
    {
        private readonly HttpClient _httpClient;
        private readonly string _dataFormat;
        private readonly int _dataWarehousePortNumber;

        public HttpTransportService(string dataFormat, int dataWarehousePortNumber)
        {
            _httpClient = new HttpClient();
            _dataFormat = dataFormat;
            _dataWarehousePortNumber = dataWarehousePortNumber;
        }

        public async Task<List<Employee>> Get(int offset, int limit)
        {
            // Set the Accept header.
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_dataFormat));
            string url = @"http://localhost:" +_dataWarehousePortNumber + "/api/employees/?offset= " + offset + "&limit= " + limit;
            // Make a remote request.
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            // Read response body and deserialize it.
            string body = await response.Content.ReadAsStringAsync();

            if (_dataFormat == "application/xml")
            {
                // Validate XML document.
                ValidateXmlSchema(body);
                // Deserialize XML document.
                Employee[] employees = await UtilityXml.DeserializeXmlAsync<Employee[]>(body);
                return employees.ToList();
            }

            if (_dataFormat == "application/json")
            {
                Employee[] employees = await UtilityJson.DeserializeJsonAsync<Employee[]>(body);
                return employees.ToList();
            }

            // Return empty list.
            return new List<Employee>();
        }

        public async Task<Employee> Get(int id)
        {
            // Set the Accept header.
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_dataFormat));
            string url = @"http://localhost:" + _dataWarehousePortNumber + "/api/employees/" + id;
            // Make a remote request.
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            // Read response body and deserialize it.
            string body = await response.Content.ReadAsStringAsync();

            if (_dataFormat == "application/xml")
            {
                // Validate XML document.
                ValidateXmlSchema(body);
                // Deserialize XML document.
                Employee employee = await UtilityXml.DeserializeXmlAsync<Employee>(body);
                return employee;
            }

            if (_dataFormat == "application/json")
            {
                Employee employee = await UtilityJson.DeserializeJsonAsync<Employee>(body);
                return employee;
            }

            return null;
        }

        public async Task<bool> IncreaseSalary(int id)
        {
            // Retreive list of all employees from the DataWarehouse.
            List<Employee> employees = await Get(0, int.MaxValue);
            // Find the target employee.
            Employee employee = employees.First(e => e.EmployeeId == id);
            // Increase his/her salary by 10%.
            employee.Salary += employee.Salary * 0.1m;

            // === Make a remote request to update the Employee.

            // Set the Accept and Content-Type headers.
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_dataFormat));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", _dataFormat);
            string url = @"http://localhost:" + _dataWarehousePortNumber + "/api/employees/" + id;
            HttpResponseMessage response = null;

            if (_dataFormat == "application/xml")
            {
                response = await _httpClient.PostAsync(url, employee, new XmlMediaTypeFormatter() { UseXmlSerializer = true });
            }
            else if (_dataFormat == "application/json")
            {
                response = await _httpClient.PostAsync(url, employee, new JsonMediaTypeFormatter());
            }

            return response != null && response.IsSuccessStatusCode;
        }

        private void ValidateXmlSchema(string message)
        {
            // Validate the XML document.
            // Read schema files.
            string employeeSchema = File.ReadAllText("D:\\DistributionCollections\\Employee_schema.xsd");
            string employeesSchema = File.ReadAllText("D:\\DistributionCollections\\Employees_schema.xsd");

            XDocument doc = XDocument.Parse(message);
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add("", XmlReader.Create(new StringReader(employeeSchema)));
            xmlSchemaSet.Add("", XmlReader.Create(new StringReader(employeesSchema)));

            try
            {
                doc.Validate(xmlSchemaSet, validationEventHandler: null);
            }
            catch (Exception)
            {
                throw new Exception("ERROR: XML Schema validation failed...");
            }
        }
    }
}
