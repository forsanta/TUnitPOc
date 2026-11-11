# TUnit : faut-il remplacer xUnit ? Retour sur un POC

En mission comme en formation, une question revient régulièrement :

xUnit, NUnit ou MSTest ?

Dans un précédent article sur la migration .NET Framework vers .NET moderne, j'expliquais que les tests sont une des préparations les plus utiles d'un projet. Sur l'une des applications, nous étions d'ailleurs passés de MSTest à xUnit.

Depuis quelques mois, un quatrième nom revient pourtant de plus en plus : **TUnit**.

Plutôt que de me contenter de la documentation, j'ai voulu le tester sur un cas concret.

## Le POC

L'idée était simple :

un petit domaine métier, et un projet de tests organisé par fonctionnalité.

Le domaine est un panier e-commerce : ajout de produits, codes promo, frais de port, et un service de commande qui vérifie le stock.

Côté tests :

- TUnit 1.83 sur .NET 10 ;
- 7 thèmes, un dossier par thème ;
- 62 tests générés à partir d'une trentaine de méthodes ;
- environ 1,2 seconde d'exécution.

Voici ce que j'en retiens.

## 1. Les tests sont découverts à la compilation

C'est la principale différence technique avec xUnit.

xUnit découvre les tests par réflexion au moment de l'exécution. TUnit, lui, utilise un *source generator* : les tests sont identifiés dès la compilation.

Cela a plusieurs conséquences :

- un démarrage plus rapide sur les grosses suites de tests ;
- la compatibilité avec Native AOT ;
- certaines erreurs remontent dès le build, par exemple un paramètre de test qui ne correspond pas aux données fournies.

TUnit s'appuie aussi sur Microsoft.Testing.Platform. Le projet de test devient un exécutable : on peut le lancer avec `dotnet test`, mais aussi directement avec `dotnet run`.

## 2. Des assertions asynchrones et fluides

Avec xUnit, on écrit :

```csharp
Assert.Equal(49m, cart.Total);
```

Avec TUnit :

```csharp
await Assert.That(cart.Total)
    .IsGreaterThan(40m)
    .And.IsEqualTo(49m);
```

Toutes les assertions sont awaitées, et elles sont intégrées au framework. Pas besoin d'ajouter une librairie d'assertions.

On peut aussi regrouper plusieurs vérifications pour obtenir tous les échecs d'un coup, plutôt que de s'arrêter au premier :

```csharp
using (Assert.Multiple())
{
    await Assert.That(cart.Lines).Count().IsEqualTo(2);
    await Assert.That(cart.SubTotal).IsEqualTo(119.87m);
    await Assert.That(cart.DiscountCode).IsNull();
}
```

Petite anecdote : au premier lancement, un test a échoué.

TUnit avait raison. C'est moi qui avais mal calculé un total à la main : 124,96 au lieu de 125,01.

Le message d'erreur affichait la valeur attendue, la valeur obtenue et l'expression exacte de l'assertion. Le diagnostic a été immédiat.

## 3. Une seule notion de test

Avec xUnit, on distingue `[Fact]` et `[Theory]`.

Avec TUnit, tout est `[Test]`, avec ou sans données :

```csharp
[Test]
[Arguments(100, "WELCOME10", 90)]
[Arguments(100, "BLACKFRIDAY", 70)]
[Arguments(100, null, 100)]
public async Task Discount_codes_are_applied(decimal amount, string? code, decimal expected)
{
    await Assert.That(DiscountCalculator.Apply(amount, code)).IsEqualTo(expected);
}
```

Les équivalences sont assez directes :

- `[InlineData]` devient `[Arguments]` ;
- `[MemberData]` devient `[MethodDataSource]`.

TUnit ajoute en plus `[Matrix]`, qui génère toutes les combinaisons de valeurs :

```csharp
[Test]
[MatrixDataSource]
public async Task Shipping_is_never_negative_nor_above_9_euros(
    [Matrix(0, 10, 49.99, 50)] decimal amount,
    [Matrix(true, false)] bool express)
```

4 montants x 2 modes de livraison : 8 tests générés.

## 4. Un cycle de vie basé sur des attributs

C'est probablement le point où le gain est le plus visible.

Avec xUnit, partager une ressource entre plusieurs classes de tests demande trois éléments : `ICollectionFixture<T>`, l'attribut `[Collection]`, et une classe de définition de collection.

Avec TUnit, un seul attribut suffit :

```csharp
[ClassDataSource<StockFixture>(Shared = SharedType.PerTestSession)]
public class OrderServiceTests(StockFixture fixture)
```

La fixture implémente `IAsyncInitializer` : TUnit l'initialise une seule fois, avant la première utilisation, puis la partage avec toutes les classes qui la demandent.

Pour le reste du cycle de vie, on utilise des attributs sur n'importe quelle méthode :

- `[Before(Test)]` / `[After(Test)]` ;
- `[Before(Class)]` ;
- `[Before(Assembly)]` ;
- `[Before(TestSession)]`.

Plus besoin d'interfaces ou de classe de base.

## 5. Tout est parallèle par défaut

C'est un changement important.

Avec xUnit, les classes de tests tournent en parallèle, mais les tests d'une même classe s'exécutent les uns après les autres.

Avec TUnit, tous les tests tournent en parallèle, y compris dans une même classe.

On restreint ensuite au cas par cas :

- `[NotInParallel("clé")]` pour sérialiser les tests qui partagent une ressource ;
- `[ParallelLimiter<T>]` pour limiter le nombre de tests simultanés, par exemple face à une API avec quota.

Cela oblige à penser l'isolation des tests dès le départ.

## 6. Des outils absents de xUnit

TUnit propose aussi plusieurs attributs qui répondent à des besoins concrets, surtout sur des tests d'intégration :

- `[Retry(3)]` relance un test instable en cas d'échec ;
- `[Repeat(5)]` exécute un test plusieurs fois ;
- `[Timeout(2000)]` injecte un `CancellationToken` annulé à l'expiration du délai ;
- `[DependsOn]` fait attendre un test qu'un autre ait réussi.

`[DependsOn]` est particulièrement intéressant pour les scénarios en plusieurs étapes :

```csharp
[Test]
[DependsOn(nameof(Step1_stock_is_provisioned))]
public async Task Step2_reservation_decrements_stock()
```

Si l'étape 1 échoue, l'étape 2 est ignorée au lieu d'échouer à son tour. Le rapport montre directement l'origine du problème.

## Les points de vigilance

Avant de migrer, il faut garder plusieurs éléments en tête.

**xUnit v3 a réduit l'écart.**
Il tourne lui aussi sur Microsoft.Testing.Platform, propose un `TestContext.Current`, des fixtures d'assembly et `Assert.Skip`. La comparaison est plus juste face à xUnit v3 que face à xUnit v2.

**TUnit est jeune.**
Le projet est porté principalement par un seul mainteneur, et son API a beaucoup évolué entre les versions. xUnit, lui, est le standard de fait, avec un écosystème très large.

**Le parallélisme par défaut a un coût.**
Une suite existante qui partage un état statique ou une base de données peut devenir instable du jour au lendemain lors d'une migration.

## Le bon outil pour le bon contexte

Après ce POC, mon avis est assez clair.

Pour des tests unitaires simples, xUnit v3 reste un choix sûr.

Pour des tests d'intégration plus lourds (base de données, conteneurs, scénarios enchaînés), TUnit apporte un confort appréciable : fixtures partagées en un attribut, retry, dépendances entre tests, parallélisme maîtrisé.

Je le recommanderais sur un nouveau projet, avant de l'imposer sur un existant.

Et surtout, comme pour une migration, le choix du framework compte moins que la présence des tests eux-mêmes.

Le code du POC est disponible ici : [lien GitHub]
