using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy.Infrastructure
{
    class RoundRobinList
    {
        private readonly IList<int> _list;
        private int _position = -1;

        public RoundRobinList(IEnumerable<int> list)
        {
            _list = new List<int>(list);
        }

        public int Next()
        {
            lock (_list)
            {
                _position++;
                int index = _position % _list.Count;
                return _list[index];
            }
        }
    }
}
