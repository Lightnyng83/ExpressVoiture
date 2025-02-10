# ExpressVoitures

**ExpressVoitures** est une application web développée en ASP.NET Core qui permet de gérer l'inventaire de véhicules pour une concession.  
L'application intègre ASP.NET Core Identity pour la gestion sécurisée des utilisateurs et utilise Entity Framework Core pour la persistance des données dans une base SQL Server.

## Table des matières

- [Fonctionnalités](#fonctionnalités)
- [Prérequis](#prérequis)
- [Installation et configuration](#installation-et-configuration)
- [Architecture de l'application](#architecture-de-lapplication)
- [Modèle de données](#modèle-de-données)
- [Utilisation](#utilisation)
- [Tests](#tests)
- [Licence](#licence)

## Fonctionnalités

- **Gestion des véhicules** : Création, édition, consultation et suppression de véhicules.
- **Gestion des marques et modèles** : Possibilité d'ajouter ou de sélectionner des marques et des modèles de véhicules.
- **Authentification et sécurité** : Intégration d'ASP.NET Core Identity pour la gestion des comptes utilisateurs.
- **Validation côté serveur** : Contrôle des données saisies via des validations personnalisées dans les ViewModels.
- **Gestion des images** : Ajout d'un visuel pour chaque véhicule avec stockage sur le serveur.

## Prérequis

- [.NET 8 ou ultérieur](https://dotnet.microsoft.com/)
- [SQL Server LocalDB](https://docs.microsoft.com/fr-fr/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou tout autre éditeur compatible

## Installation et configuration

1. **Cloner le dépôt :**
   ```bash
   git clone https://github.com/votre-utilisateur/ExpressVoitures.git
   cd ExpressVoitures
   ```

2. **Configurer la chaîne de connexion :**
   Dans le fichier `ApplicationDbContext.cs`, la méthode `OnConfiguring` définit la chaîne de connexion à SQL Server LocalDB :

   ```csharp
   optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Initial Catalog=aspnet-ExpressVoitures-cea89d45-b133-413b-a3d4-a3572252c30d;Trusted_Connection=True;");
   ```

   Adaptez cette chaîne de connexion en fonction de votre environnement.

3. **Appliquer les migrations :**
   Depuis la console du Gestionnaire de Package ou via la CLI .NET, exécutez :
   ```bash
   Update-Database
   ```

4. **Exécuter l'application :**
   Lancez l'application depuis Visual Studio ou avec la commande :
   ```bash
   dotnet run
   ```

## Architecture de l'application

L'application suit une architecture MVC (Model-View-Controller) :

- **Models** : Représente les entités métier telles que Car, CarBrand, CarModel et CarBrandModel.
- **Views** : Utilise Razor pour l'interface utilisateur (formulaires de création, édition, consultation et suppression).
- **Controllers** : Gère les actions utilisateur. Par exemple, CarController orchestre les opérations CRUD sur les véhicules.
- **Services** : Contient la logique métier dans des classes implémentant des interfaces (ex. : ICarService).
- **Identity** : L'authentification et la gestion des utilisateurs sont assurées par ASP.NET Core Identity via IdentityDbContext.

## Modèle de données

L'application utilise Entity Framework Core avec un contexte de données personnalisé qui hérite de IdentityDbContext.
Voici le code du contexte de données principal :

```csharp
using ExpressVoitures.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Data
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<CarBrand> CarBrands { get; set; }
        public virtual DbSet<CarBrandModel> CarBrandModels { get; set; }
        public virtual DbSet<CarModel> CarModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Initial Catalog=aspnet-ExpressVoitures-cea89d45-b133-413b-a3d4-a3572252c30d;Trusted_Connection=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuration des clés composées pour Identity
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            // Relation entre Car et CarBrandModel
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasOne(d => d.CarBrandModel).WithMany(p => p.Cars)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Car_CarBrandModelId");
            });

            // Configuration de la table de liaison
            modelBuilder.Entity<CarBrandModel>(entity =>
            {
                entity.HasKey(e => e.CarBrandModelId).HasName("PK_CarBrandModelId_1");

                entity.HasOne(d => d.CarBrand).WithMany(p => p.CarBrandModels)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CarBrandModelId_CarBrand1");

                entity.HasOne(d => d.CarModel).WithMany(p => p.CarBrandModels)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CarBrandModelId_CarModel1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
```

### Explication du modèle

- **Cars** : La table Cars contient les informations relatives aux véhicules (année, prix, finition, image, etc.) et référence la table CarBrandModels via la clé étrangère CarBrandModelId.
- **CarBrands** : Stocke les marques des véhicules (ex. : Toyota, Honda).
- **CarModels** : Stocke les modèles des véhicules (ex. : Corolla, Civic).
- **CarBrandModels** : Table de liaison entre CarBrands et CarModels.
- **Identity** : En héritant de IdentityDbContext, le contexte intègre les tables nécessaires pour la gestion des utilisateurs.

## Utilisation

1. **Démarrage de l'application :**
   - Après avoir appliqué les migrations, lancez l'application pour accéder à l'interface d'administration.
   - Vous pouvez créer, modifier, consulter et supprimer des véhicules.

2. **Gestion des utilisateurs :**
   - Grâce à ASP.NET Core Identity, la gestion des comptes utilisateurs et des rôles est intégrée à l'application.

## Tests

L'application inclut des tests unitaires et des tests d'intégration pour valider le comportement des contrôleurs et la persistance des données.
Les tests d'intégration utilisent une base de données en mémoire (InMemory) pour simuler les interactions sans avoir à lancer le serveur via HTTP.