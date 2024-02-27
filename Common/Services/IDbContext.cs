using System.Data;

namespace Common.Services;

public interface IDbContext
{
	public IDbConnection CreateConnection();
}