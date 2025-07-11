# Contributing to ThunderDesign.Net-PCL.Threading

Thank you for your interest in contributing to ThunderDesign.Net-PCL.Threading! We welcome contributions from the community and are grateful for your help in making this library better.

## 🚀 Quick Start

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Run tests (`dotnet test`)
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Setup](#development-setup)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Source Generator Development](#source-generator-development)
- [Pull Request Process](#pull-request-process)
- [Issue Reporting](#issue-reporting)

## 🤝 Code of Conduct

This project and everyone participating in it is governed by our [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior to [project maintainer email].

## 🛠 How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the [existing issues](https://github.com/ThunderDesign/ThunderDesign.Net-PCL.Threading/issues) to avoid duplicates.

When filing a bug report, please include:
- **Clear title and description**
- **Steps to reproduce** the behavior
- **Expected vs actual behavior**
- **Environment details** (.NET version, OS, etc.)
- **Sample code** if applicable

### Suggesting Enhancements

Enhancement suggestions are welcome! Please:
- Use a clear and descriptive title
- Provide a detailed description of the proposed enhancement
- Explain why this enhancement would be useful
- Include code examples if applicable

### Contributing Code

We welcome the following types of contributions:
- Bug fixes
- New thread-safe collection types
- Performance improvements
- Source generator enhancements
- Documentation improvements
- Test coverage improvements

## 🔧 Development Setup

### Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) (latest)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Setting Up Your Development Environment

1. **Fork and clone the repository:**

```console
git clone https://github.com/YOUR_USERNAME/ThunderDesign.Net-PCL.Threading.git cd ThunderDesign.Net-PCL.Threading
```

2. **Restore dependencies:**

```console
dotnet restore
```

3. **Build the solution:**

```console
dotnet build
```

4. **Run tests:**

```console
dotnet test
```

### Project Structure

├── ThunderDesign.Net-PCL.Threading/          # Main library 
├── ThunderDesign.Net-PCL.Threading.Shared/   # Shared components 
├── ThunderDesign.Net-PCL.SourceGenerators/   # Source generators 
├── ThunderDesign.Net-PCL.Threading.Tests/    # Unit tests 
└── samples/                                  # Example projects

## 📝 Coding Standards

### General Guidelines

- Follow [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- Use meaningful names for variables, methods, and classes
- Write self-documenting code with clear comments where necessary
- Keep methods focused and concise

### Naming Conventions

- **Classes**: PascalCase (`ThreadSafeCollection`)
- **Methods**: PascalCase (`GetProperty`)
- **Properties**: PascalCase (`IsThreadSafe`)
- **Fields**: camelCase with underscore prefix (`_lockObject`)
- **Constants**: PascalCase (`DefaultTimeout`)
- **Interfaces**: PascalCase with 'I' prefix (`IThreadSafe`)

### Threading Considerations

Since this is a threading library, please ensure:
- All public members are thread-safe unless explicitly documented otherwise
- Use appropriate locking mechanisms (prefer `lock` statements)
- Consider performance implications of synchronization
- Document thread-safety guarantees in XML comments

### XML Documentation

All public APIs must include XML documentation:

```csharp
/// <summary>
/// Provides thread-safe access to a collection of items. 
/// </summary> 
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class ThreadSafeCollection<T> : ICollection<T> 
{
    /// <summary> 
    /// Gets the number of elements in the collection in a thread-safe manner. 
    /// </summary> 
    /// <returns>The number of elements in the collection.</returns> 
    public int Count 
    { 
        get; 
    } 
}
```

## 🧪 Testing Guidelines

### Writing Tests

- Use [xUnit](https://xunit.net/) for unit tests
- Follow the Arrange-Act-Assert pattern
- Test both success and failure scenarios
- Include thread-safety tests where applicable
- Aim for high code coverage

### Test Organization

```csharp
public class ThreadSafeCollectionTests 
{ 
    [Fact] 
    public void Add_SingleItem_IncreasesCount() 
    { 
        // Arrange var collection = new ThreadSafeCollection<int>();
        // Act
        collection.Add(1);
    
        // Assert
        Assert.Equal(1, collection.Count);
    }

    [Fact]
    public void ConcurrentAccess_MultipleThreads_MaintainsConsistency()
    {
        // Test concurrent operations
    }
}
```

### Running Tests

Run all tests
```console
dotnet test
```
Run tests with coverage
```console
dotnet test --collect:"XPlat Code Coverage"
```
Run specific test project
```console
dotnet test ThunderDesign.Net-PCL.Threading.Tests/
```

## ⚙️ Source Generator Development

When working on source generators:

### Guidelines

- Follow the [Source Generator Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
- Ensure generators are deterministic
- Handle edge cases gracefully
- Provide meaningful error messages
- Test with various input scenarios

### Testing Source Generators

```csharp
[Fact] 
public void PropertyGenerator_WithValidField_GeneratesExpectedCode() 
{ 
    var source = @" public partial class TestClass 
    { 
        [Property] private string _name; 
    }";
    var expected = @"

    public partial class TestClass
    {
        public string Name
        {
            get { return this.GetProperty(ref _name, _Locker); }
            set { this.SetProperty(ref _name, value, _Locker); }
        }
    }";

    // Test the generator
    VerifyGenerator(source, expected);
}
```

## 📬 Pull Request Process

### Before Submitting

1. **Update documentation** if you've changed APIs
2. **Add or update tests** for your changes
3. **Ensure all tests pass** locally
4. **Follow the coding standards**
5. **Update the changelog** if applicable

### Pull Request Template

When creating a pull request, please:

1. **Use a clear title** that describes the change
2. **Reference any related issues** (`Fixes #123`)
3. **Describe your changes** in detail
4. **List any breaking changes**
5. **Include screenshots** for UI-related changes

### Review Process

1. At least one maintainer will review your PR
2. Address any feedback promptly
3. Keep your branch up to date with main
4. Once approved, a maintainer will merge your PR

## 🐛 Issue Reporting

### Bug Reports

Use the bug report template and include:
- Clear reproduction steps
- Expected vs actual behavior
- Environment information
- Minimal code sample

### Feature Requests

Use the feature request template and include:
- Clear description of the feature
- Use cases and benefits
- Possible implementation approach

## 📞 Getting Help

- **GitHub Issues**: For bugs and feature requests
- **Discussions**: For questions and general discussion
- **Discord/Slack**: [Add community chat links if available]

## 🏷️ Versioning

We use [Semantic Versioning](http://semver.org/) (SemVer):
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

## 📄 License

By contributing, you agree that your contributions will be licensed under the same license as the project.

---

Thank you for contributing to ThunderDesign.Net-PCL.Threading! 🎉