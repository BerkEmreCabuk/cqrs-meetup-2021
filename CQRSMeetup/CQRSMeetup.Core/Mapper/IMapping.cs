using AutoMapper;

namespace CQRSMeetup.Core.Mapper
{
    public interface IMapping
    {
        void CreateMappings(IProfileExpression profileExpression);
    }
}
