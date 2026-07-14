using System.Collections.ObjectModel;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class CollectionThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_CollectionThreadSafe_CoversCollectionOfT()
        {
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(CollectionThreadSafe<int>),
                typeof(Collection<int>));
        }
    }
}
