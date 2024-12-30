using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FitnessApp.Common.Paged;

namespace FitnessApp.Common.Mapping;

[ExcludeFromCodeCoverage]
public class PagedMappingProfile<TContract, TModel> : Profile
{
    public PagedMappingProfile()
    {
        CreateMap<PagedDataModel<TModel>, PagedDataContract<TContract>>();
    }
}
