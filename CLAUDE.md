# Instructions pour Claude Code — Corral App

Lire `README.md` pour la vision produit et l'architecture complète.

## Conventions C# obligatoires

### Nullable
- **Nullable est désactivé** dans tous les csproj — ne jamais activer
- Pas de `?` sur les types référence (`IMediator _mediator`, pas `IMediator?`)
- Pas de `?.` — utiliser `if (x != null) x.Method()`
- Pas de `#nullable enable`

### Constructeurs
- **Primary constructors** C# 12 partout où c'est possible
- **Exception** : WPF windows/UserControls doivent appeler `InitializeComponent()` dans un corps de constructeur — garder la forme classique
- **Pas de null checks DI** dans les constructeurs — le container garantit les instances
- **Constructeur design-time** pour les ViewModels MVVM : ajouter un constructeur sans paramètre qui chaîne `: this(null)` et peuple des données de test pour le XAML designer

### Un type par fichier
Strictement un seul type (class/interface/record/enum) par fichier `.cs`. Zéro exception.

## WPF / MVVM

- Les événements UI sont câblés aux commands via `Microsoft.Xaml.Behaviors.Wpf` (`EventToCommand`) — pas de code-behind pour la logique
- Les styles et ressources sont centralisés dans `Resources/` (Colors, Brushes, Styles, DataTemplates)
- `IDialogService` pour ouvrir des fenêtres dialog — injecté dans les ViewModels, jamais `new Window()` directement
- Les couleurs sont au format `#AARRGGBB` (ex: `#FF0078D4`)

## Pipeline MediatR

Ordre des behaviors : `LoggingBehavior → ValidationBehavior → Handler`

Chaque nouveau Command doit avoir son validator FluentValidation dans le même dossier.

## Logging

Utiliser `ILogger<T>` de `Microsoft.Extensions.Logging` dans Application et Infrastructure — jamais référencer Serilog directement en dehors de `Corral.Desktop/Program.cs`.

## Migrations EF Core

```bash
dotnet ef migrations add [Nom] --project src/Corral.Infrastructure
dotnet ef database update --project src/Corral.Infrastructure
```

Les migrations sont appliquées automatiquement au démarrage via `dbContext.Database.Migrate()`.
