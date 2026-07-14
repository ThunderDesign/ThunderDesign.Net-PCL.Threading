using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class DictionaryThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_DictionaryThreadSafe_CoversDictionaryOfTKeyTValue()
        {
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(DictionaryThreadSafe<string, int>),
                typeof(Dictionary<string, int>));
        }
    }
}
