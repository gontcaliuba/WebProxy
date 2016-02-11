using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TransportService
{
    interface ITransportService
    {
        void Start();
        void Stop();
    }
}
