# Contributing to Mistruna.Core

First off, thank you for considering contributing to Mistruna.Core! It's people like you that make this SDK such a great tool.

## Code of Conduct

This project and everyone participating in it is governed by our code of conduct. By participating, you are expected to uphold this code.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues as you might find that the problem has already been reported. When you are creating a bug report, please include as many details as possible:

- **Use a clear and descriptive title** for the issue to identify the problem.
- **Describe the exact steps which reproduce the problem** in as many details as possible.
- **Provide specific examples to demonstrate the steps**.
- **Describe the behavior you observed after following the steps** and point out what exactly is the problem with that behavior.
- **Explain which behavior you expected to see instead and why.**
- **Include the .NET version** you're using.

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, please include:

- **Use a clear and descriptive title** for the issue to identify the suggestion.
- **Provide a step-by-step description of the suggested enhancement** in as many details as possible.
- **Provide specific examples to demonstrate the steps**.
- **Describe the current behavior** and **explain which behavior you expected to see instead** and why.
- **Explain why this enhancement would be useful** to most users.

### Pull Requests

Please follow these steps to have your contribution considered:

1. Fork the repository and create your branch from `main`.
2. If you've added code that should be tested, add tests.
3. If you've changed APIs, update the documentation.
4. Ensure the test suite passes (`dotnet test`).
5. Make sure your code follows the existing code style (run `dotnet format`).
6. Issue the pull request!

## Development Setup

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- Your favorite IDE (Visual Studio, VS Code, Rider, etc.)

### Building

```powershell
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Create packages
./Build.ps1
```

### Running Tests

```powershell
dotnet test --configuration Release --verbosity normal
```

### Code Style

This project uses:
- `.editorconfig` for code style settings
- C# 14 language features
- Nullable reference types enabled

Before submitting a PR, please run:
```powershell
dotnet format
```

## Project Structure

```
Mistruna.Core/
├── src/
│   ├── Mistruna.Core/           # Main library
│   └── Mistruna.Core.Contracts/ # Contracts/interfaces only
├── test/
│   └── Mistruna.Core.Tests/     # Unit tests
├── samples/                     # Usage examples
└── assets/                      # Logo and assets
```

## Commit Messages

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests liberally after the first line

## Versioning

This project uses [Semantic Versioning](https://semver.org/) with [MinVer](https://github.com/adamralph/minver) for automatic versioning based on git tags.

To create a new release:
1. Create and push a tag: `git tag v1.0.0 && git push origin v1.0.0`
2. The CI/CD pipeline will automatically build and publish to NuGet

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
