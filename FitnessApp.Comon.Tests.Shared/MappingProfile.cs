using System.Collections.Generic;
using AutoMapper;
using FitnessApp.Common.Abstractions.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionBlobAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericBlobAggregator;

namespace FitnessApp.Comon.Tests.Shared
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region GenericModel 2 GenericEntity
            CreateMap<CreateTestGenericModel, TestGenericEntity>();
            CreateMap<UpdateTestGenericModel, TestGenericEntity>();
            CreateMap<TestGenericModel, TestGenericEntity>();
            #endregion

            #region GenericEntity 2 GenericModel
            CreateMap<TestGenericEntity, TestGenericModel>();
            #endregion

            #region CollectionModel 2 CollectionEntity
            CreateMap<CreateTestCollectionModel, TestCollectionEntity>();
            #endregion

            #region CollectionItemModel 2 CollectionItemEntity
            CreateMap<TestCollectionItemModel, TestCollectionItemEntity>();
            #endregion

            #region CollectionItemEntity 2 CollectionItemModel
            CreateMap<TestCollectionItemEntity, TestCollectionItemModel>();
            #endregion

            #region CollectionEntity 2 CollectionModel
            CreateMap<TestCollectionEntity, TestCollectionModel>()
                .ForMember(e => e.Collection, m => m.MapFrom(o => CollectionEntitiesToCollectionModels(o.Collection)));
            #endregion

            #region GenericBlobAggregatorModel 2 GenericModel
            CreateMap<CreateTestGenericBlobAggregatorModel, CreateTestGenericModel>();
            CreateMap<UpdateTestGenericBlobAggregatorModel, UpdateTestGenericModel>();
            #endregion

            #region CollectionBlobAggregatorModel 2 CollectionModel
            CreateMap<CreateTestCollectionBlobAggregatorModel, CreateTestCollectionModel>();
            CreateMap<UpdateTestCollectionBlobAggregatorModel, UpdateTestCollectionModel>()
                .ForMember(m1 => m1.Model, m2 => m2.MapFrom(m2 => m2.Model.Model));

            #endregion
        }

        private Dictionary<string, List<TestCollectionItemModel>> CollectionEntitiesToCollectionModels(Dictionary<string, List<ICollectionItemEntity>> collections)
        {
            var result = new Dictionary<string, List<TestCollectionItemModel>>();
            foreach (var kvp in collections)
            {
                var list = new List<TestCollectionItemModel>();
                foreach (var item in kvp.Value)
                {
                    list.Add(new TestCollectionItemModel
                    {
                        Id = item.Id
                    });
                }

                result.Add(kvp.Key, list);
            }

            return result;
        }
    }
}
