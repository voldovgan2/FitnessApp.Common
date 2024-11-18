using System.Collections.Generic;
using AutoMapper;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db;

namespace FitnessApp.Common.UnitTests;

public abstract class TestBase
{
    protected readonly IMapper _mapper;

    protected TestBase()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        })
            .CreateMapper();
    }

    protected static TestGenericEntity[] GetGenericEntitiesMock()
    {
        return TestData.GetAll(TestData.CreateGenericEntity, new Dictionary<string, object>());
    }
}