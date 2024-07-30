using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models.Collection;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;

[ExcludeFromCodeCoverageAttribute]
public class TestCollectionModel : ICollectionModel
{
    public string UserId { get; set; }
    public Dictionary<string, List<ICollectionItemModel>> Collection { get; set; }
}