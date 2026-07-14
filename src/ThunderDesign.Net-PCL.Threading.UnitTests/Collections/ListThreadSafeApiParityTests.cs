using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class ListThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_ListThreadSafe_CoversListOfT()
        {
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(ListThreadSafe<int>),
                typeof(List<int>));
        }
    }
}
