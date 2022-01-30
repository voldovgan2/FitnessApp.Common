using AutoMapper;
using FitnessApp.Common.Paged.Contracts.Output;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Mapping
{
    public class PagedMappingProfile<C, M> : Profile
    {
        public PagedMappingProfile()
        {
            CreateMap<PagedDataContract<C>, PagedDataModel<M>>();
        }
    }
}
