﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.GenericFileAggregator;

namespace FitnessApp.Common.Abstractions.Services.GenericFileAggregator;

public interface IGenericFileAggregatorService<
    TGenericFileAggregatorModel,
    TGenericModel,
    TCreateGenericFileAggregatorModel,
    TUpdateGenericFileAggregatorModel>
    where TGenericFileAggregatorModel : IGenericFileAggregatorModel<TGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
    where TUpdateGenericFileAggregatorModel : IUpdateGenericFileAggregatorModel
{
    Task<TGenericFileAggregatorModel> GetItemByUserId(string userId);
    Task<IEnumerable<TGenericFileAggregatorModel>> GetItemsByIds(string[] ids);
    Task<TGenericFileAggregatorModel> CreateItem(TCreateGenericFileAggregatorModel model);
    Task<TGenericFileAggregatorModel> UpdateItem(TUpdateGenericFileAggregatorModel model);
    Task<string> DeleteItem(string userId);
}
