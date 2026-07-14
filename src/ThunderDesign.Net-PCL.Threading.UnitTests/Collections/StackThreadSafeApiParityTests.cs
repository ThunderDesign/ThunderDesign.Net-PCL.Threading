using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class StackThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_StackThreadSafe_CoversStackOfT()
        {
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(StackThreadSafe<int>),
                typeof(Stack<int>));
        }
    }
}
