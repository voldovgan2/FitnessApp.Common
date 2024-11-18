using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Db;
using FitnessApp.Common.Abstractions.Services;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.Generic;

[ExcludeFromCodeCoverageAttribute]
public class GenericServiceMock(
    IGenericRepository<
        TestGenericModel,
        CreateTestGenericModel,
        UpdateTestGenericModel> repository) : GenericService<
        TestGenericModel,
        CreateTestGenericModel,
        UpdateTestGenericModel>(repository);
