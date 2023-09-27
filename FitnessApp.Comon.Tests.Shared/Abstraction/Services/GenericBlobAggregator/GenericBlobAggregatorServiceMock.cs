using AutoMapper;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Common.Abstractions.Services.GenericBlobAggregator;
using FitnessApp.Common.Blob;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericBlobAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericBlobAggregator
{
    public class GenericBlobAggregatorServiceMock : GenericBlobAggregatorService<TestGenericEntity, TestGenericBlobAggregatorModel, TestGenericModel, CreateTestGenericBlobAggregatorModel, CreateTestGenericModel, UpdateTestGenericBlobAggregatorModel, UpdateTestGenericModel>
    {
        public GenericBlobAggregatorServiceMock(
            IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel> genericService,
            IBlobService blobService,
            IMapper mapper,
            GenericBlobAggregatorSettings genericBlobAggregatorSettings
            ) : base(genericService, blobService, mapper, genericBlobAggregatorSettings) { }
    }
}
