using System.Reflection;

namespace Common.CommonTypes.Interfaces
{
	public interface IGenericRepository<T> where T : class
	{
		public Task<List<T>> GetAllAsync();

		public Task<T?> GetByIdAsync(int id);

		public Task CreateAsync(T entity, bool excludeKey = false);

		public Task UpdateAsync(T entity, bool excludeKey = false);

		public Task DeleteAsync(T entity);

		protected Dictionary<string, dynamic?> GetColumns(T entity, bool excludeKey = false);

		protected string? GetKeyColumnName();

		protected string? GetKeyPropertyName();

		protected dynamic? GetKeyColumnValue(T entity);

		protected string GetPropertyNames(bool excludeKey = false);

		protected IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false);

		protected string ParseValue(dynamic value);
	}
}
