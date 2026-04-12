# Copilot Instructions — Corral App

Lire `README.md` pour la vision produit et l'architecture complète.

## Ce projet en bref

Corral est une app WPF .NET 10 qui gère des zones visuelles (fences) sur l'écran.
Architecture : Clean Architecture + DDD + CQRS (MediatR) + FluentValidation + Serilog + EF Core SQLite.

## Conventions impératives

### Nullable désactivé
Nullable est désactivé dans tous les csproj. Ne jamais utiliser :
- `Type?` sur les types référence
- `?.` (null-conditional)
- `#nullable enable`

Utiliser des vérifications nulles explicites : `if (x != null) x.Method()`

### Primary constructors C# 12
Toujours utiliser la forme `public class Foo(IService service) : Base` sauf pour les WPF windows
(qui ont besoin de `InitializeComponent()` dans un corps de constructeur classique).

### Un type par fichier
Un seul type `.cs` par fichier — toujours.

### Pas de null checks sur les paramètres DI
Le container DI garantit les instances. Pas de `ArgumentNullException.ThrowIfNull(service)` dans les constructeurs.

## Structure CQRS

```
Application/Commands/[Verbe][Nom]/
├── [Verbe][Nom]Command.cs          ← record immutable
├── [Verbe][Nom]CommandValidator.cs ← FluentValidation (obligatoire pour les commands)
└── [Verbe][Nom]CommandHandler.cs   ← IRequestHandler<TCommand, TResponse>

Application/Queries/Get[Nom][Filtre]/
├── Get[Nom][Filtre]Query.cs
└── Get[Nom][Filtre]QueryHandler.cs
```

## Couleurs
Format `#AARRGGBB` obligatoire (ex: `#FF0078D4`). Le `Color.FromHexString()` du Domain valide ce format.

## Logging
Utiliser `ILogger<T>` (Microsoft.Extensions.Logging) — pas de référence directe à Serilog hors de `Program.cs`.
