# Claude Code Instructions - Corral App

## Architecture & Code Organization

### 1. One Class Per File
- **Rule**: Exactly 1 class/interface/record = 1 file
- **NO nested types** allowed (no classes inside classes)
- **NO anonymous types** or local types
- File name must match the class name exactly (case-sensitive on Linux)

### 2. Command/Query Structure

Each Command or Query MUST have its own folder with 3 separate files:

**Commands Example:**
```
src/Core/Corral.Application/Commands/CreateFence/
├── CreateFenceCommand.cs          # The command record (ICommand<FenceDto>)
├── CreateFenceCommandValidator.cs # FluentValidation validator
└── CreateFenceCommandHandler.cs   # MediatR handler
```

**Queries Example:**
```
src/Core/Corral.Application/Queries/GetFenceById/
├── GetFenceByIdQuery.cs           # The query record (IQuery<FenceDto>)
└── GetFenceByIdQueryHandler.cs    # MediatR handler (no validator)
```

### 3. Layer Separation

**Domain Layer** (`src/Core/Corral.Domain/`)
- Pure business logic, no external dependencies
- Aggregate Roots, Value Objects, Domain Events, Exceptions only
- **Contracts folder** for interfaces that other layers depend on:
  - `Contracts/CQRS/` - ICommand<T>, IQuery<T>
  - `Contracts/Repositories/` - IFenceRepository, etc.

**Application Layer** (`src/Core/Corral.Application/`)
- Commands, Queries, Handlers, Validators
- DTOs for data transfer
- Mappers for Domain → DTO transformation
- **NO direct exposure of Domain entities** to upper layers

**Infrastructure Layer** (`src/Infrastructure/Corral.Infrastructure/`)
- **Entities folder**: EF Core entity models (FenceEntity, etc.)
- **Mappers folder**: EF Core Entity → Domain Aggregate transformation
- **Repositories folder**: Data access implementations
- **Persistence folder**: DbContext and EF Core configurations
- **UnitOfWork folder**: Transaction management

**Presentation Layer** (`src/Presentation/Corral.Desktop/`)
- **ViewModels folder**: MVVM-based view logic
- **Mappers folder**: DTO → ViewModel transformation
- **Views/Pages**: WinUI 3 XAML files
- **Services folder**: UI services (navigation, etc.)
- **NO direct Domain references** - only DTO and ViewModel objects

### 4. Naming Conventions

| Item | Naming | Example |
|------|--------|---------|
| Commands | PascalCase + "Command" | `CreateFenceCommand` |
| Command Handlers | PascalCase + "CommandHandler" | `CreateFenceCommandHandler` |
| Command Validators | PascalCase + "CommandValidator" | `CreateFenceCommandValidator` |
| Queries | PascalCase + "Query" | `GetFenceByIdQuery` |
| Query Handlers | PascalCase + "QueryHandler" | `GetFenceByIdQueryHandler` |
| DTOs | PascalCase + "Dto" | `FenceDto` |
| Entities (EF) | PascalCase + "Entity" | `FenceEntity` |
| Mappers | Source + "To" + Destination + "Mapper" | `FenceEntityToDomainMapper` |
| Repositories | PascalCase + "Repository" | `FenceRepository` |

### 5. XML Documentation Requirements

All public types and members MUST have MSDN-style XML documentation:

```csharp
/// <summary>
/// Brief description (one line, max 100 chars).
/// </summary>
/// <remarks>
/// Detailed explanation if needed. Use for:
/// - Business logic context
/// - Edge cases
/// - Usage patterns
/// </remarks>
/// <param name="paramName">Description of parameter</param>
/// <returns>Description of return value</returns>
/// <exception cref="ArgumentNullException">When X is null</exception>
/// <example>
/// <code>
/// var example = MyMethod("value");
/// </code>
/// </example>
public void MyMethod(string paramName) { }
```

### 6. Data Flow Through Layers

```
User Input (UI)
    ↓
ViewModel receives input
    ↓
Sends Command/Query via MediatR
    ↓
CommandHandler/QueryHandler processes
    ↓
Repository loads/saves Domain entities
    ↓
Mapper: FenceEntity (EF) ↔ Fence (Domain)
    ↓
Response mapped: Fence (Domain) → FenceDto (Application)
    ↓
Result mapped: FenceDto → FenceViewModel (Presentation)
    ↓
ViewModel displays to UI
```

### 7. Import Organization

Group imports in this order:
1. System and System.* namespaces
2. Third-party libraries (MediatR, FluentValidation, etc.)
3. Project namespaces (Corral.*)
4. Empty line between groups

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using FluentValidation;

using Corral.Domain.Aggregates;
using Corral.Domain.ValueObjects;
using Corral.Application.DTOs;
using Corral.Infrastructure.Persistence;
```

### 8. Nullable Reference Types

Nullable est **désactivé** dans tous les projets.

- Pas de `?` sur les types référence
- Pas de `#nullable enable`
- Pas de `?.` — utiliser `if (x != null) x.Method()`
- Pas de null checks DI dans les constructeurs (le container garantit les instances)

### 9. Dependency Injection & Registration

- **Handlers**: Register via MediatR in Startup/Program.cs
- **Validators**: Register via FluentValidation in Startup/Program.cs
- **Repositories**: Register in Infrastructure service collection
- **DbContext**: Register with appropriate lifetime (Scoped)
- **Mappers**: Register as Singletons (stateless)

### 10. Testing Requirements

**Unit Tests** (`tests/Corral.Domain.Tests/` and `tests/Corral.Application.Tests/`)
- Test file name: `[ClassBeingTested]Tests.cs`
- Test method name: `MethodName_Scenario_ExpectedResult`
- Example: `Move_WithValidPosition_UpdatesPositionAndRaisesEvent`
- Use xUnit with Arrange-Act-Assert pattern
- Use Moq for mocking dependencies

### 11. Git Commit Messages

Format: `<type>: <subject> [<scope>]`

Examples:
- `feat: Add CreateFenceCommand handler with validation`
- `refactor(domain): Extract Position validation to ValueObject`
- `fix(infrastructure): Correct datetime nullable mapping`
- `test(application): Add handler tests for UpdateFence`

---

## Best Practices

### ✅ DO

- Create separate files for each handler, validator, command, query
- Use immutable records for Commands and Queries
- Return DTOs from Application layer, never Domain entities
- Use Value Objects in Domain layer
- Map at layer boundaries (Infrastructure ↔ Domain ↔ Application ↔ Presentation)
- Keep handlers thin - business logic goes in Domain/Aggregates
- Validate input at Application layer (Commands)

### ❌ DON'T

- Nest types (no classes inside classes)
- Expose Domain entities beyond Application layer
- Use anonymous types or local types
- Mix persistence logic with business logic
- Have ViewModels reference Domain entities
- Create circular dependencies between projects
- Use static classes for business logic
- Skip XML documentation

---

## Project References (Dependency Direction)

```
Domain (no external deps except System)
  ↑
Application (depends on Domain)
  ↑
Infrastructure (depends on Domain, Application)
  ↑
Presentation (depends on Application, Infrastructure)
```

**NO reverse dependencies allowed**.
Example: Never have Domain reference Application or Infrastructure.

---

## File Structure Template

### Command Structure
```
Application/Commands/[CommandName]/
├── [CommandName]Command.cs
├── [CommandName]CommandValidator.cs
└── [CommandName]CommandHandler.cs
```

### Query Structure
```
Application/Queries/[QueryName]/
├── [QueryName]Query.cs
└── [QueryName]QueryHandler.cs
```

---

## Questions?

If the instructions are unclear or conflict with project needs, prioritize:
1. DDD principles (Aggregates, Value Objects, Domain Events)
2. Clean Architecture (layer separation)
3. SOLID principles (especially Dependency Inversion)
4. Code clarity over cleverness
