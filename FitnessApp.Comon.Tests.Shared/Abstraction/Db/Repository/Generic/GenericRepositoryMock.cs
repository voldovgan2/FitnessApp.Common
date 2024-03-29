﻿using AutoMapper;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Repository.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Generic
{
    public class GenericRepositoryMock : GenericRepository<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>
    {
        public GenericRepositoryMock(IDbContext<TestGenericEntity> dbContext, IMapper mapper)
            : base(dbContext, mapper) { }
    }
}
