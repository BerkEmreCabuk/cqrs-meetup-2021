using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSMeetup.Core.Redis
{
    public interface IRedisService
    {
        T Get<T>(string key);
        IList<T> GetAll<T>(string key);
        void Set(string key, object data);
        void Remove(string key);
    }
}
