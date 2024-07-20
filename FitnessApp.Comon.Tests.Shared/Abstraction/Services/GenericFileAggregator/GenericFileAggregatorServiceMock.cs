using AutoMapper;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Common.Abstractions.Services.GenericFileAggregator;
using FitnessApp.Common.Files;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericFileAggregator;

public class GenericFileAggregatorServiceMock(
        IGenericService<
            TestGenericEntity,
            TestGenericModel,
            CreateTestGenericModel,
            UpdateTestGenericModel> genericService,
        IFilesService filesService,
        IMapper mapper,
        GenericFileAggregatorSettings genericFileAggregatorSettings) :
    GenericFileAggregatorService<
        TestGenericEntity,
        TestGenericFileAggregatorModel,
        TestGenericModel,
        CreateTestGenericFileAggregatorModel,
        CreateTestGenericModel,
        UpdateTestGenericFileAggregatorModel,
        UpdateTestGenericModel>(
        genericService,
        filesService,
        mapper,
        genericFileAggregatorSettings);
