using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class QueueThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_QueueThreadSafe_CoversQueueOfT()
        {
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(QueueThreadSafe<int>),
                typeof(Queue<int>));
        }
    }
}
