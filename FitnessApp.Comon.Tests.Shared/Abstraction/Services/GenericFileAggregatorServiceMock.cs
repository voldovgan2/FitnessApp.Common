using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FitnessApp.Common.Abstractions.Services;
using FitnessApp.Common.Files;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericFileAggregator;

[ExcludeFromCodeCoverageAttribute]
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
