using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator;

namespace FitnessApp.Comon.Tests.Shared;

[ExcludeFromCodeCoverageAttribute]
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region GenericModel 2 GenericEntity
        CreateMap<CreateTestGenericModel, TestGenericEntity>();
        CreateMap<UpdateTestGenericModel, TestGenericEntity>();
        CreateMap<TestGenericModel, TestGenericEntity>();
        #endregion

        #region GenericEntity 2 GenericModel
        CreateMap<TestGenericEntity, TestGenericModel>();
        #endregion

        #region GenericFileAggregatorModel 2 GenericModel
        CreateMap<CreateTestGenericFileAggregatorModel, CreateTestGenericModel>();
        CreateMap<UpdateTestGenericFileAggregatorModel, UpdateTestGenericModel>();
        #endregion
    }
}
