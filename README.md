# ThunderDesign.Net-PCL.Threading
[![CI](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/actions/workflows/CI.yml/badge.svg)](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/actions/workflows/CI.yml)
[![CD](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/actions/workflows/CD.yml/badge.svg)](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/actions/workflows/CD.yml)
[![Nuget](https://img.shields.io/nuget/v/ThunderDesign.Net-PCL.Threading)](https://www.nuget.org/packages/ThunderDesign.Net-PCL.Threading)
[![License](https://img.shields.io/github/license/ThunderDesign/ThunderDesign.Net-PCL.Threading)](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/blob/main/LICENSE)
[![Net](https://img.shields.io/badge/.net%20standard-v1.0%20--%20v2.1-blue)](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/blob/main/README.md)

A combination of generic Thread-Safe objects for .Net development.

----

A simple C# repository containing a few basic useful Thread-Safe Objects.
### Highlights include:

- Collections
  - ObservableDictionaryThreadSafe
  - ObservableCollectionThreadSafe
  - CollectionThreadSafe
  - DictionaryThreadSafe
  - SortedListThreadSafe
  - ListThreadSafe
  - QueueThreadSafe
- DataCollections
  - ObservableDataDictionary
  - ObservableDataCollection
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
- Objects
  - BindableObject
  - ThreadObject

----

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