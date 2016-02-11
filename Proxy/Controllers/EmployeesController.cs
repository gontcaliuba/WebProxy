using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Models;
using Proxy.Data;
using Proxy.Infrastructure;
using ServiceStack.Text;

namespace Proxy.Controllers
{
    public class EmployeesController : ApiController
    {
        private readonly ICacheRepository _cacheRepository = new CacheRepository();
        private static readonly RoundRobinList _dataWarehousesPorts = new RoundRobinList(new int[] { 8080, 8081 });

        public async Task<IHttpActionResult> Get(int id)
        {
            return await ProcessGetRequest<Employee>();
        }

        public async Task<IHttpActionResult> Get(int offset, int limit)
        {
            return await ProcessGetRequest<Employee[]>();
        }

        public async Task<IHttpActionResult> Put(Employee employee)
        {
            // Extract the request URL.
            string path = Request.RequestUri.AbsolutePath;

            // Choose DataWarehouse server.
            int dateWarehousePort = _dataWarehousesPorts.Next();

            using (HttpClient httpClient = new HttpClient())
            {
                // Set the base address and the Accept header.
                httpClient.BaseAddress = new Uri($"http://localhost:{dateWarehousePort}");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.PutAsJsonAsync(path, employee);
                Employee savedEmployee = await response.Content.ReadAsAsync<Employee>();
                if (response.IsSuccessStatusCode)
                {
                    string uri = Url.Route("DefaultApi", new { id = savedEmployee.EmployeeId });
                    return Created(uri, savedEmployee);
                }
            }

            return InternalServerError();
        }

        public async Task<IHttpActionResult> Post(Employee employee, int id)
        {
            // Extract the request URL.
            string path = Request.RequestUri.AbsolutePath;

            // Choose DataWarehouse server.
            int dateWarehousePort = _dataWarehousesPorts.Next();

            using (HttpClient httpClient = new HttpClient())
            {
                // Set the base address and the Accept header.
                httpClient.BaseAddress = new Uri($"http://localhost:{dateWarehousePort}");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.PostAsJsonAsync(path, employee);
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
            }

            return InternalServerError();
        }

        private async Task<IHttpActionResult> ProcessGetRequest<T>()
        {
            // Extract the request URL.
            string path = Request.RequestUri.PathAndQuery;
            // Try to find the cached response.
            T cachedData = _cacheRepository.Get<T>(path);
            // Send cached response.
            if (cachedData != null)
            {
                // === Return data.
                return Ok(cachedData);
            }

            // Choose DataWarehouse server.
            int dateWarehousePort = _dataWarehousesPorts.Next();

            // Make request to the Data Warehouse.
            HttpResponseMessage response = null;
            using (HttpClient httpClient = new HttpClient())
            {
                // Set the base address and the Accept header.
                httpClient.BaseAddress = new Uri($"http://localhost:" + dateWarehousePort + "/");
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Make request to the DataWarehouse.
                response = await httpClient.GetAsync(path.Substring(1));
            }

            if (response.IsSuccessStatusCode)
            {
                // Cache the response.
                T data = await CacheResponseBody<T>(response, path);

                // Return data.
                return Ok(data);
            }

            return InternalServerError();
        }

        private async Task<T> CacheResponseBody<T>(HttpResponseMessage response, string path)
        {
            // Read response body (content).
            T data = await response.Content.ReadAsAsync<T>();
            // Read the Cache-Control max-age value.
            TimeSpan? expiresIn = GetMaxAge(response);

            // Save to the cache.
            _cacheRepository.Add<T>(path, data, expiresIn.Value);
            return data;
        }

        // Extracts the Max-Age value of the Cache-Control header
        // or sets the default value if no such header was provided in the response.
        private static TimeSpan? GetMaxAge(HttpResponseMessage response)
        {
            TimeSpan? expiresIn;
            CacheControlHeaderValue cacheControlHeader = response.Headers.CacheControl;
            if (cacheControlHeader != null)
            {
                expiresIn = cacheControlHeader.MaxAge;
            }
            else
            {
                expiresIn = TimeSpan.FromSeconds(60);
            }

            return expiresIn;
        }
    }
}
