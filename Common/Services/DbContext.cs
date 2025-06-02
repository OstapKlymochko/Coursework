using System.Data;
using Common.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Common.Services
{
    public class DbContext : IDbContext
	{
		private readonly string _connectionString;

		public DbContext(IConfiguration configuration)
		{
			this._connectionString = configuration.GetConnectionString("DefaultConnection")!;
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}

		public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
	}
}
