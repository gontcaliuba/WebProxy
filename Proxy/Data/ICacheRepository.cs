using System;
using Common.Models;

namespace Proxy.Data
{
    internal interface ICacheRepository
    {
        void Add<T>(string key, T value, TimeSpan expiresIn);
        T Get<T>(string key);
    }
}