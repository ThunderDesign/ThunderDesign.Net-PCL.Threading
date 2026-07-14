using System.Collections.Generic;
using System.Runtime.Serialization;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class HashSetThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_HashSetThreadSafe_CoversHashSetOfT()
        {
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(HashSetThreadSafe<int>),
                typeof(HashSet<int>));
        }
    }
}
