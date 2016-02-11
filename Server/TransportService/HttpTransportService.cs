using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Models;
using Server.Data;

namespace Server.TransportService
{
    class HttpTransportService : ITransportService
    {
        private HttpListener _httpListener;
        private int _portNumber;
        private string _dataFormat;
        private IEmployeeRepository _repository = EmployeeRepository.Instance;
        public bool IsStopped { get; private set; }

        public HttpTransportService(int portNumber, string dataFormat)
        {
            _portNumber = portNumber;
            _dataFormat = dataFormat;
        }

        public void Start()
        {
            Task.Run(() =>
            {
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add(@"http://localhost:" +_portNumber +"/");
                _httpListener.Start();
                Console.WriteLine("The Http Transport Service has started...");

                while (!IsStopped)
                {
                    // Wait for the incoming request.
                    HttpListenerContext context = _httpListener.GetContext();
                    HttpListenerRequest request = context.Request;
                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;

                    // Process the POST request to update the Employee object.
                    if (request.HttpMethod == "POST")
                    {
                        UpdateEmployee(request, response);
                    }
                }
            });
        }

        private void UpdateEmployee(HttpListenerRequest request, HttpListenerResponse response)
        {
            // Deserialize Employee object.
            string body = String.Empty;
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                body = reader.ReadToEnd();
            }

            Employee employee = null;
            try
            {
                if (_dataFormat == "application/xml")
                {
                    employee = UtilityXml.DeserializeXml<Employee>(body);
                }
                else if (_dataFormat == "application/json")
                {
                    employee = UtilityJson.DeserializeJson<Employee>(body);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"ERROR: Failed to deserialize the Employee object - " + ex.Message);
            }

            if (employee == null)
            {
                response.StatusCode = 400;
                response.Close();
                return;
            }

            // Perform the actual update.
            _repository.Update(employee);

            // Construct and send the response.
            response.StatusCode = 200;
            response.Close();

            Console.WriteLine(@"POST: Updated employee with ID " + employee.EmployeeId);
        }

        public void Stop()
        {
            IsStopped = true;
            _httpListener.Stop();
            Console.WriteLine("The Http Transport Service has been stopped...");
        }
    }
}
