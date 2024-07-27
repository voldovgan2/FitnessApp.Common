using AutoMapper;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Common.Abstractions.Services.GenericFileAggregator;
using FitnessApp.Common.Files;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericFileAggregator;

public class GenericFileAggregatorServiceMock(
        IGenericService<
            TestGenericModel,
            CreateTestGenericModel,
            UpdateTestGenericModel> genericService,
        IFilesService filesService,
        IMapper mapper,
        GenericFileAggregatorSettings genericFileAggregatorSettings) :
    GenericFileAggregatorService<
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
