using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Runner.Models;
using TestEnvironment.Docker;
using TestEnvironment.Docker.Containers.Postgres;

namespace Runner
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    public class Benchmark
    {
        private string _connectionString;
        private DockerEnvironment _env;

        [Params(100000)]
        public int RowsCount { get; set; }

        [Benchmark(Baseline = true)]
        public void SelectAll_Dapper_Buffered()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var result = connection.Query<Blog>("select id, url, name, created_date, updated_date from blogs").ToList();
            }
        }

        [Benchmark]
        public void SelectAll_Dapper_Ubuffered()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var result = connection.Query<Blog>("select id, url, name, created_date, updated_date from blogs", buffered: false).ToList();
            }
        }

        [Benchmark]
        public void SelectAll_Ef31_FromSqlRaw()
        {
            using (var context = new BloggingContext(_connectionString))
            {
                var result = context.Blogs.FromSqlRaw("select id, url, name, created_date, updated_date from blogs").ToList();
            }
        }

        [Benchmark]
        public void SelectAll_Ef31_FromSqlRaw_AsNoTracking()
        {
            using (var context = new BloggingContext(_connectionString))
            {
                var result = context.Blogs.FromSqlRaw("select id, url, name, created_date, updated_date from blogs").AsNoTracking().ToList();
            }
        }

        [Benchmark]
        public void SelectAll_Ef31_Simple()
        {
            using (var context = new BloggingContext(_connectionString))
            {
                var result = context.Blogs.ToList();
            }
        }

        [Benchmark]
        public void SelectAll_Ef31_Simple_AsNoTracking()
        {
            using (var context = new BloggingContext(_connectionString))
            {
                var result = context.Blogs.AsNoTracking().ToList();
            }
        }

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            _env = new DockerEnvironmentBuilder().SetName("efcore5_tests").AddPostgresContainer("postgres").Build();
            await _env.Up();
            var container = _env.GetContainer<PostgresContainer>("postgres");

            _connectionString = container.GetConnectionString();

            using (var context = new BloggingContext(_connectionString))
            {
                context.Database.EnsureCreated();

                context.AddRange(Enumerable.Range(0, RowsCount).Select(i => new Blog
                {
                    Url = $"1{i}3456{i}8",
                    Name = $"Bobby-{i}-Johns",
                    CreatedDate = DateTime.UtcNow.AddDays(new Random().Next(-10, 10)),
                    UpdatedDate = DateTime.UtcNow
                }).ToList());

                context.SaveChanges();
            }
        }

        [GlobalCleanup]
        public async Task GlobalCleanup()
        {
            await _env.Down();
            await _env.DisposeAsync();
        }
    }
}
