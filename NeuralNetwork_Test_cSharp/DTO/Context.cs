using Microsoft.EntityFrameworkCore;
using NeuralNetwork_Test_cSharp.DTO.DatabaseModel;

namespace NeuralNetwork_Test_cSharp.DTO
{
    public class Context : DbContext
    {
        private readonly string _connectionString;
        public Context(string connectionString)
        {
            _connectionString = connectionString;
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_connectionString}");
        }

        public DbSet<SimulationDb> Simulations { get; set; }
        public DbSet<GenerationResultDb> GenerationResults { get; set; }
        public DbSet<UnitDb> Units { get; set; }
        public DbSet<UnitStepDb> UnitSteps { get; set; }
    }
}
