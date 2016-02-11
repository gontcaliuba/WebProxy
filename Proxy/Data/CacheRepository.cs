using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using ServiceStack.Redis;

namespace Proxy.Data
{
    class CacheRepository : ICacheRepository
    {
        private readonly IRedisClient _redisClient;

        public CacheRepository()
        {
            _redisClient = new RedisClient("localhost");
        }

        public void Add<T>(string key, T value, TimeSpan expiresIn)
        {
            var typedClient = _redisClient.As<T>();
            typedClient.SetEntry(key, value, expiresIn);
        }

        public T Get<T>(string key)
        {
            var typedClient = _redisClient.As<T>();
            return typedClient.GetValue(key);
        }
    }
}
