using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CQRSMeetup.Core.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile(string assemblyString) : this("AutoMapperProfileMappings", assemblyString)
        {
        }
        public AutoMapperProfile(string profileName, string assemblyString) : base(profileName)
        {
            IEnumerable<Type> types = Assembly.Load(assemblyString).GetExportedTypes();

            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where typeof(IMapping).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface
                        select (IMapping)Activator.CreateInstance(t)).ToArray();

            foreach (var map in maps)
            {
                map.CreateMappings(this);
            }
        }
    }
}
