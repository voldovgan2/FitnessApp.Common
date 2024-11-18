using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FitnessApp.Common.Abstractions.Db;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Db;

[ExcludeFromCodeCoverageAttribute]
public class GenericRepositoryMock(IDbContext<TestGenericEntity> dbContext, IMapper mapper) :
    GenericRepository<
    TestGenericEntity,
    TestGenericModel,
    CreateTestGenericModel,
    UpdateTestGenericModel>(dbContext, mapper);
