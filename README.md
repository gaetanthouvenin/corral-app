# Corral

Corral est une application Windows qui permet de définir et gérer des **zones visuelles** (fences) sur l'écran. Chaque zone est un overlay coloré et nommé qui délimite une région de l'espace de travail.

## Vision produit

L'objectif est d'aider les utilisateurs à organiser leur espace de travail en créant des frontières visuelles permanentes : une zone "Développement" en bleu couvre les deux tiers gauche de l'écran, une zone "Communication" en orange occupe le coin supérieur droit, etc. Les zones persistent entre les sessions et peuvent être activées/désactivées à volonté.

## Stack technique

| Couche | Technologie |
|---|---|
| Language | C# 12 / .NET 10 |
| UI | WPF (Windows Presentation Foundation) |
| Pattern UI | MVVM — CommunityToolkit.Mvvm |
| CQRS | MediatR 12 |
| Validation | FluentValidation 12 |
| ORM | Entity Framework Core 10 + SQLite |
| Logging | Serilog (console + fichier rotatif) |
| Icons | MahApps.Metro.IconPacks |
| Tests | xUnit + Moq |

## Architecture

Le projet suit la **Clean Architecture** avec séparation stricte en couches :

```
corral-app/
├── src/
│   ├── Corral.Domain/          # Logique métier pure — aucune dépendance externe
│   │   ├── Aggregates/         # Fence (racine d'agrégat)
│   │   ├── ValueObjects/       # Color, Position, Dimensions, Opacity, FenceId
│   │   ├── Contracts/          # Interfaces (IFenceRepository, IUnitOfWork, ICommand, IQuery)
│   │   └── Exceptions/         # Exceptions métier du domaine
│   │
│   ├── Corral.Application/     # Orchestration CQRS — dépend de Domain
│   │   ├── Commands/           # CreateFence, UpdateFence, DeleteFence, MoveFence
│   │   ├── Queries/            # GetAllFences, GetActiveFences, GetFenceById
│   │   ├── Behaviors/          # LoggingBehavior, ValidationBehavior (pipeline MediatR)
│   │   └── DependencyInjection/
│   │
│   ├── Corral.Infrastructure/  # Persistence EF Core — dépend de Domain + Application
│   │   ├── Persistence/        # CorralDbContext, FenceEntity, migrations SQLite
│   │   ├── Repositories/       # FenceRepository
│   │   ├── Mappers/            # FenceEntity ↔ Domain Fence
│   │   └── UnitOfWork/
│   │
│   └── Corral.Desktop/         # Application WPF — dépend de Application + Infrastructure
│       ├── ViewModels/          # MainWindowViewModel, CreateZoneDialogViewModel, FenceViewModel
│       ├── Views/               # CreateZoneDialog.xaml
│       ├── Resources/           # Colors.xaml, Brushes.xaml, Styles.xaml, DataTemplates.xaml
│       ├── Converters/          # StringToColorConverter, BoolToVisibilityConverter, etc.
│       ├── Services/            # IDialogService, DialogService
│       ├── Mappers/             # Fence (Domain) → FenceViewModel
│       └── DependencyInjection/
│
└── tests/
    ├── Corral.Domain.Tests/
    └── Corral.Application.Tests/
```

### Règle de dépendance

```
Domain  ←  Application  ←  Infrastructure  ←  Desktop
```

Aucune dépendance inverse. Domain n'a aucune dépendance externe.

### Pipeline MediatR

Chaque commande/query traverse ce pipeline avant d'atteindre son handler :

```
Request → LoggingBehavior → ValidationBehavior → Handler → Response
```

- **LoggingBehavior** : logue le nom, la durée, et les erreurs via `ILogger<T>`
- **ValidationBehavior** : exécute tous les `IValidator<TRequest>` FluentValidation enregistrés et lève `ValidationException` si invalide

### Format des couleurs

Les couleurs sont stockées en format **`#AARRGGBB`** (8 hex + `#`, ex: `#FF0078D4`). C'est le format du `Color.FromHexString()` du Domain. Toujours inclure le canal alpha `FF` pour une couleur opaque.

## Conventions de code

### Structure d'un Command

```
Application/Commands/CreateFence/
├── CreateFenceCommand.cs          ← record immutable
├── CreateFenceCommandValidator.cs ← FluentValidation
└── CreateFenceCommandHandler.cs   ← IRequestHandler<TCommand, TResponse>
```

### Structure d'une Query

```
Application/Queries/GetAllFences/
├── GetAllFencesQuery.cs
└── GetAllFencesQueryHandler.cs
```

### Règles strictes

- **Un type par fichier** — zéro exception
- **Primary constructors** C# 12 partout où possible (sauf WPF windows qui ont besoin d'`InitializeComponent()`)
- **Nullable désactivé** — pas de `?` sur les types référence, pas de `#nullable enable`, pas de `?.` (utiliser `if (x != null)`)
- **Pas de null checks DI dans les constructeurs** — le container DI garantit les instances
- **Constructeur design-time** pour les ViewModels : chaîner avec `: this(null)` et peupler des données de test
- **XML doc** sur tous les membres publics

### Nommage

| Concept | Format |
|---|---|
| Commandes | `[Verbe][Nom]Command` |
| Handlers | `[Verbe][Nom]CommandHandler` |
| Validators | `[Verbe][Nom]CommandValidator` |
| Queries | `Get[Nom][Filtre]Query` |
| Entités EF | `[Nom]Entity` |
| Mappers | `[Source]To[Dest]Mapper` |
| Value Objects | `[Concept]` (Position, Color, Opacity) |

## Démarrage rapide

```bash
# Build
dotnet build

# Base de données (migrations auto-appliquées au démarrage)
dotnet ef database update --project src/Corral.Infrastructure

# Lancer l'application
dotnet run --project src/Corral.Desktop

# Tests
dotnet test
```

La base de données `Corral.db` est créée automatiquement dans le répertoire de travail au premier lancement. Les migrations EF Core sont appliquées via `dbContext.Database.Migrate()` dans `Program.cs`.

## État actuel (Avril 2026)

- [x] Architecture Clean Architecture + CQRS en place
- [x] Pipeline MediatR (logging + validation)
- [x] UI WPF dark theme avec sidebar, grille de zones, status bar
- [x] MVVM avec EventToCommand (Microsoft.Xaml.Behaviors.Wpf)
- [x] Support design-time XAML designer
- [x] Dialog "Créer une zone" fonctionnel
- [x] Serilog (console + fichier)
- [x] Dialog "Éditer une zone" avec UpdateFenceCommand
- [x] Suppression de zone via context menu
- [x] Barre de recherche fonctionnelle (SearchFencesQuery)
- [ ] Overlays visuels réels sur l'écran (fonctionnalité cœur — nécessite une architecture overlay separate)
