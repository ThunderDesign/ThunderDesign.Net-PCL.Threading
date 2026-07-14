using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class LinkedListThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_LinkedListThreadSafe_CoversLinkedListOfT()
        {
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(LinkedListThreadSafe<int>),
                typeof(LinkedList<int>));
        }
    }
}
