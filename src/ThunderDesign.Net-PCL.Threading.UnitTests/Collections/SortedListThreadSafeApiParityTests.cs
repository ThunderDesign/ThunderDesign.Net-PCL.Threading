using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class SortedListThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_SortedListThreadSafe_CoversSortedListOfTKeyTValue()
        {
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(SortedListThreadSafe<string, int>),
                typeof(SortedList<string, int>));
        }
    }
}
