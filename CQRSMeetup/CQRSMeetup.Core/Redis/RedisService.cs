using CQRSMeetup.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CQRSMeetup.Core.Redis
{
    public class RedisService : IRedisService
    {
        public readonly IOptions<RedisConfigModel> _redisConfig;
        private readonly RedisEndpoint conf = null;

        public RedisService(IOptions<RedisConfigModel> redisConfig)
        {
            _redisConfig = redisConfig;
            if (_redisConfig != null)
                conf = new RedisEndpoint { Host = _redisConfig.Value.RedisEndPoint, Port = _redisConfig.Value.RedisPort, Password = "", RetryTimeout = 1000 };
        }
        public T Get<T>(string key)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    return client.Get<T>(key);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<T> GetAll<T>(string key)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    if (!key.EndsWith("*"))
                    {
                        key = $"{key}*";
                    }
                    var keys = client.SearchKeys(key);
                    var result = new List<T>();
                    if (keys.Any())
                    {
                        result = client.GetAll<T>(keys).Values.ToList();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Remove(string key)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    if (client.ContainsKey(key))
                    {
                        client.Remove(key);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Set(string key, object data)
        {
            try
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    if (client.ContainsKey(key))
                    {
                        client.Remove(key);
                    }
                    var dataSerialize = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                    client.Set(key, Encoding.UTF8.GetBytes(dataSerialize));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
