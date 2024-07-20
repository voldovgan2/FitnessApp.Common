using AutoMapper;
using FitnessApp.Common.Abstractions.Db.Repository.Generic;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.Generic;

public class GenericServiceMock(
        IGenericRepository<
            TestGenericEntity,
            TestGenericModel,
            CreateTestGenericModel,
            UpdateTestGenericModel> repository,
        IMapper mapper
        ) : GenericService<
            TestGenericEntity,
            TestGenericModel,
            CreateTestGenericModel,
            UpdateTestGenericModel>(repository, mapper);
