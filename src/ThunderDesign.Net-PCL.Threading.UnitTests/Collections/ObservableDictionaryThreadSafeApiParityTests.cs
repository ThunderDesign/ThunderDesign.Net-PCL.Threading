using System.Collections;
using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class ObservableDictionaryThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_ObservableDictionaryThreadSafe_CoversDictionaryOfTKeyTValue()
        {
            string[] knownExclusions = new string[]
            {
                "ContainsKey(String)",
                "ContainsValue(Int32)",
                "GetEnumerator()",
                "OnDeserialization(Object)",
                "TryGetValue(String, Int32&)",
                "EnsureCapacity(Int32)",
                "TrimExcess()",
                "TrimExcess(Int32)",
                "ContainsKey(String)",
                "ContainsValue(Int32)",
                "GetEnumerator()",
                "GetObjectData(SerializationInfo, StreamingContext)",
                "TryGetValue(String, Int32&)",
                "EnsureCapacity(Int32)",
                "TrimExcess()",
                "TrimExcess(Int32)",
                "Boolean IsSynchronized { get; }",
                "IEqualityComparer`1 Comparer { get; }",
                "Int32 Count { get; }",
                "KeyCollection Keys { get; }",
                "ValueCollection Values { get; }",
#if NET9_0_OR_GREATER
                "GetAlternateLookup()",
                "TryGetAlternateLookup(AlternateLookup`1&)",
                "Int32 Capacity { get; }"
#endif
            };

            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(ObservableDictionaryThreadSafe<string, int>),
                typeof(DictionaryThreadSafe<string, int>),
                knownExclusions);
        }
    }
}
