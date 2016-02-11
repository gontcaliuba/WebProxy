using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace DataWarehouse
{
    class Program
    {
        static void Main(string[] args)
        {
            int portNumber = 8080;

            using (WebApp.Start<Startup>(@"http://localhost:" + portNumber))
            {
                Console.WriteLine(@"Web Server is running on port " + portNumber + ".");
                Console.WriteLine("Press any key to quit.");
                Console.ReadLine();
            }
        }
    }
}
