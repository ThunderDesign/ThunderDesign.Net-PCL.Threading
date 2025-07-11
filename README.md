# ThunderDesign.Net-PCL.Threading
[![CI](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/actions/workflows/CI.yml/badge.svg)](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/actions/workflows/CI.yml)
[![CD](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/actions/workflows/CD.yml/badge.svg)](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/actions/workflows/CD.yml)
[![Nuget](https://img.shields.io/nuget/v/ThunderDesign.Net-PCL.Threading)](https://www.nuget.org/packages/ThunderDesign.Net-PCL.Threading)
[![License](https://img.shields.io/github/license/ThunderDesign/ThunderDesign.Net-PCL.Threading)](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/blob/main/LICENSE)
[![NetStandard](https://img.shields.io/badge/.net%20standard-v1.0%20--%20v2.1-blue)](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/blob/main/README.md)
[![Net](https://img.shields.io/badge/.net%20-v6.0%20--%20v8.0-blue)](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/blob/main/README.md)

---

A combination of generic Thread-Safe objects for .Net development.

---

## 🚀 Now with .NET 8 support and built-in Source Generators! 🚀

> - **.NET 8**: Take advantage of the latest .NET features and performance improvements.  
> - **Source Generators**: Eliminate boilerplate and let the library generate thread-safe, bindable properties for you automatically!  
>  
> _Get started faster, write less code, and enjoy modern .NET development!_

----

A simple C# repository containing a few basic useful Thread-Safe Objects.
### Highlights include:

- Collections
  - CollectionThreadSafe
  - DictionaryThreadSafe
  - HashSetThreadSafe
  - LinkedListThreadSafe
  - ListThreadSafe
  - ObservableCollectionThreadSafe
  - ObservableDictionaryThreadSafe
  - QueueThreadSafe
  - SortedDictionaryThreadSafe
  - SortedListThreadSafe
  - StackThreadSafe
- DataCollections
  - ObservableDataCollection
  - ObservableDataDictionary
- DataObjects
  - BindableDataObject
  - DataObject
- Extentions
  - IBindableObjectExtention
  - INotifyCollectionChangedExtension
  - INotifyPropertyChangedExtension
  - ObjectExtention
- HelperClasses
  - ThreadHelper
- Interfaces
  - IBindableCollection
  - IBindableDataObject
  - IBindableDataObject\<Key>
  - IBindableObject
  - ICollectionThreadSafe
  - IDataObject
  - IDataObject\<Key>
  - IDictionaryThreadSafe
  - IHashSetThreadSafe
  - ILinkedListThreadSafe
  - IListThreadSafe
  - IObservableDataCollection
  - IObservableDataCollection\<T>
  - ISortedDictionaryThreadSafe
  - IStackThreadSafe
- Objects
  - BindableObject
  - ThreadObject

----

## Source Generators

The `ThunderDesign.Net-PCL.Threading` project provides Roslyn-based source generators that automate the creation of common boilerplate code for thread-safe and bindable objects in this library. By including this package in your project, you can reduce repetitive code and ensure consistency across your data and collection classes.

### What does it do?

- **Automatic Property Generation:**  
  The source generator scans your code for fields marked with specific attributes (such as `[BindableProperty]` or `[Property]`) and automatically generates the corresponding properties, including thread-safe accessors and `INotifyPropertyChanged` support where appropriate.
- **Interface Implementation:**  
  If your class does not already implement interfaces like `IBindableObject`, the generator will add the necessary interface implementations and event wiring.
- **Thread Safety:**  
  Generated properties use locking patterns to ensure thread safety, matching the patterns used throughout the ThunderDesign.Net-PCL.Threading library.

### How to use

1. **Add the NuGet package:**  
   Reference the `ThunderDesign.Net-PCL.Threading` package in your project. If you are building from source, add a project reference to `ThunderDesign.Net-PCL.SourceGenerators.csproj`.

2. **Annotate your fields:**  
   Use `[BindableProperty]` or `[Property]` attributes on your fields to indicate which properties should be generated. The generator will handle the rest.

3. **Build your project:**  
   When you build, the source generator will automatically add the generated code to your compilation. You do not need to manually include or maintain the generated files.

4. **Enjoy less boilerplate:**  
   Your classes will have all the necessary properties, events, and thread-safety mechanisms without manual implementation.

> **Note:** Source generators require Visual Studio 2019 16.9+ or .NET SDK 5.0+ for full support.

### Example Usage

Suppose you want to create a thread-safe, bindable object with automatic property and notification support.  
With the source generator, you only need to annotate your fields:

```csharp
using ThunderDesign.Net.Threading.Attributes;

public partial class Person 
{
    [BindableProperty]
    private string _name;

    [Property]
    private int _age;
}
```

**What gets generated:**

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;

public partial class Person : IBindableObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected readonly object _Locker = new object();

    public string Name
    {
        get { return this.GetProperty(ref _name, _Locker); }
        set { this.SetProperty(ref _name, value, _Locker, true); }
    }

    public int Age
    {
        get { return this.GetProperty(ref _age, _Locker); }
        set { this.SetProperty(ref _age, value, _Locker); }
    }
}
```

You can now use your `Person` class like this:

```csharp
var person = new Person();
person.Name = "Alice";
person.Age = 30;
// PropertyChanged event will be raised for Name changes if you subscribe to it.
```

**No need to manually implement** property notification, thread safety, or boilerplate code—the generator does it for you!

> For more advanced scenarios, you can use attribute parameters to control property behavior (e.g., read-only, also notify other properties, or control accessor/property visibility).

---

### Advanced: Customizing Getter and Setter Accessors

You can control the visibility of the generated property's getter and setter using the `AccessorAccessibility` enum.  
The property itself will use the most accessible (widest) of the getter or setter's accessibilities.

#### Example

```csharp
using ThunderDesign.Net.Threading.Attributes;
using ThunderDesign.Net.Threading.Enums;

public partial class Person
{
    // Public getter, private setter (property will be public)
    [BindableProperty(getter: AccessorAccessibility.Public, setter: AccessorAccessibility.Private)]
    private string _name;

    // Internal getter, protected setter (property will be internal)
    [Property(getter: AccessorAccessibility.Internal, setter: AccessorAccessibility.Protected)]
    private int _age;
}
```

**What gets generated:**
```csharp
public partial class Person
{
    public string Name
    {
        get { return this.GetProperty(ref _name, _Locker); }
        private set { this.SetProperty(ref _name, value, _Locker, true); }
    }

    internal int Age
    {
        internal get { return this.GetProperty(ref _age, _Locker); }
        protected set { this.SetProperty(ref _age, value, _Locker); }
    }
}

```

> The property will be as accessible as its most accessible accessor (getter or setter).
> The default for `getter`, and `setter` is `public` if not specified.

**Available options for `AccessorAccessibility`:**
- `Public`
- `Private`
- `Protected`
- `Internal`
- `ProtectedInternal`
- `PrivateProtected`

---

### Advanced: Notify Other Properties

You can notify other properties when a specific property changes by using the `alsoNotify` parameter in the `[BindableProperty]` attribute.

#### Example
```csharp
using ThunderDesign.Net.Threading.Attributes;

public partial class Person
{
    [BindableProperty(alsoNotify: new[] { nameof(DisplayName) })]
    private string _firstName;

    [BindableProperty(alsoNotify: new[] { nameof(DisplayName) })]
    private string _lastName;

    public string DisplayName => $"{FirstName} {LastName}";
}
```

**What gets generated:**

```csharp
public partial class Person : IBindableObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected readonly object _Locker = new object();

    public string FirstName
    {
        get { return this.GetProperty(ref _firstName, _Locker); }
        set
        {
            if (this.SetProperty(ref _firstName, value, _Locker, true))
            {
                this.OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    public string LastName
    {
        get { return this.GetProperty(ref _lastName, _Locker); }
        set
        {
            if (this.SetProperty(ref _lastName, value, _Locker, true))
            {
                this.OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    public string DisplayName => $"{FirstName} {LastName}";
}
```

> This feature is particularly useful for computed properties like `DisplayName` that depend on other properties.

---

### Advanced: Static Properties

The `[Property]` attribute now supports static fields, allowing you to generate thread-safe static properties with automatic locking mechanisms.

#### Example
```csharp
using ThunderDesign.Net.Threading.Attributes;

public partial class AppSettings
{
    [Property]
    private static string _applicationName = "MyApp";

    [Property(getter: AccessorAccessibility.Internal)]
    private static readonly string _version = "1.0.0";
}
```

**What gets generated:**

```csharp
public partial class AppSettings
{ 
    static readonly object _StaticLocker = new object();

    public static string ApplicationName
    {
        get { return GetStaticProperty(ref _applicationName, _StaticLocker); }
        set { SetStaticProperty(ref _applicationName, value, _StaticLocker); }
    }

    internal static string Version
    {
        get { return GetStaticProperty(ref _version, _StaticLocker); }
    }

    // Helper methods for static property access
    public static T GetStaticProperty<T>(ref T backingStore, object? lockObj = null) { /* ... */ }
    public static bool SetStaticProperty<T>(ref T backingStore, T value, object? lockObj = null) { /* ... */ }
}
```

> **Note:** Static properties are only supported with the `[Property]` attribute. Use the `readonly` field modifier to create read-only static properties.

---

## Installation

Grab the latest [ThunderDesign.Net-PCL.Threading NuGet](https://www.nuget.org/packages/ThunderDesign.Net-PCL.Threading) package and install in your solution.

> Install-Package ThunderDesign.Net-PCL.Threading

Use the `-version` option to specify an [older version](https://www.nuget.org/packages/ThunderDesign.Net-PCL.Threading#versions-tab) to install.

## Examples

*(TIP: Clone repo, open the solution, build it and run sample app.)*
- Xamarin
  - [SimpleContacts Example](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/tree/main/samples/Xamarin/SimpleContacts)

## Please Contribute!

This is an open source project that welcomes contributions/suggestions/bug reports from those who use it. If you have any ideas on how to improve the library, please [post an issue here on GitHub](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/issues). Please check out the [How to Contribute](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/blob/main/.github/CONTRIBUTING.md).

----

## Breaking changes from v1.0.7 to v1.0.8!

Observable Objects now Wait when calling `PropertyChanged` Event.
This can be overwritten durring creation or by setting Property `WaitOnNotifyPropertyChanged`. Default value is `true`.

Observable Collections now Wait when calling `CollectionChanged` Event.
This can be overwritten durring creation or by setting Property `WaitOnNotifyCollectionChanged`. Default value is `true`.

*(TIP: If you experience Dead Locks change this value to `false`.)*


## Breaking changes from v1.0.9 to v1.0.10!

Observable Objects Property `WaitOnNotifyPropertyChanged` has been renamed to Property `WaitOnNotifying`.

Observable Collections Property `WaitOnNotifyCollectionChanged` has been removed and now uses Property `WaitOnNotifying`.
----


