using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.TransportService;
using Common.Models;

namespace Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client application...");

            // Read command line parameters.
            int startupDelay = int.Parse(args[0]);
            string dataFormat = args[1];
            int dataWarehousePortNumber = int.Parse(args[2]);
            

            Thread.Sleep(startupDelay);
            Task t = Task.Run(async() =>
            {
                ITransportService transportService = 
                    new HttpTransportService(dataFormat, dataWarehousePortNumber);
                var userInteractor = new UserInteractor(transportService);
                string input = String.Empty;
                do
                {
                    Console.WriteLine(
                        "\nChoose an option:" +
                        "\n1. Retreive the list of Employees from the DataWarehouse" +
                        "\n2. Retreive data for the specified Employee" +
                        "\n3. Update an Employee using its ID" +
                        "\n4. Exit");
                    try
                    {
                        input = Console.ReadLine();
                        switch (input)
                        {
                            case "1":
                                await userInteractor.RetreiveEmployees();
                                break;
                            case "2":
                                await userInteractor.RetreiveEmployee();
                                break;
                            case "3":
                                await userInteractor.UpdateEmployee();
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                } while (input != "4");
            });

            t.Wait();
            Console.ReadLine();
        }
    }
}
