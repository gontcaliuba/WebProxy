using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Common.Models;
using DataWarehouse.Data;

namespace DataWarehouse.Controllers
{
    public class EmployeesController : ApiController
    {
        private readonly IEmployeeRepository _repository = new EmployeeRepository();

        public HttpResponseMessage Get(int id)
        {
            Employee employee = _repository.Get(id);
            HttpResponseMessage response = CraftGetResponse(employee);

            return response;
        }

        public HttpResponseMessage Get(int offset, int limit)
        {
            List<Employee> employees = _repository.Get()
                .Skip(offset)
                .Take(limit)
                .ToList();
            HttpResponseMessage response = CraftGetResponse(employees);

            return response;
        }

        public IHttpActionResult Put(Employee employee)
        {
            _repository.Add(employee);
            string uri = Url.Route("DefaultApi", new { id = employee.EmployeeId });
            return Created(uri, employee);
        }

        public IHttpActionResult Post(Employee employee, int id)
        {
            _repository.Update(employee, id);
            return Ok();
        }

        private HttpResponseMessage CraftGetResponse(object data)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(60)
            };
            return response;
        }
    }
}
