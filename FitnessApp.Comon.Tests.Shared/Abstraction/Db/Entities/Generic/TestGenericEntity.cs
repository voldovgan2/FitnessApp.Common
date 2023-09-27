using System;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic
{
    public class TestGenericEntity : IGenericEntity, IEquatable<TestGenericEntity>
    {
        [BsonId]
        public string UserId { get; set; }
        [SingleWordSearchable]
        public string TestProperty1 { get; set; }
        [SingleWordSearchable]
        public string TestProperty2 { get; set; }
        [MultiWordSearchable]
        public string TestProperty3 { get; set; }
        [MultiWordSearchable]
        public string TestProperty4 { get; set; }

        public static bool operator ==(TestGenericEntity ge1, TestGenericEntity ge2)
        {
            if (ge1 is null)
                return (object)ge2 == null;

            return ge1.Equals(ge2);
        }

        public static bool operator !=(TestGenericEntity ge1, TestGenericEntity ge2)
        {
            return !(ge1 == ge2);
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode()
                ^ TestProperty1.GetHashCode()
                ^ TestProperty2.GetHashCode()
                ^ TestProperty3.GetHashCode()
                ^ TestProperty4.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return IsEqual(obj);
        }

        public bool Equals([AllowNull] TestGenericEntity obj)
        {
            return IsEqual(obj);
        }

        private bool IsEqual(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var ge2 = (TestGenericEntity)obj;
            return UserId == ge2.UserId
                && TestProperty1 == ge2.TestProperty1
                && TestProperty2 == ge2.TestProperty2
                && TestProperty3 == ge2.TestProperty3
                && TestProperty4 == ge2.TestProperty4;
        }
    }
}
