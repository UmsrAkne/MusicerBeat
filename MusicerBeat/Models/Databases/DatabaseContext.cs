using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MusicerBeat.Models.Databases
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DatabaseContext : DbContext
    {
        public DbSet<SoundFile> SoundFiles { get; set; }

        public DbSet<ListenHistory> ListenHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string dbFileName = "database.sqlite";
            if (!File.Exists(dbFileName))
            {
                using var connection = new SqliteConnection($"Data Source={dbFileName}");
                connection.Open();
                connection.Close();
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = dbFileName, }.ToString();
            optionsBuilder.UseSqlite(new SqliteConnection(connectionString));
        }
    }
}