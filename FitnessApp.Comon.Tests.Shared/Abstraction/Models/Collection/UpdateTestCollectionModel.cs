using System;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection
{
    public class UpdateTestCollectionModel : IUpdateCollectionModel, IEquatable<UpdateTestCollectionModel>
    {
        public string UserId { get; set; }
        public string CollectionName { get; set; }
        public UpdateCollectionAction Action { get; set; }
        public ICollectionItemModel Model { get; set; }

        public static bool operator ==(UpdateTestCollectionModel ucm1, UpdateTestCollectionModel ucm2)
        {
            if (ucm1 is null)
                return ucm2 is null;

            return ucm1.Equals(ucm2);
        }

        public static bool operator !=(UpdateTestCollectionModel ucm1, UpdateTestCollectionModel ucm2)
        {
            return !(ucm1 == ucm2);
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode() ^ CollectionName.GetHashCode() ^ Action.GetHashCode() ^ Model.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return IsEqual(obj);
        }

        public bool Equals([AllowNull] UpdateTestCollectionModel obj)
        {
            return IsEqual(obj);
        }

        private bool IsEqual(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var ucm2 = (UpdateTestCollectionModel)obj;
            return UserId == ucm2.UserId && CollectionName == ucm2.CollectionName && Action == ucm2.Action && Model.Id == ucm2.Model.Id;
        }
    }
}
