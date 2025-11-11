using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MusicerBeat.Models.Services;

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

            // データベースファイルを、実行方法に関係なく実行ファイルと同じディレクトリに作成する。
            var baseDir = AppContext.BaseDirectory;
            var dbPath = Path.Combine(baseDir, dbFileName);

            // 診断用にフルパスを記録
            AppDiagnosticsService.DatabaseFullPath = Path.GetFullPath(dbPath);

            if (!File.Exists(dbPath))
            {
                using var connection = new SqliteConnection($"Data Source={dbPath}");
                connection.Open();
                connection.Close();
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = dbPath, }.ToString();
            optionsBuilder.UseSqlite(new SqliteConnection(connectionString));
        }
    }
}