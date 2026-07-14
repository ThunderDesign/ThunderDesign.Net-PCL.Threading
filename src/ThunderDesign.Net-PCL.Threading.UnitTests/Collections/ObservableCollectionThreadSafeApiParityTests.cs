using System.Collections.ObjectModel;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net_PCL.Threading.UnitTests.Helpers;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    public class ObservableCollectionThreadSafeApiParityTests
    {
        [Fact]
        public void ApiParity_ObservableCollectionThreadSafe_CoversObservableCollectionOfT()
        {
            string[] knownExclusions = new string[]
            {
                "Contains(Int32)",
                "CopyTo(Int32[], Int32)",
                "GetEnumerator()",
                "IndexOf(Int32)",
                "GetItemByIndex(Int32)",
                "CopyTo(Int32[], Int32)",
                "Contains(Int32)",
                "GetEnumerator()",
                "IndexOf(Int32)",
                "Boolean IsSynchronized { get; }",
                "Int32 Item { get; }",
                "Int32 Count { get; }"
            };

            // ObservableCollectionThreadSafe<T> derives from CollectionThreadSafe<T> rather
            // than from ObservableCollection<T> directly, but is intended to be its
            // thread-safe functional analog, so parity is checked against ObservableCollection<T>.
            ApiParityHelper.AssertWrapperCoversBaseApi(
                typeof(ObservableCollectionThreadSafe<int>),
                typeof(CollectionThreadSafe<int>),
                knownExclusions);
        }
    }
}
