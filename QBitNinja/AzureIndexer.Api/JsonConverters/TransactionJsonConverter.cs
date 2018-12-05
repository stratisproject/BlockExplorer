using System.Reflection;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sequence = NBitcoin.Sequence;

namespace AzureIndexer.Api.JsonConverters
{
    public class TransactionJsonConverter : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(Transaction) &&
                property.PropertyName == "LockTime")
            {
                property.ShouldSerialize = instanceOfProblematic => false;
            }

            if (property.DeclaringType == typeof(Sequence))
            {
                property.ShouldSerialize = instanceOfProblematic => false;
            }

            return property;
        }
    }
}
