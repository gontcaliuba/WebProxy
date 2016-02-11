using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Common
{
    public static class UtilityJson
    {
        public static string SerializeJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static async Task<string> SerializeJsonAsync(object obj)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(obj));
        }

        public static T DeserializeJson<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static async Task<T> DeserializeJsonAsync<T>(string data)
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<T>(data));
        }
    }
}
