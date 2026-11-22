# Simplifier Test Management

[![NET Unit Tests](https://github.com/loknath2002/simplifier.testmanagement/actions/workflows/unit-tests.yml/badge.svg)](https://github.com/loknath2002/simplifier.testmanagement/actions/workflows/unit-tests.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Simplifier Test Management (SSTM)** is a desktop application for creating, organizing, and running manual test cases. It stores test projects as plain Markdown files, so test cases can be reviewed, diffed, searched, and managed with Git or edited by humans and AI tools.

## Features

- Create and open test projects
- Create, edit, save, and delete test cases
- Define test metadata, descriptions, preconditions, test data, and steps
- Run test cases and record Pass, Fail, Started, or Pending status for each step
- Add notes to failed steps
- Review run history for completed test runs
- Keep recent projects available for quick access
- Store test cases locally as portable Markdown documents
- Run on Avalonia-supported desktop platforms

## Technology

- [.NET 11](https://dotnet.microsoft.com/)
- [Avalonia UI](https://avaloniaui.net/)
- [SukiUI](https://github.com/kikipoulet/SukiUI)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- NUnit for unit tests
- Markdown as the test-case storage format

## Getting Started

### Prerequisites

- .NET 11 SDK
- A desktop environment supported by Avalonia
- Git

Verify the SDK is available:

```bash
dotnet --version
```

### Clone and Run

```bash
git clone https://github.com/loknath2002/simplifier.testmanagement.git
cd simplifier.testmanagement
dotnet restore Simplifier.TestManagement.sln
dotnet run --project Simplifier.TestManagement/Simplifier.TestManagement.csproj
```

To build a Release configuration:

```bash
dotnet build Simplifier.TestManagement.sln --configuration Release
```

## Test Data

Test projects are folders containing Markdown test-case files. Each test case includes:

- Test ID, name, author, date, tags, and status
- Description
- Preconditions
- Test data
- Steps and expected results
- Run history and step results

The supported on-disk format is identified in the file as `SSTM1`.

Example structure:

```text
MyProject/
├── TC-1_Login.md
├── TC-2_CreateAccount.md
└── TC-3_PasswordReset.md
```

Using Git for a test-project folder makes file history, reviews, branches, and AI-assisted editing available alongside the application.

## Running Tests

Run the complete NUnit test suite:

```bash
dotnet test Simplifier.TestManagement.Tests/Simplifier.TestManagement.Tests.csproj
```

Run tests in Release mode:

```bash
dotnet test Simplifier.TestManagement.Tests/Simplifier.TestManagement.Tests.csproj --configuration Release
```

Tests run automatically through GitHub Actions for pushes and pull requests targeting `master`.

## Contributing

Contributions are welcome. Please keep changes focused, include tests for behavioral changes, and follow the existing C# and MVVM conventions.

### Development Workflow

1. Fork the repository and clone your fork.
2. Create a descriptive branch:

   ```bash
   git switch -c feature/my-change
   ```

3. Make the change and add or update NUnit tests.
4. Run the test suite and build:

   ```bash
   dotnet build Simplifier.TestManagement.sln
   dotnet test Simplifier.TestManagement.Tests/Simplifier.TestManagement.Tests.csproj
   ```

5. Commit the changes with a clear message and open a pull request against `master`.
6. Explain the problem, implementation, and test coverage in the pull request description.

### Pull Request Guidelines

- Keep pull requests small and reviewable.
- Do not commit build output, IDE state, personal settings, or generated files.
- Preserve the Markdown format unless a backward-compatible format change is explicitly proposed.
- Add or update tests for parser, renderer, model, or view-model changes.
- Include screenshots or a short recording for UI changes.
- Ensure CI passes before requesting a review.

## Project Structure

```text
Simplifier.TestManagement/
├── Assets/                 # Application assets
├── Models/                 # Test project, test case, and run models
├── Services/               # Storage, Markdown, platform, and navigation services
├── Validators/             # Model validation rules
├── ViewModels/             # Application state and commands
└── Views/                  # Avalonia desktop views

Simplifier.TestManagement.Tests/
├── Models/                 # Model tests
├── Services/               # Storage and Markdown renderer tests
├── ViewModels/             # View-model tests
└── TestFiles/              # Markdown test fixtures
```

## License

This project is licensed under the [MIT License](LICENSE).
