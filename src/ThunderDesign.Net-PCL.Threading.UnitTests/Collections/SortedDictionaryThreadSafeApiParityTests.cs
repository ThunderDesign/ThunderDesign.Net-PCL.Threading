using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class SortedDictionaryThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_SortedDictionaryThreadSafe_CoversSortedDictionaryOfTKeyTValue()
        {
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(SortedDictionaryThreadSafe<string, int>),
                typeof(SortedDictionary<string, int>));
        }
    }
}
