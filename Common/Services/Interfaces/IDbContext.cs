using System.Data;

namespace Common.Services.Interfaces;

public interface IDbContext
{
    public IDbConnection CreateConnection();
}