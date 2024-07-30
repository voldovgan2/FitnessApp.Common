using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Db.Repository.Generic;
using FitnessApp.Common.Abstractions.Services.Generic;
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
