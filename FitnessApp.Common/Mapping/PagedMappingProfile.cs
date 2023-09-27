using AutoMapper;
using FitnessApp.Common.Paged.Contracts.Output;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Mapping
{
    public class PagedMappingProfile<TContract, TModel> : Profile
    {
        public PagedMappingProfile()
        {
            CreateMap<PagedDataModel<TModel>, PagedDataContract<TContract>>();
        }
    }
}
