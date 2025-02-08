using ExpressVoitures.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoiture.Tests.Config
{
    public class TestApplicationDbContext : ApplicationDbContext
    {
        public TestApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // On annule la configuration par défaut pour ne pas ajouter SQL Server.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Si les options sont déjà configurées (par exemple via UseInMemoryDatabase dans les tests),
            // on ne fait rien.
            if (!optionsBuilder.IsConfigured)
            {
                // Optionnel : on peut forcer l'utilisation d'une base en mémoire par défaut pour le test.
                optionsBuilder.UseInMemoryDatabase("TestDb");
            }
        }
    }
}
