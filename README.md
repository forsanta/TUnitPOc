# POC TUnit

Petit projet pour découvrir [TUnit](https://tunit.dev), le framework de tests .NET moderne
basé sur **Microsoft.Testing.Platform** et la **génération de source** (pas de réflexion à l'exécution, compatible Native AOT).

- TUnit **1.83.0**, .NET **10**
- Domaine testé : un panier e-commerce (`Cart`, `DiscountCalculator`, `OrderService`)
- **62 tests** générés à partir de ~30 méthodes, exécutés en ~1,2 s

## Lancer les tests

```bash
dotnet test
```

Le projet de test est un **exécutable** : on peut aussi le lancer directement.

```bash
dotnet run --project tests/TUnitPoc.Tests
```

Filtrer par catégorie :

```bash
dotnet run --project tests/TUnitPoc.Tests -- --treenode-filter "/*/*/*/*[Category=Pricing]"
```

Un rapport HTML est généré dans `TestResults/`.

## Structure (une section d'article = un dossier)

| Dossier | Fonctionnalité | Attributs / API |
|---|---|---|
| `01_Basics` | Assertions asynchrones et fluides, `Assert.Multiple`, exceptions | `[Test]`, `Assert.That(...)`, `.And`, `.Throws<T>()` |
| `02_DataDriven` | Tests paramétrés | `[Arguments]`, `[MethodDataSource]`, `[MatrixDataSource]` + `[Matrix]` |
| `03_Lifecycle` | Hooks à tous les niveaux | `[Before(Test/Class/TestSession)]`, `[After(...)]`, `TestContext` |
| `04_Fixtures` | Ressource partagée initialisée une seule fois | `[ClassDataSource<T>(Shared = ...)]`, `IAsyncInitializer` |
| `05_Orchestration` | Ordre et parallélisme | `[DependsOn]`, `[NotInParallel]`, `[ParallelLimiter<T>]`, `[Repeat]` |
| `06_Resilience` | Tests instables et délais | `[Retry]`, `[Timeout]` + `CancellationToken` injecté |
| `07_Metadata` | Organisation et filtrage | `[Category]`, `[Property]`, `[DisplayName]`, `[Skip]`, skip personnalisé |

## Points marquants pour l'article

1. **Tout est async.** Les assertions s'`await`ent : impossible d'oublier un `await` sans warning de l'analyseur.
2. **Parallèle par défaut.** Tous les tests tournent en parallèle ; on *désactive* au besoin avec `[NotInParallel("clé")]`, ce qui oblige à penser à l'isolation dès le départ.
3. **Hooks sans héritage.** `[Before(Class)]`, `[Before(TestSession)]`… sur n'importe quelle méthode, plus besoin de `IClassFixture` / `ICollectionFixture` (xUnit) ou de `[OneTimeSetUp]` (NUnit).
4. **Fixtures partagées déclaratives.** `Shared = PerTestSession | PerClass | PerAssembly | Keyed | None`, avec cycle de vie async (`IAsyncInitializer` / `IAsyncDisposable`).
5. **`[DependsOn]`.** Les tests de scénario deviennent explicites ; si l'étape parente échoue, la suivante est *ignorée* au lieu d'échouer en cascade.
6. **Génération de source.** Les tests sont découverts à la compilation : erreurs de paramètres d'`[Arguments]` détectées par le compilateur, démarrage rapide, AOT possible.
7. **Une vraie anecdote.** Lors de l'écriture du POC, un test a échoué sur un total mal calculé à la main (124,96 au lieu de 125,01) : le message `Expected to be 124,96 but found 125,01` + l'expression `Assert.That(cart.Total).IsEqualTo(expectedTotal)` rendent le diagnostic immédiat.

## Correspondance avec xUnit / NUnit

| Besoin | xUnit | NUnit | TUnit |
|---|---|---|---|
| Test simple | `[Fact]` | `[Test]` | `[Test]` |
| Données en ligne | `[InlineData]` | `[TestCase]` | `[Arguments]` |
| Données par méthode | `[MemberData]` | `[TestCaseSource]` | `[MethodDataSource]` |
| Combinatoire | — | `[Combinatorial]` + `[Values]` | `[MatrixDataSource]` + `[Matrix]` |
| Setup par test | constructeur | `[SetUp]` | `[Before(Test)]` (ou constructeur) |
| Setup par classe | `IClassFixture<T>` | `[OneTimeSetUp]` | `[Before(Class)]` / `[ClassDataSource<T>]` |
| Ignorer | `Skip = "..."` | `[Ignore]` | `[Skip("...")]` |
| Relancer si échec | — | `[Retry]` | `[Retry]` |
| Ordre | — | `[Order]` | `[DependsOn]` |
