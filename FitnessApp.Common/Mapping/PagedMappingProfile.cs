using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FitnessApp.Common.Paged.Contracts.Output;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Mapping;

[ExcludeFromCodeCoverage]
public class PagedMappingProfile<TContract, TModel> : Profile
{
    public PagedMappingProfile()
    {
        CreateMap<PagedDataModel<TModel>, PagedDataContract<TContract>>();
    }
}
