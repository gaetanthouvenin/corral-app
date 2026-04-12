# Corral App - Development Guide

## Quick Start

### Technology Stack
- **Language**: C# 12.0
- **Framework**: .NET 10.0
- **Architecture**: Clean Architecture + DDD + CQRS
- **Database**: Entity Framework Core 9.0 + SQLite
- **CQRS**: MediatR 12.5
- **Validation**: FluentValidation 11.x
- **UI**: WPF (Windows Presentation Foundation)
- **Testing**: xUnit 2.x + Moq 4.x

### Project Structure

```
corral-app/
├── .claude/
│   └── instructions.md          # Claude-specific guidelines
├── .gitignore-agents            # Guidelines for all AI assistants
├── DEVELOPMENT.md               # This file
├── src/
│   ├── Corral.Domain/           # Pure business logic (no dependencies)
│   ├── Corral.Application/      # CQRS + Mappers
│   ├── Corral.Infrastructure/   # EF Core, Repositories, Mappers
│   └── Corral.Desktop/          # WinUI 3 application
└── tests/
    ├── Corral.Domain.Tests/     # Domain layer tests
    └── Corral.Application.Tests/ # Application layer tests
```

## Building & Running

### Build
```bash
dotnet build
```

### Build Individual Projects
```bash
dotnet build "src/Corral.Domain/"
dotnet build "src/Corral.Application/"
dotnet build "src/Corral.Infrastructure/"
dotnet build "tests/Corral.Domain.Tests"
dotnet build "tests/Corral.Application.Tests"
```

### Run Tests
```bash
dotnet test
```

### Run Desktop App
```bash
dotnet run --project "src/Presentation/Corral.Desktop/"
```

## Architecture Rules

### 1. One Class Per File
- **Exactly one class/interface/record per file**
- No nested types whatsoever
- File name matches class name (case-sensitive)

Example structure:
```
Commands/CreateFence/
├── CreateFenceCommand.cs           ← Only CreateFenceCommand
├── CreateFenceCommandValidator.cs  ← Only CreateFenceCommandValidator
└── CreateFenceCommandHandler.cs    ← Only CreateFenceCommandHandler
```

### 2. Layer Dependencies

**ALLOWED:**
```
Domain (zero external dependencies)
   ↑
Application (depends on Domain only)
   ↑
Infrastructure (depends on Domain, Application)
   ↑
Presentation (depends on Application, Infrastructure)
```

**NOT ALLOWED:**
```
❌ Domain → Application
❌ Application → Infrastructure (for business logic)
❌ Presentation → Domain
❌ Any circular references
```

### 3. Data Transformation Pipeline

**When loading from database:**
```
Database
    ↓
FenceEntity (EF Core)
    ↓ FenceEntityToDomainMapper
Domain Aggregate (Fence)
    ↓ FenceDomainToDtoMapper
Application DTO (FenceDto)
    ↓ FenceDtoToViewModelMapper
Presentation ViewModel (FenceViewModel)
    ↓
UI Display
```

**Every transformation is explicit** - no AutoMapper, no convention-based mapping.

### 4. CQRS Pattern

**Commands** (change state):
```
Commands/[Action][Entity]/
├── [Action][Entity]Command.cs              (immutable record)
├── [Action][Entity]CommandValidator.cs     (FluentValidation)
└── [Action][Entity]CommandHandler.cs       (MediatR IRequestHandler)
```

**Queries** (read state):
```
Queries/[Get/List][Entity]/
├── [Get/List][Entity]Query.cs         (immutable record)
└── [Get/List][Entity]QueryHandler.cs  (MediatR IRequestHandler)
```

## Implementation Checklist

### For Each New Command/Query

- [ ] Create feature folder: `Commands/CreateFence/` or `Queries/GetFenceById/`
- [ ] Create `.cs` file with command/query record
- [ ] Create validator file (for commands only)
- [ ] Create handler file
- [ ] Add complete XML documentation
- [ ] Register with MediatR/DI in Startup
- [ ] Write unit tests
- [ ] Update this guide if new patterns emerge

### Code Quality Checklist

- [ ] No nested types
- [ ] All public members documented with XML
- [ ] Proper namespaces matching folder structure
- [ ] One class per file
- [ ] Validates input at Application layer
- [ ] Maps at layer boundaries
- [ ] No Domain references in Presentation
- [ ] No Infrastructure references in Application business logic
- [ ] Tests have clear, descriptive names

## Naming Conventions

| Item | Format | Example |
|------|--------|---------|
| Commands | `[Verb][Noun]Command` | `CreateFenceCommand` |
| Command Handlers | `[Verb][Noun]CommandHandler` | `CreateFenceCommandHandler` |
| Command Validators | `[Verb][Noun]CommandValidator` | `CreateFenceCommandValidator` |
| Queries | `Get[Noun][Filter]Query` | `GetFenceByIdQuery`, `GetActiveFencesQuery` |
| Query Handlers | `Get[Noun][Filter]QueryHandler` | `GetFenceByIdQueryHandler` |
| DTOs | `[Noun]Dto` | `FenceDto` |
| EF Entities | `[Noun]Entity` | `FenceEntity` |
| Mappers | `[Source]To[Dest]Mapper` | `FenceEntityToDomainMapper` |
| Value Objects | `[Concept]` | `Position`, `Color`, `Opacity` |
| Aggregates | `[Entity]` | `Fence` |
| Repositories | `[Noun]Repository` | `FenceRepository` |

## XML Documentation Template

All public APIs must be documented:

```csharp
/// <summary>
/// Brief one-line description (max 100 characters).
/// </summary>
/// <remarks>
/// Optional detailed explanation:
/// - Business context
/// - Edge cases
/// - Design decisions
/// - Performance considerations
/// </remarks>
/// <param name="paramName">What this parameter does and constraints</param>
/// <returns>What is returned and its meaning</returns>
/// <exception cref="ArgumentNullException">Thrown when X is null</exception>
/// <example>
/// <code>
/// var result = MyMethod("value");
/// </code>
/// </example>
public void MyMethod(string paramName) { }
```

## Testing Strategy

### Unit Tests (xUnit + Moq)

**Test file naming:** `[ClassBeingTested]Tests.cs`

**Test method naming:** `[MethodName]_[Scenario]_[ExpectedResult]`

Example:
```csharp
public class CreateFenceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesFenceAndReturnsDto()
    {
        // Arrange
        var command = new CreateFenceCommand(/* ... */);
        var handler = new CreateFenceCommandHandler(unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
    }
}
```

### Coverage Targets
- Domain Layer: >80% coverage
- Application Layer: >75% coverage
- Infrastructure Layer: >60% coverage (database tests are integration tests)
- Presentation Layer: Not required (WinUI testing is complex)

## Git Workflow

### Branch Naming
- Feature: `feature/descriptive-name`
- Fix: `fix/issue-description`
- Refactor: `refactor/what-changed`

### Commit Messages
```
<type>(<scope>): <subject>

<optional detailed explanation>

Closes #<issue-number>
```

Types: `feat`, `fix`, `refactor`, `test`, `docs`, `style`, `chore`
Scope: `domain`, `application`, `infrastructure`, `presentation`

Examples:
```
feat(application): Add CreateFenceCommand handler and validator
fix(infrastructure): Correct DateTime nullable mapping in FenceRepository
test(application): Add handler tests for UpdateFenceCommand
refactor(domain): Extract color validation to separate method
docs: Update architecture guidelines
```

## Common Tasks

### Add a New Command

1. Create folder: `src/Core/Corral.Application/Commands/CreateFence/`
2. Create files:
   - `CreateFenceCommand.cs` - ICommand<FenceDto> record
   - `CreateFenceCommandValidator.cs` - FluentValidation validator
   - `CreateFenceCommandHandler.cs` - IRequestHandler implementation
3. Write tests in `tests/Corral.Application.Tests/Commands/CreateFence/`
4. Register in DI (Startup/Program.cs)
5. Update Application layer's service collection

### Add a New Query

1. Create folder: `src/Core/Corral.Application/Queries/GetFenceById/`
2. Create files:
   - `GetFenceByIdQuery.cs` - IQuery<FenceDto> record
   - `GetFenceByIdQueryHandler.cs` - IRequestHandler implementation
3. Write tests in `tests/Corral.Application.Tests/Queries/GetFenceById/`
4. Register in DI
5. Update Application layer's service collection

### Add a New Domain Event

1. Create file in `src/Core/Corral.Domain/DomainEvents/`
2. Implement `IDomainEvent` interface
3. Use in Aggregate Root methods
4. Document the event purpose and when it's raised

### Add Database Migration

```bash
cd src/Infrastructure/Corral.Infrastructure
dotnet ef migrations add [MigrationName]
dotnet ef database update
```

## Troubleshooting

### Build Fails: "The type or namespace name 'X' could not be found"
- Check using statements
- Verify file is in correct namespace matching folder structure
- Rebuild NuGet packages: `dotnet restore`

### Tests Fail: "Could not load type"
- Clear bin/obj folders: `dotnet clean`
- Rebuild: `dotnet build`
- Run tests again: `dotnet test`

### WPF on .NET 10
- WPF is fully supported and natively integrated with .NET 10
- No external dependencies required (UseWPF=true in .csproj)
- All projects compile without issues

## Resources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation](https://fluentvalidation.net/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## Questions or Clarifications?

If any architecture decision is unclear:
1. Check `.claude/instructions.md` for detailed guidelines
2. Check `.gitignore-agents` for AI assistant specific rules
3. Look at existing implementations as examples
4. Follow DDD + Clean Architecture principles as tiebreaker

---

Last Updated: April 12, 2026
Framework: .NET 10.0
