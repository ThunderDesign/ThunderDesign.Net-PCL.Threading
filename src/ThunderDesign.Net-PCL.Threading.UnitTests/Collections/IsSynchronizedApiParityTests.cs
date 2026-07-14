using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.DataCollections;
using ThunderDesign.Net.Threading.DataObjects;
using Xunit;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Collections
{
    /// <summary>
    /// Reflection-based test that discovers every public, non-abstract, non-generic-open
    /// class in the Collections and DataCollections namespaces that implements the
    /// non-generic <see cref="ICollection"/> interface, and asserts that
    /// <see cref="ICollection.IsSynchronized"/> returns true for an instance of each.
    /// </summary>
    public class IsSynchronizedApiParityTests
    {
        public static IEnumerable<object[]> CollectionInstanceFactories()
        {
            yield return new object[] { "CollectionThreadSafe<int>", (Func<object>)(() => new CollectionThreadSafe<int>()) };
            yield return new object[] { "ListThreadSafe<int>", (Func<object>)(() => new ListThreadSafe<int>()) };
            yield return new object[] { "DictionaryThreadSafe<string,int>", (Func<object>)(() => new DictionaryThreadSafe<string, int>()) };
            yield return new object[] { "QueueThreadSafe<int>", (Func<object>)(() => new QueueThreadSafe<int>()) };
            yield return new object[] { "StackThreadSafe<int>", (Func<object>)(() => new StackThreadSafe<int>()) };
            yield return new object[] { "SortedListThreadSafe<string,int>", (Func<object>)(() => new SortedListThreadSafe<string, int>()) };
            yield return new object[] { "LinkedListThreadSafe<int>", (Func<object>)(() => new LinkedListThreadSafe<int>()) };
            yield return new object[] { "SortedDictionaryThreadSafe<string,int>", (Func<object>)(() => new SortedDictionaryThreadSafe<string, int>()) };
            yield return new object[] { "ObservableCollectionThreadSafe<int>", (Func<object>)(() => new ObservableCollectionThreadSafe<int>()) };
            yield return new object[] { "ObservableDictionaryThreadSafe<string,int>", (Func<object>)(() => new ObservableDictionaryThreadSafe<string, int>()) };
            yield return new object[] { "ObservableDataCollection<BindableDataObject<int>>", (Func<object>)(() => new ObservableDataCollection<BindableDataObject<int>>()) };
            yield return new object[] { "ObservableDataDictionary<int,BindableDataObject<int>>", (Func<object>)(() => new ObservableDataDictionary<int, BindableDataObject<int>>()) };
        }

        [Theory]
        [MemberData(nameof(CollectionInstanceFactories))]
        public void ICollection_IsSynchronized_ShouldReturnTrue(string typeName, Func<object> factory)
        {
            var instance = factory();

            Assert.True(instance is ICollection, $"'{typeName}' was expected to implement the non-generic ICollection interface.");

            var collection = (ICollection)instance;

            Assert.True(collection.IsSynchronized, $"'{typeName}' should report ICollection.IsSynchronized == true.");
        }

        [Theory]
        [MemberData(nameof(CollectionInstanceFactories))]
        public void Object_IsSynchronized_ShouldReturnTrue(string typeName, Func<object> factory)
        {
            var instance = factory();

            Assert.True(instance != null, $"'{typeName}' was expected to be a non-null instance.");

            // 1. Get the property info from the runtime type
            PropertyInfo? prop = instance.GetType()?.GetProperty("IsSynchronized");
            if (prop != null)
            {
                Assert.True(prop.PropertyType == typeof(bool), $"'{typeName}' was expected to have a property named 'IsSynchronized' of type bool.");
           
                Assert.True(prop.CanRead && !prop.CanWrite, $"'{typeName}' was expected to have a readable property named 'IsSynchronized'.");

                // 2. Read the value from the object instance
                object? actualValue = prop.GetValue(instance, null);

                Assert.True(actualValue is bool && (bool)actualValue, $"'{typeName}' should report IsSynchronized == true.");
            }
        }

        [Fact]
        public void AllICollectionImplementations_InCollectionsAndDataCollectionsNamespaces_AreCoveredByThisTest()
        {
            var coveredTypes = new HashSet<Type>(
                CollectionInstanceFactories().Select(args => ((Func<object>)args[1])().GetType().GetGenericTypeDefinition() is Type gtd ? gtd : ((Func<object>)args[1])().GetType()));

            var assembly = typeof(CollectionThreadSafe<>).Assembly;

            var candidateTypes = assembly.GetTypes()
                .Where(t => t.IsClass && t.IsPublic && !t.IsAbstract)
                .Where(t => (t.Namespace == typeof(CollectionThreadSafe<>).Namespace) || (t.Namespace == typeof(ObservableDataCollection<>).Namespace))
                .Where(t => typeof(ICollection).IsAssignableFrom(t) || ImplementsOpenGenericICollectionInterface(t))
                .ToList();

            var uncovered = candidateTypes
                .Where(t => !coveredTypes.Contains(t.IsGenericTypeDefinition ? t : (t.IsGenericType ? t.GetGenericTypeDefinition() : t)))
                .Select(t => t.Name)
                .ToList();

            Assert.True(uncovered.Count == 0,
                $"The following types implement ICollection but are not covered by {nameof(IsSynchronizedApiParityTests)}: {string.Join(", ", uncovered)}");
        }

        private static bool ImplementsOpenGenericICollectionInterface(Type t)
        {
            // Non-generic ICollection is inherited transitively via generic base types (e.g. List<T> : ICollection).
            // For open generic definitions in this assembly, check a closed instantiation instead.
            if (!t.IsGenericTypeDefinition) return false;

            foreach (var candidateArg in new[] { typeof(int), typeof(BindableDataObject<int>) })
            {
                try
                {
                    var genericArgs = t.GetGenericArguments().Select(_ => candidateArg).ToArray();
                    var closed = t.MakeGenericType(genericArgs);
                    if (typeof(ICollection).IsAssignableFrom(closed))
                        return true;
                }
                catch
                {
                    // constraint not satisfied by this candidate argument, try next
                }
            }

            return false;
        }
    }
}
