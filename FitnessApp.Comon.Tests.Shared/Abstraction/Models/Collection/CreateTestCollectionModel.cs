using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models.Collection;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection
{
    public class CreateTestCollectionModel : ICreateCollectionModel, IEquatable<CreateTestCollectionModel>
    {
        public string UserId { get; set; }
        public Dictionary<string, IEnumerable<ICollectionItemModel>> Collection { get; set; }

        public static bool operator ==(CreateTestCollectionModel ccm1, CreateTestCollectionModel ccm2)
        {
            if (ccm1 is null)
                return (object)ccm2 == null;

            return ccm1.Equals(ccm2);
        }

        public static bool operator !=(CreateTestCollectionModel ccm1, CreateTestCollectionModel ccm2)
        {
            return !(ccm1 == ccm2);
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return IsEqual(obj);
        }

        public bool Equals([AllowNull] CreateTestCollectionModel obj)
        {
            return IsEqual(obj);
        }

        private bool IsEqual(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var ccm2 = (CreateTestCollectionModel)obj;
            return UserId == ccm2.UserId;
        }
    }
}
